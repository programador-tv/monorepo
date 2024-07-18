import '../styles/globals.css';
import type { AppProps } from 'next/app';
import '@livekit/components-styles';
import '@livekit/components-styles/prefabs';
import { DefaultSeo } from 'next-seo';

function MyApp({ Component, pageProps }: AppProps) {
  
  return (
    <>
      <DefaultSeo
        title="Programador {TV} | Sala"
        titleTemplate="%s"
        defaultTitle="Programador {TV} | Sala"
        additionalLinkTags={[
          {
            rel: 'icon',
            href: '/favicon.ico',
          },
        ]}
      />
      <Component {...pageProps} />
    </>
  );
}

export default MyApp;
