export default {
    mediaConstraints : {
        optional: [],
        mandatory: {
          OfferToReceiveAudio: true,
          OfferToReceiveVideo: true
        },
        OfferToReceiveAudio: true,
        OfferToReceiveVideo: true
      },
    connection : {
        iceServers: [
          {
            urls: [
              'stun:stun.l.google.com:19302',
              'stun:stun1.l.google.com:19302',
              'stun:stun2.l.google.com:19302',
              'stun:stun.l.google.com:19302?transport=udp',
              'stun:stun.cloudflare.com:3478'
            ],
          },
        ],
        iceCandidatePoolSize: 10,
    }
}