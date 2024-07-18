const config = {
  start: false,
  socketTransmit: null,
}

const bufferQueue = [];
const reader = new FileReader();

reader.onload = function(e) {
  const rawData = e.target.result;
  config.socketTransmit.send(rawData);
};

onmessage = function(e) {
  if (!config.start && !config.socketTransmit) {
    config.start = true;
    const endpointTransmit = e.data;
    config.socketTransmit = new WebSocket(endpointTransmit);
    return;
  }

  const buffer = e.data;
  bufferQueue.push(buffer);

  const file = new File(bufferQueue, 'filename', { type: 'video/webm' });
  
  reader.readAsArrayBuffer(file);
  bufferQueue.length = 0; 
  
};