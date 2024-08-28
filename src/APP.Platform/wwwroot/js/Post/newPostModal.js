document.addEventListener('DOMContentLoaded', function () {
    console.log("Testando 123")

    btnSubmit = document.getElementById("submitNewPost");
    errorSmall = document.querySelector("#newPostFromSite #error");

    errorSmall.style.display = "block";

    function validarFormulario() {
        var link = document.getElementById('post-link').value;
        var dominiosPermitidos = [
            'linkedin.com',
            'x.com',
            'dev.to',
            'tabnews.com.br',
            'medium'
        ];

        // Remove https e www
        var urlSemProtocolo = link.replace(/^https?:\/\//, '').replace(/^www\./, '');

        // Verifica se o domínio permitido esta no comeco
        var valido = dominiosPermitidos.some(function (dominio) {
            return urlSemProtocolo.startsWith(dominio);
        });

        if (!link) {
            atualizarErro('Preencha com um link válido');
        }
        if (!valido) {
            atualizarErro('O link deve ser de dos seguintes sites: linkedin.com, x.com, dev.to, tabnews.com.br, medium');
            return false;
        }


        atualizarErro(''); // Limpar mensagem de erro se válido
        return true;
    }
    function atualizarErro(mensagem) {
        errorSmall.innerHTML += mensagem;
        errorSmall.style.display = mensagem ? 'block' : 'none';
    }

    btnSubmit.addEventListener("click", () => {
        validarFormulario();
    });
});