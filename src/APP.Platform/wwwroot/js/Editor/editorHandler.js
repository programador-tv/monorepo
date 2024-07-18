const messageDiv = document.querySelector("#editor-message");
const editorForm = document.querySelector("#video-editor-form");
const key = document.querySelector("#btn-save").getAttribute('data-value');

editorForm.addEventListener("submit", (event) => {
    event.preventDefault();

    const selectElement = document.getElementById("TagsSelected");
    const selectedOptions = selectElement.selectedOptions;
    const hideEventModal = selectedOptions.length !== 0;
    if (hideEventModal) {
        editVideo(editorForm);
    }
    else {
        Swal.fire({

            title: "Por favor, preencha todos os campos não opcionais.",
            icon: "error"
          });
    }
});

function editVideo(aspForm) {
    const previewTitle = document.querySelector("#previewTitle");
    const previewThumbnail = document.querySelector("#previewThumbnail");
    const formData = new FormData(aspForm);
    const url = "?key=" + key;
    const options = {
        method: "POST",
        body: formData,
    };

    fetch(url, options)
        .then((response) => {
            if (!response.ok) {
                throw new Error(`Erro: ${response.status}`);
            }
            return response.json();
        })
        .then((data) => {
            previewTitle.innerHTML = `${data.titulo}`;
            previewThumbnail.src = data.thumbnail;

            Swal.fire({

                title: "Alterações realizadas com sucesso.",
                icon: "sucess"
              });
        })
        .catch(() => {
            Swal.fire({

                title: "Ocorreu um erro ao tentar salvar as alterações. Por favor tente mais tarde.",
                icon: "error"
              });
        });
}