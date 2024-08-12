function enviarTagsParaAPI (tags) {
    console.log("Enviando tags para API: " + tags);
    fetch("?handler=ChatGPT&entrada=" + tags)
        .then(function (response) {
            document.querySelector('#Titulo-Sched').classList.remove('skeleton-loading');
            document.querySelector('#TituloDescricao-Sched').classList.remove('skeleton-loading');
            document.querySelector('#LiveTitulo-Sched').placeholder = ""
            document.querySelector('#LiveDescricao-Sched').placeholder = ""
            if (!response.ok) {
                throw new Error("Erro ao enviar a solicitação POST.");
            }
            return response.json();
        })
        .then(data => {
            document.querySelector('#LiveDescricao-Sched').value = data.descricao;
            document.querySelector('#LiveTitulo-Sched').value = data.titulo;

        })
}

function verificarSelecaoLiveSchedule () {
    let selectedTags = [];
    for (var i = 0; i < selectElementScheduleLive.options.length; i++) {
        if (selectElementScheduleLive.options[i].selected) {
            selectedTags.push(selectElementScheduleLive.options[i].value);
        }
    }
    if (selectedTags.length > 0) {
        document.getElementById('btnPrepararChatGPT-Sched').disabled = false;
        document.getElementById('btnPrepararChatGPT-Sched').classList.remove('button-disabled');
    } else {
        document.getElementById('btnPrepararChatGPT-Sched').disabled = true;
        document.getElementById('btnPrepararChatGPT-Sched').classList.add('button-disabled');
    }
}

document.getElementById('btnPrepararChatGPT-Sched').addEventListener('click', function (event) {
    event.preventDefault();

    let selectedTags = [];
    for (var i = 0; i < selectElementScheduleLive.options.length; i++) {
        var option = selectElementScheduleLive.options[i];
        if (option.selected) {
            selectedTags.push(option.value);
        }
    }
    if (selectedTags.length > 0) {
        enviarTagsParaAPI(selectedTags.toString());

        document.querySelector('#Titulo-Sched').classList.add('skeleton-loading');
        document.querySelector('#TituloDescricao-Sched').classList.add('skeleton-loading');
        document.querySelector('#LiveDescricao-Sched').value = "";
        document.querySelector('#LiveTitulo-Sched').value = "";
        document.querySelector('#LiveTitulo-Sched').placeholder = "Carregando informações ..."
        document.querySelector('#LiveDescricao-Sched').placeholder = "Carregando informações ..."
    }

});

const selectElementScheduleLive = document.querySelector("#TagsForScheduleLive");
document.addEventListener("DOMContentLoaded", function () {
    createSelect2("#TagsForScheduleLive")
});