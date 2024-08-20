document.addEventListener("DOMContentLoaded", function () {

const saveScheduleLiveForm = document.getElementById("scheduleLiveForm");

saveScheduleLiveForm.addEventListener("submit", (event) => {
  event.preventDefault();
  SaveTime(saveScheduleLiveForm);
});

  async function createLiveModal(id) {
      const response = await fetch("?handler=AfterLoadLivePreview")
      if (!response.ok) {
          throw new Error("Erro ao enviar a solicitação GET.");
      }
      const data = await response.json();

      const content = document.querySelector("#savedVideos")

      if (data.preview) {
          document.querySelectorAll(".schedule").forEach(e => e.remove())
          content.innerHTML = data.preview + content.innerHTML;
          $("#destaque-tittle").show();
      }

      const modalContainer = document.querySelector("#eventModals");
      fetch("/ScheduleActions?handler=PartialLiveModal&timeSelectionId=" + id)
          .then((response) => response.text())
          .then((data) => {
              modalContainer.innerHTML += data;
          })
          .catch((error) => {
              console.error("Error:", error);
          });

  }

function SaveTime (aspForm, _hideEventModal) {
  $(".eventModal").modal("hide");
  let tsId;
  const descricaoValue = document.getElementById(
      "LiveDescricao-Sched"
  ).value;

  const formData = new FormData(aspForm);
    formData.append("ScheduleLiveForTimeSelection.Descricao", descricaoValue);

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
      if (content !== undefined) {
        tsId = content.id;
        content.backgroundColor = 'rgba(222, 164, 156, 0.45)';

          await createLiveModal(tsId);

        calendar.addEvent(content);
        // deve abrir um alert para pessoa decidir se quer ver o modal
        Swal.fire({
          title: 'Live criada com sucesso !',
          icon: 'success',
          showCancelButton: true,
          showConfirmButton: true,
          cancelButtonText: "Fechar",
          confirmButtonText: "Ver evento",
          showLoaderOnConfirm: true,
          preConfirm: () => {
            openModal(tsId);

          }
        });
       }  
    }).catch(function (error) {
      console.error(error);
    });
}

});


