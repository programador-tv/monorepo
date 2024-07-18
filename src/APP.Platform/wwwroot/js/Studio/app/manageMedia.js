export default class {
    static setVideoStream(camera,e){
        
        e.roomSet.myVideo.srcObject = camera
        // e.roomStreams.main.video = camera
        e.roomStreams.main.camera = camera
        e.roomStreams.main.merger.addStream(camera, {
            x: e.roomStreams.main.merger.width - 183,
            y: e.roomStreams.main.merger.height - 110,
            width: 173,
            height: 100,
            mute: true,
            index: 1
        })
        e.roomSet.videoSwitch.style.backgroundColor = "#B5DCF0"
        setTimeout(() => {
            e.roomSet.myPreview.style.display = "none"
            e.roomSet.myVideo.style.display = "block"
        }, 300);

    }
    static unsetVideoStream(e){
        e.roomStreams.main.camera
        .getTracks()
        .forEach(function(track) {
            track.stop();
        });
         e.roomStreams.main.merger.removeStream(e.roomStreams.main.camera)
        e.roomStreams.main.camera = null
        e.roomSet.myVideo.srcObject = null
        e.roomSet.myPreview.style.display = "block"
        e.roomSet.myVideo.style.display = "none"
        e.roomSet.videoSwitch.style.backgroundColor = "grey"
    }
    static async videoManeger(e){
        
        if(!e.roomConfig.videoActive){
            if (navigator.mediaDevices.getUserMedia) {
                navigator.mediaDevices
                .getUserMedia({ video: true, audio: false })
                .then(c =>
                     this.setVideoStream(c,e))
                     .catch(error => console.log(error));
               }
        }else{
            this.unsetVideoStream(e)
        }
        e.roomConfig.videoActive = !e.roomConfig.videoActive
    }

    static setAudioStream(audio,e){
        e.roomStreams.main.audio = audio
        e.roomStreams.main.merger.addStream(audio)
        e.roomSet.audioSwitch.style.backgroundColor = "#B5DCF0"
    }
    static unsetAudioStream(e){
        
        e.roomStreams.main.audio.getTracks().forEach((track) => {
            track.stop();
        });
         e.roomStreams.main.merger.removeStream(e.roomStreams.main.audio)
        e.roomSet.audioSwitch.style.backgroundColor = "#8A8A8A"
        
    }
    static async audioManeger(e){
        if(!e.roomConfig.audioActive){
            if (navigator.mediaDevices.getUserMedia) {
                navigator.mediaDevices
                .getUserMedia({ video: false, audio: true })
                .then(c => this.setAudioStream(c,e))
                .catch(error => console.log(error));
               }

        }else{
            this.unsetAudioStream(e)
        }
        e.roomConfig.audioActive = !e.roomConfig.audioActive
    }

    static setScreenStream(screen,e){
        
        e.roomStreams.main.screen = screen
        // e.roomSet.myScreen.srcObject = screen
        e.roomStreams.main.merger.addStream(screen, {
            x: 0,
            y: 0,
            width: e.roomStreams.main.merger.width,
            height: e.roomStreams.main.merger.height,
            mute: false
        })
        screen.getTracks()[0].onended = ()=>{

            e.roomStreams.main.merger.removeStream(e.roomStreams.main.screen)
            e.roomStreams.main.screen = null
            e.roomSet.myScreen.srcObject = null
            e.roomSet.screenSwitch.style.backgroundColor = "#8A8A8As"
            e.roomConfig.screenActive = !e.roomConfig.screenActive
         
        }
        e.roomSet.screenSwitch.style.backgroundColor = "#B5DCF0"
    }
    static unsetScreenStream(e){
        if(!e.roomStreams.main.screen) return
        e.roomStreams.main.screen.getTracks().forEach((track) => {
            track.stop();
        });
         e.roomStreams.main.merger.removeStream(e.roomStreams.main.screen)
        e.roomStreams.main.screen = null

        e.roomSet.screenSwitch.style.backgroundColor = "#8A8A8A"
    }
    static async screenManeger(e){
        if(!e.roomConfig.screenActive){
            if (navigator.mediaDevices.getDisplayMedia) {
                navigator.mediaDevices
                .getDisplayMedia({
                    video: true, // compartilhar vídeo
                    audio: {
                        echoCancellation: true, // cancelar eco
                        noiseSuppression: true, // supressão de ruído
                        sampleRate: 44100 // taxa de amostragem do áudio
                    }
                })
                .then(c => this.setScreenStream(c,e))
                .catch(error => console.log(error));
            }

        }else{
            this.unsetScreenStream(e)
        }
        e.roomConfig.screenActive = !e.roomConfig.screenActive
    }
}