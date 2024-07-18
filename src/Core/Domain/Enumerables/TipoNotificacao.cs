namespace Domain.Enumerables
{
    public enum TipoNotificacao
    {
        // FINALIZOU CADASTRO
        FinalizouCadastro = 0,

        // MEUS VIDEOS
        ComentarioNoMeuVideo = 1,

        // MENTORIA MENTOR
        NovoInteressadoMentoria = 2,
        MentorMentoriaProxima = 3,
        MentorConsideracaoFinalPendente = 4,

        MentorCancelaMentoria = 9,

        // MENTORIA ALUNO
        AlunoAceitoNaMentoria = 5, //ok
        AlunoMentoriaProxima = 6,
        AlunoConsideracaoFinalPendente = 7,
        AlunoCancelaMentoria = 8,
        PreviewCriada = 11,

        //LIVE
        EspectadorLiveProxima = 10,
    }
}
