import {
  LiveKitRoom,
  PreJoin,
  LocalUserChoices,
  useToken,
  VideoConference,
  formatChatMessageLinks,
} from '@livekit/components-react';

import { LogLevel, RoomOptions } from 'livekit-client';
import process from 'process';
import type { NextPage } from 'next';
import Head from 'next/head';
import { useRouter } from 'next/router';
import { useMemo, useState } from 'react';
import { getLiveKitURL } from '../../lib/server-utils';
import { DebugMode } from '../../lib/Debug';
import { useServerUrl } from '../../lib/client-utils';

const Home: NextPage = () => {
  const router = useRouter();
  const {
     name: roomName, 
     usr: username
     } = router.query;
 
    // const wsurl = process.env.NEXT_PUBLIC_ROOMWS_URL;  
    
    if(roomName){
      
      // let endpointTransmit = wsurl + '/transmit?id=' + roomName + '&usr='+ username;
      // if(roomName=="openroom-openroom"){
        
      //   endpointTransmit = wsurl + '/transmitToOpenRoom?usr='+ username;
     
      // }  
      // const ws = new WebSocket(endpointTransmit);
      // ws.onopen = () => {
      //   ws.send(
      //     ""
      //   );
      //   setInterval(() => {
      //     ws.send(
      //       ""
      //     );
      //   },30000)
      // }
    
    }
    
 return (
    <>
      <Head>
        <title>Mentoria Programador.TV</title>
        <link rel="icon" href="/favicon.ico" />
      </Head>

      <main>
        {
          <ActiveRoom
            roomName={roomName as string}
            userChoices={{
              username: username as string,
              videoEnabled: true,
              audioEnabled: true
            } as LocalUserChoices }
            // onLeave={() => {
            //   location.href = process.env.NEXT_PUBLIC_PLATFORM_URL + '/end/?key=' + roomName
            // }}
          ></ActiveRoom>
        }
      </main>
    </>
  );
};

export default Home;

type ActiveRoomProps = {
  userChoices: LocalUserChoices;
  roomName: string;
  region?: string;
  onLeave?: () => void;
};
const ActiveRoom = ({ roomName, userChoices, onLeave }: ActiveRoomProps) => {
  const token = useToken({
    tokenEndpoint: process.env.NEXT_PUBLIC_LK_TOKEN_ENDPOINT,
    roomName,
    userInfo: {
      identity: userChoices.username,
      name: userChoices.username,
    },
  });

  const router = useRouter();
  const { region } = router.query;

  const liveKitUrl = useServerUrl(region as string | undefined);

  const roomOptions = useMemo((): RoomOptions => {
    return {
      videoCaptureDefaults: {
        deviceId: userChoices.videoDeviceId ?? undefined,
      },
      audioCaptureDefaults: {
        deviceId: userChoices.audioDeviceId ?? undefined,
      },
      adaptiveStream: { pixelDensity: 'screen' },
      dynacast: true,
    };
  }, [userChoices]);

  return (
    <>
      {liveKitUrl && (
        <LiveKitRoom
          token={token}
          serverUrl={liveKitUrl}
          options={roomOptions}
          video={userChoices.videoEnabled}
          audio={userChoices.audioEnabled}
          onDisconnected={onLeave}
        >
          <VideoConference chatMessageFormatter={formatChatMessageLinks} />
          <DebugMode logLevel={LogLevel.info} />
        </LiveKitRoom>
      )}
    </>
  );
};
