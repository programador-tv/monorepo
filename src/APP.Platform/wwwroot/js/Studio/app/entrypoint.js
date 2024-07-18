import QueryString from '/js/Studio/Utils/QueryString.js'
import manageMedia from './manageMedia.js';
import Media from './Media.js';


export default class Entrypoint {
    connection;
    url;
    roomKey;
    mentorKey;
    publicKey;
    media;
    mediaLive;
    liveStream;
    finalStream;
    mediaRecorder;
    roomSet = {
        titulo: document.querySelector('#titulo'),
        descricao: document.querySelector('#desc'),
        txtStart: document.querySelector('#text-start'),
        participantPreview: document.querySelector('#you'),
        participantVideo: document.querySelector('#remoteVideo'),
        myPreview: document.querySelector('#me'),
        myVideo: document.querySelector('#localVideo'),
        myScreen: document.querySelector('#screenVideo'),
        startBtn: document.querySelector("#startBtn"),
        videoSwitch: document.querySelector('#switchCamera'),
        audioSwitch: document.querySelector('#switchAudio'),
        screenSwitch: document.querySelector('#switchScreen'),
        currentRoom: document.querySelector('#currentRoom'),
        ByeBtn: document.querySelector('#hangupBtn'),
        switchLive: document.querySelector('#goLive'),
        isLive: document.querySelector('#isLive'),
        live: document.querySelector('#live'),
        contador: document.querySelector('#accountant-start'),
        oneShot: document.querySelector('#oneShot'),
        repeat: document.querySelector('#repeat'),
        keep: document.querySelector('#keep'),
        patternGroup: document.querySelector('#patternGroup'),
    }

    roomConfig = {
        haveThumb: false,
        inLive: false,
        isIndividual: false,
        tipoSala: null,
        audioActive: false,
        videoActive: false,
        screenActive: false,
    }
    roomStreams = {
        main: {
            video: new MediaStream(),
            audio: new MediaStream(),
            screen: new MediaStream(),
            merger: new VideoStreamMerger({
                width: 1920,
                height: 1080,
                audioContext: null, // Supply an external AudioContext (for audio effects)
            })
        }
    }

    startUp() {
        this.getInfoFromParams()
            .then(() => {
                this.renderLayout()
                this.applyFunctionalities()
                this.startComunication()
            })
            // .catch(e => {
            //     throw e
            //     //window.location.href = 'https://programador.tv/Ensinar'
            // })
    }
    startComunication() {
        //
    }
    applyFunctionalities() {
        

        this.roomSet.videoSwitch.onclick = () => manageMedia.videoManeger(this)
        this.roomSet.audioSwitch.onclick = () => manageMedia.audioManeger(this)
        this.roomSet.screenSwitch.onclick = () => manageMedia.screenManeger(this)
        this.roomSet.ByeBtn.onclick = () => this.closeRoom()
        this.roomSet.startBtn.onclick = () => this.openRoom()
        this.roomSet.switchLive.onclick = () => this.goLive()
        
       
    }

    getSupportedMimeType(types) {
        return types.find((type) => MediaRecorder.isTypeSupported(type)) || null;
    }

    goLive() {
        this.media = new Media()
        this.finalStream = this.media.mergeScreenAndVideoAndAudio(
            this.roomStreams.main
            )
        this.roomStreams.main.merger.start()
        this.roomSet.myScreen.srcObject = this.roomStreams.main.merger.result
        
        let types = ['video/webm;codecs=h264',
            'video/webm',
            'video/mp4'];
        let mimeType = this.getSupportedMimeType(types);
        if (null === mimeType) {
            alert("Navegador não suporta gravação de vídeo.");
            throw new Error("Navegador não suporta gravação de vídeo.");
        }
        this.mediaRecorder = new MediaRecorder(this.roomStreams.main.merger.result, {
            mimeType: mimeType,
            videoBitsPerSecond: 10000000,
            audioBitsPerSecond: 128000,
            video: {
                width: 1920,
                height: 1080,
                frameRate: 30,
            },
            audio: {
                codec: 'aac',
                bitrate: '128k',
                channels: 2,
                sampleRate: 44100,
            },
        });
        if (!this.mediaRecorder) {
            console.error("Failed to create MediaRecorder instance");
        }

        if (!this.roomConfig.inLive) {

            const videoWorker = new Worker('./js/Studio/app/video-processing-worker.js');
            videoWorker.postMessage(wsurl + '/ws/live/transmit/'+ this.roomKey);

            this.mediaRecorder.ondataavailable = e => {
                videoWorker.postMessage(e.data);
            };

            this.mediaRecorder.start(3000);

            this.roomConfig.inLive = true
            this.roomSet.switchLive.remove();
            this.startContador();
            window.onbeforeunload = this.alertExitLive
            this.roomSet.isLive.style.display = 'inline-flex'
            if(!this.roomSet.keep.checked){
                if(this.roomSet.oneShot.checked){
                    setTimeout(() => {
                        this.trySetThumbnail()
                    }, 5000);
                }else{
                    setTimeout(() => {
                        this.trySetThumbnail()
                    }, 5000);
                    setInterval(() => {
                        this.trySetThumbnail()
                    }, 60000);
                }
            }

        }
    }
    updateContador(contador, startTime) {
        const currentTime = Date.now();
        const elapsedTime = currentTime - startTime;

        const hours = Math.floor(elapsedTime / 3600000);
        const minutes = Math.floor((elapsedTime % 3600000) / 60000);
        const seconds = Math.floor((elapsedTime % 60000) / 1000);

        const formattedTime = ` ${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;

        contador.textContent = formattedTime;

    };
    startContador() {
        const startTime = Date.now();
        const contador = this.roomSet.contador;
        this.updateContador(contador, startTime);
        setInterval(() => {
            this.updateContador(contador, startTime)
        }, 1000);
    }
    alertExitLive(e) {
        return 'Evento: ' + e.type;
    }
    trySetThumbnail() {

        const canvas = document.createElement('canvas');
        canvas.width = 1920;
        canvas.height = 1080;

        const ctx = canvas.getContext('2d');

        const video = document.createElement('video');
        this.roomStreams.main.merger.result;
        video.srcObject = this.roomStreams.main.merger.result;
        video.muted = true;

        video.onloadedmetadata = () => {
            
            video.play().catch();
            ctx.drawImage(video, 0, 0, canvas.width, canvas.height);
            const thumbnail = canvas.toDataURL('image/png');

            video.remove();

            const endpointThumb = wsurl + '/ws/live/thumb/' + this.roomKey;
            const socketThumb = new WebSocket(endpointThumb);
            socketThumb.addEventListener('open', event => {
                socketThumb.send(thumbnail);
            })
        }

    }
    closeRoom() {
        this.mediaRecorder.stop()
        location.href = '/end?key=' + this.roomKey
    }
    openRoom() {
        this.roomSet.startBtn.style.display = 'none'
        this.roomSet.patternGroup.style.display = 'none'
        this.goLive()
    }
    
    async getInfoFromParams() {
        this.roomKey = QueryString('mainkey')
        this.mentorKey = QueryString('mentorkey')
        this.publicKey = QueryString('publickey')

        this.updateRoomInfo()
    }
    updateRoomInfo() {
        
        this.roomSet.titulo.innerText = tituloLive
        this.roomConfig.tipoSala = 1
        this.roomSet.descricao.innerText = tituloLive
        
        if(!thumbnailLive){
            this.roomSet.keep.remove()
        }else{
            this.roomSet.keep.checked = true
        }
        this.roomSet.patternGroup.style.display = 'block'
    }
    renderLayout() {

        this.roomSet.startBtn.style.display = 'inline-flex'
    }
    isIndividual() {

        return this.roomConfig.tipoSala == 1
    }
}