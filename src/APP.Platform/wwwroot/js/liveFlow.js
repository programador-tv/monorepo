// function enviarTagsParaAPI (tags) {
//     fetch("?handler=ChatGPT&entrada=" + tags)
//         .then(function (response) {
//             document.querySelector('#TituloTitulo').classList.remove('skeleton-loading');
//             document.querySelector('#TituloDescricao').classList.remove('skeleton-loading');
//             document.querySelector('#LiveTitulo').placeholder = ""
//             document.querySelector('#LiveDescricao').placeholder = ""
//             if (!response.ok) {
//                 throw new Error("Erro ao enviar a solicitação POST.");
//             }
//             return response.json();
//         })
//         .then(data => {
//             document.querySelector('#LiveDescricao').value = data.descricao;
//             document.querySelector('#LiveTitulo').value = data.titulo;

//         })
// }
// function verificarSelecao () {
//     const selectedTags = [];
//     for (let i = 0; i < selectElement.options.length; i++) {
//         if (selectElement.options[i].selected) {
//             selectedTags.push(selectElement.options[i].value);
//         }
//     }
//     if (selectedTags.length > 0) {
//         document.getElementById('btnPrepararChatGPT').disabled = false;
//         document.getElementById('btnPrepararChatGPT').classList.remove('button-disabled');
//     } else {
//         document.getElementById('btnPrepararChatGPT').disabled = true;
//         document.getElementById('btnPrepararChatGPT').classList.add('button-disabled');
//     }
// }
// document.getElementById('btnPrepararChatGPT').addEventListener('click', function (event) {
//     event.preventDefault();

//     const selectedTags = [];
//     for (let i = 0; i < selectElement.options.length; i++) {
//         const option = selectElement.options[i];
//         if (option.selected) {
//             selectedTags.push(option.value);
//         }
//     }
//     if (selectedTags.length > 0) {
//         enviarTagsParaAPI(selectedTags.toString());

//         document.querySelector('#TituloTitulo').classList.add('skeleton-loading');
//         document.querySelector('#TituloDescricao').classList.add('skeleton-loading');
//         document.querySelector('#LiveDescricao').value = "";
//         document.querySelector('#LiveTitulo').value = "";
//         document.querySelector('#LiveTitulo').placeholder = "Carregando informações ..."
//         document.querySelector('#LiveDescricao').placeholder = "Carregando informações ..."
//     }

// });

// const selectElement = document.querySelector("#TagsForLive");
// document.addEventListener("DOMContentLoaded", function () {
//     createSelect2("#TagsForLive")
// });
