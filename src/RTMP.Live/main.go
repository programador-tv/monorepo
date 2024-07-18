package main

import (
	"context"
	"fmt"
	"io"
	"net/http"
	"net/url"
	"os"
	"os/exec"
	"path/filepath"
	"strings"
	"sync"
	"time"

	"github.com/joho/godotenv"
	"github.com/nareix/joy4/av/avutil"
	"github.com/nareix/joy4/av/pubsub"
	"github.com/nareix/joy4/format"
	"github.com/nareix/joy4/format/rtmp"
)

var client *http.Client
var coreURL string

func init() {
	format.RegisterAll()
	print("App start")

	err := godotenv.Load()
	if err != nil {
		fmt.Println("Error loading .env file:", err)
	}

	coreURL = os.Getenv("CORE_URL")
	if coreURL == "" {
		fmt.Println("CORE_URL not set")
		os.Exit(1)
	}

	client = &http.Client{}
}

func readError(ffmpegStderr io.ReadCloser) {
	for {
		buf := make([]byte, 1024)
		n, err := ffmpegStderr.Read(buf)
		if err != nil {
			break
		}
		fmt.Println("FFmpeg:", string(buf[:n]))
	}
}

func runFFmpeg(cmd *exec.Cmd, ctx context.Context) {
	fmt.Println("Will run")
	err := cmd.Start()

	if err != nil {
		fmt.Println("FFmpeg exited with error:", err)
	}
	go func() {
		<-ctx.Done()
		if err := cmd.Process.Kill(); err != nil {
			fmt.Println("Failed to kill FFmpeg process:", err)
		}
	}()

	if err := cmd.Wait(); err != nil {
		fmt.Println("FFmpeg process exited with error:", err)
	} else {
		fmt.Println("FFmpeg process exited successfully")
	}
}

func validateKey(conn *rtmp.Conn) string {
	return extractStreamKey(conn.URL)
}

func fetchIDFromToken(token string) (string, error) {
	resp, err := client.Get(fmt.Sprintf("%s/api/lives/key/%s", coreURL, token))
	if err != nil {
		return "", err
	}
	defer resp.Body.Close()
	if resp.StatusCode != http.StatusOK {
		return "", fmt.Errorf("failed to fetch id: %s", resp.Status)
	}

	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return "", err
	}

	id := strings.Trim(string(body), "\"")
	return id, nil
}

func updateLiveOn(id string) error {
	resp, err := client.Get(fmt.Sprintf("%s/api/lives/on/%s", coreURL, id))

	if err != nil {
		return err
	}

	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		return fmt.Errorf("failed to update live: %s", resp.Status)
	}

	return nil
}

func startLiveUpdateRoutine(ctx context.Context, id string) {
	if err := updateLiveOn(id); err != nil {
		fmt.Println("Error updating live status:", err)
	}

	ticker := time.NewTicker(20 * time.Second)
	defer ticker.Stop()

	for {
		select {
		case <-ticker.C:
			if err := updateLiveOn(id); err != nil {
				fmt.Println("Error updating live status:", err)
			}
		case <-ctx.Done():
			return
		}
	}
}

func main() {
	server := &rtmp.Server{}
	l := &sync.RWMutex{}

	type Channel struct {
		que *pubsub.Queue
	}

	channels := map[string]*Channel{}

	server.HandlePublish = func(conn *rtmp.Conn) {
		token := validateKey(conn)

		id, err := fetchIDFromToken(token)
		if err != nil {
			fmt.Println("Error fetching id:", err)
			return
		}

		dir, err := os.Getwd()
		if err != nil {
			fmt.Println("Error fetching Getwd:", err)
			return
		}
		baseDirectory := filepath.Join(dir, "Assets", "Lives")
		if _, err := os.Stat(baseDirectory); err != nil {
			if os.IsNotExist(err) {
				if err := os.MkdirAll(baseDirectory, os.ModePerm); err != nil {
					fmt.Println("Error creating directory:", err)
					return
				}
				fmt.Printf("Directory created: %s\n", baseDirectory)
			} else {
				fmt.Println("Error checking directory:", err)
				return
			}
		} else {
			fmt.Printf("Directory already exists: %s\n", baseDirectory)
		}
		hlsSegmentPath := filepath.Join(baseDirectory, fmt.Sprintf("%s-%%d.ts", id))
		hlsPlaylistPath := filepath.Join(baseDirectory, fmt.Sprintf("%s.m3u8", id))

		fmt.Println("Start broadcast")
		ctx, cancel := context.WithCancel(context.Background())
		cmd := exec.CommandContext(ctx, "ffmpeg",
			"-re", "-fflags", "+genpts",
			"-i", fmt.Sprintf("rtmp://localhost/live/%s", token),
			"-err_detect", "ignore_err",
			"-vsync", "cfr",
			"-c:v", "libx264",
			"-r", "30",
			"-g", "5",
			"-crf", "9",
			"-s", "1920x1080",
			"-preset", "ultrafast",
			"-tune", "zerolatency",
			"-pix_fmt", "yuv420p",
			"-c:a", "aac",
			"-b:a", "128k",
			"-hls_flags", "append_list+omit_endlist",
			"-f", "hls",
			"-hls_playlist_type", "event",
			"-hls_time", "3",
			"-hls_list_size", "0",
			"-hls_segment_filename", hlsSegmentPath,
			hlsPlaylistPath)

		ffmpegStderr, err := cmd.StderrPipe()
		if err != nil {
			fmt.Println("Fail get input stream:", err)
			cancel()
			return
		}

		streams, _ := conn.Streams()

		l.Lock()
		ch := channels[conn.URL.Path]
		if ch == nil {
			ch = &Channel{}
			ch.que = pubsub.NewQueue()
			ch.que.WriteHeader(streams)
			channels[conn.URL.Path] = ch
		}
		l.Unlock()

		if ch == nil {
			cancel()
			return
		}

		go readError(ffmpegStderr)
		go runFFmpeg(cmd, ctx)
		go startLiveUpdateRoutine(ctx, id) // Start the update routine

		avutil.CopyPackets(ch.que, conn)

		l.Lock()
		delete(channels, conn.URL.Path)
		l.Unlock()
		ch.que.Close()
		cancel()
	}

	server.HandlePlay = func(conn *rtmp.Conn) {
		l.RLock()
		ch := channels[conn.URL.Path]
		l.RUnlock()

		if ch == nil {
			fmt.Println("Channel not found:", conn.URL.Path)
			return
		}

		cursor := ch.que.Latest()

		avutil.CopyFile(conn, cursor)
	}

	server.ListenAndServe()
}

func extractStreamKey(urlObj *url.URL) string {
	parts := strings.Split(urlObj.Path, "/")
	if len(parts) > 0 {
		return parts[len(parts)-1]
	}
	return ""
}
