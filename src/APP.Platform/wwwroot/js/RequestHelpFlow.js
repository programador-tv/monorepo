document.addEventListener("DOMContentLoaded", function () {

const saveTimeRequestHelpForm = document.getElementById("saveTimeFormHelp");

saveTimeRequestHelpForm.addEventListener("submit", (event) => {
    event.preventDefault();
    const selectElement = document.getElementById("TagsSelectedHelp");
    const selectedOptions = selectElement.selectedOptions;
    const hideEventModal = selectedOptions.length !== 0;

    SaveTime(saveTimeRequestHelpForm, hideEventModal);
});

function SaveTime(aspForm, _hideEventModal) {
    $(".eventModal").modal("hide");
    const formData = new FormData(aspForm);

    const url = "/ScheduleActions?handler=SaveTime";
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
            const content = data.content;
            createTimeModal(content.id);
            calendar.addEvent(content);
            let firstForm = content;
            alertTimeSelectionCreatedSucessfully(content.id);
        })
        .catch(function (error) {
            console.error(error);
        });
}

let pendentesContainer;

function createTimeModal(id) {
    /*codigo de referencia
    */

    if (!pendentesContainer) {
        pendentesContainer = document.querySelector("#free-time-pendentes-card");
    }
    const semAbertos = document.querySelector("#semAbertos");
    if (semAbertos) {
        semAbertos.remove();
    }

    fetch("/ScheduleActions?handler=PartialFreeTimeModal&timeSelectionId=" + id)
       .then((response) => response.text())
       .then((data) => {
           pendentesContainer.innerHTML += data;
       })
       .catch((error) => {
           console.error("Error:", error);
       });

    const modalContainer = document.querySelector("#eventModals");

    fetch("/ScheduleActions?handler=PartialFreeTimePanel&timeSelectionId=" + id)
       .then((response) => response.text())
       .then((data) => {
           modalContainer.innerHTML += data;
       })
       .catch((error) => {
           console.error("Error:", error);
    });
}});