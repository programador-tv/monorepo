function enviarTagsParaAPI (tags) {
    console.log("Enviando tags para API: " + tags);
    fetch("?handler=ChatGPT&entrada=" + tags)
        .then(function (response) {
            document.querySelector('#Titulo-Sched').classList.remove('skeleton-loading');
            document.querySelector('#TituloDescricao-Sched').classList.remove('skeleton-loading');
            document.querySelector('#LiveTitulo-SchedOnChannel').placeholder = ""
            document.querySelector('#LiveDescricao-SchedOnChannel').placeholder = ""
            if (!response.ok) {
                throw new Error("Erro ao enviar a solicitação POST.");
            }
            return response.json();
        })
        .then(data => {
            document.querySelector('#LiveDescricao-SchedOnChannel').value = data.descricao;
            document.querySelector('#LiveTitulo-SchedOnChannel').value = data.titulo;

        })
}

function verificarSelecaoLiveScheduleOnChannel () {
    let selectedTags = [];
    for (let i = 0; i < selectElementScheduleLiveOnChannel.options.length; i++) {
        if (selectElementScheduleLiveOnChannel.options[i].selected) {
            selectedTags.push(selectElementScheduleLiveOnChannel.options[i].value);
        }
    }
    if (selectedTags.length > 0) {
        document.getElementById('btnPrepararChatGPT-SchedOnChannel').disabled = false;
        document.getElementById('btnPrepararChatGPT-SchedOnChannel').classList.remove('button-disabled');
    } else {
        document.getElementById('btnPrepararChatGPT-SchedOnChannel').disabled = true;
        document.getElementById('btnPrepararChatGPT-SchedOnChannel').classList.add('button-disabled');
    }
}

document.getElementById('btnPrepararChatGPT-SchedOnChannel').addEventListener('click', function (event) {
    event.preventDefault();

    let selectedTags = [];
    for (let i = 0; i < selectElementScheduleLiveOnChannel.options.length; i++) {
        const option = selectElementScheduleLiveOnChannel.options[i];
        if (option.selected) {
            selectedTags.push(option.value);
        }
    }
    if (selectedTags.length > 0) {
        enviarTagsParaAPI(selectedTags.toString());

        document.querySelector('#Titulo-Sched').classList.add('skeleton-loading');
        document.querySelector('#TituloDescricao-Sched').classList.add('skeleton-loading');
        document.querySelector('#LiveDescricao-SchedOnChannel').value = "";
        document.querySelector('#LiveTitulo-SchedOnChannel').value = "";
        document.querySelector('#LiveTitulo-SchedOnChannel').placeholder = "Carregando informações ..."
        document.querySelector('#LiveDescricao-SchedOnChannel').placeholder = "Carregando informações ..."
    }

});

const selectElementScheduleLiveOnChannel = document.querySelector("#TagsForScheduleLiveOnChannel");
document.addEventListener("DOMContentLoaded", function () {
    createSelect2("#TagsForScheduleLive")
});