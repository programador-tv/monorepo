import type { NextPage } from 'next';
import { useRouter } from 'next/router';
import { useEffect } from 'react';
import styles from '../styles/Home.module.css';
declare var ga: UniversalAnalytics.ga;

const Home: NextPage = () => {
  const router = useRouter();
  const {
    name: roomName, 
    usr: username
    } = router.query;

  const startMeeting = () => {
    router.push(`/rooms/${roomName}/?usr=${username}`);
  };
  
  useEffect(() => {
    const gaScript = document.createElement('script');
    gaScript.setAttribute('src', 'https://www.google-analytics.com/analytics.js');
    gaScript.setAttribute('async', '');
    document.body.appendChild(gaScript);
    gaScript.onload = function() {
      ga('create', 'G-17S4EYBPS2', 'auto');
      ga('send', 'pageview');
    }
  })
  return (
    <>
     <nav className="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
            <div className="container">
                <a href="https://programador.tv" target="_blank" rel="noreferrer">
                    <img width="220" src="https://programador.tv/logo.png"  alt=""/>
                </a>
            </div>
        </nav>
      <main className={styles.main}>
        <div className="header">
          <ul>
            <p><b>Cuidado com o que compartilha.</b> Senhas e arquivos pessoais devem ser previamente ocultados</p>
            <p><b>Seja gentil.</b> pergunte como o aluno está, conheça brevemente sua historia antes de ajuda-lo com algo especifico</p>
            <p><b> Esteja vestido de forma adequada</b></p>
        </ul>
        <br></br>
        <br></br>
        <br></br>
        <br></br>
        </div>
        <button
          style={{ fontSize: '1.25rem', paddingInline: '1.25rem' }}
          className="lk-button"
          onClick={startMeeting}
        >
          Entendi
        </button>
      </main>
      <footer>
       Programador.TV
      </footer>
    </>
  );
};

export default Home;
