export default class {
    merger;  
    config = {
      video: new MediaStream(),
      audio: new MediaStream(),
      screen: new MediaStream(),
      participant: new MediaStream()
    }
  
    
    constructor(){
      this.merger = new VideoStreamMerger({
          width: 1920,
          height: 1080,
          audioContext: null, // Supply an external AudioContext (for audio effects)
        })
      }
      addNewParticipant(participant){
  
        //this.merger.removeStream(this.config.participant)
  
        this.config.participant = participant
  
         this.merger.addStream(participant, {
          x: this.merger.width - 183,
          y: this.merger.height - 220,
          width: 173,
          height: 100,
          mute: false
        })
        
      }
  
      removeParticipant(){
        this.merger.removeStream(this.config.participant)
      }
      removeAudio(){
        this.merger.removeStream(this.config.audio)
      }
      removeVideo(){
        this.merger.removeStream(this.config.video)
  
      }
      removeScreen(){
        this.merger.removeStream(this.config.screen)
  
      }
      addNewVideo(video){
  
        this.merger.removeStream(this.config.video)
  
        this.config.video = video
        const audioVideo = new MediaStream([
          ...this.config.video.getVideoTracks(),
          ...this.config.audio.getAudioTracks(),
         ])
         this.merger.addStream(audioVideo, {
          x: this.merger.width - 183,
          y: this.merger.height - 110,
          width: 173,
          height: 100,
          mute: false
        })
        
      }
  
      addNewAudio(audio){
        this.merger.removeStream(this.config.audio)
  
        this.config.audio = audio
        const audioVideo = new MediaStream([
          ...this.config.video.getVideoTracks(),
          ...this.config.audio.getAudioTracks(),
         ])
         this.merger.addStream(audioVideo, {
          x: this.merger.width - 183,
          y: this.merger.height - 110,
          width: 173,
          height: 100,
          mute: false
        })
      }
      addNewScreen(screen){
        this.merger.removeStream(this.config.screen)
  
        this.config.screen = screen
        this.merger.addStream(screen, {
          x: 0, // position of the topleft corner
          y: 0,
          width: this.merger.width,
          height: this.merger.height,
          mute: false // we don't want sound from the screen (if there is any)
        })
        this?.addNewVideo?.(this.config.video)
        if(this?.config?.participant?.active){
          this.addNewParticipant(this.config.participant)
        }
      
      }
      mergeScreenAndVideoAndAudio({screen,video,audio}){
        this.config = {screen,video,audio}
         const audioVideo = new MediaStream([
          ...video.getVideoTracks(),
          ...audio.getAudioTracks(),
         ])
        
        
         
          this.merger.addStream(screen, {
            x: 0, // position of the topleft corner
            y: 0,
            width: this.merger.width,
            height: this.merger.height,
            mute: false // we don't want sound from the screen (if there is any)
          })
        
         this.merger.addStream(audioVideo, {
            x: this.merger.width - 183,
            y: this.merger.height - 110,
            width: 173,
            height: 100,
            mute: false
          })
        
       
        this.merger.start()
        return this.merger.result;
      }
  }