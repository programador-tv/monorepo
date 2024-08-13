document.addEventListener("DOMContentLoaded", function () {

const saveTimeForm = document.getElementById("saveTimeForm");

saveTimeForm.addEventListener("submit", (event) => {
  event.preventDefault();
  const selectElement = document.getElementById("TagsSelected");
  const selectedOptions = selectElement.selectedOptions;
  const hideEventModal = selectedOptions.length !== 0;

  SaveTime(saveTimeForm, hideEventModal);
});

function SaveTime(aspForm, _hideEventModal) {
  // if (hideEventModal) {
     $(".eventModal").modal("hide");
  // }

  // const descricaoValue = document.getElementById(
  //     "TimeSelectionDescricao"
  // ).value;

  const formData = new FormData(aspForm);

  // formData.append("ScheduleLiveForTimeSelection.Descricao", descricaoValue);

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
    .then( async(eventData) => {
      const content = eventData.content;
      if  (content !== undefined) {
          if (content.tipo == 0) {
            const timeSelectionIdForLive = content.id;
            content.backgroundColor = 'rgba(222, 164, 156, 0.45)';
           await  createLiveModal(content.id);
          } else {
            createTimeModal(content.id)
          }
          calendar.addEvent(content);
          let firstForm = content;
          alertTimeSelectionCreatedSucessfully(content.id);
      }
        // if (content.tipo == 1) {
        //     setTimeout(function () {
        //         $('#repeatModal').modal('show');
        //     }, 800);
        // }
    })
    .catch(function (error) {
        console.error(error);
    });
}

let pendentesContainer;

function createTimeModal(id) {
  if(!pendentesContainer){
    pendentesContainer = document.querySelector("#free-time-pendentes-card");
  }

  const semAbertos = document.querySelector("#semAbertos");
  if(semAbertos){
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
