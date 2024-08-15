document.addEventListener("DOMContentLoaded", function () {

    const saveScheduleLiveForm = document.getElementById("scheduleLiveFormOnChannel");

    saveScheduleLiveForm.addEventListener("submit", (event) => {
      event.preventDefault();
      SaveTime(saveScheduleLiveForm);
    });

    function SaveTime (aspForm, _hideEventModal) {
      $(".eventModal").modal("hide");

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
        .then((eventData) => {
          const content = eventData.content;
          if (content !== undefined) {
            // const timeSelectionIdForLive = content.id;
            // content.backgroundColor = 'rgba(222, 164, 156, 0.45)';
            // // createLiveModal();
            // calendar.addEvent(content);
            // // firstForm = content;
            setTimeout(() => {
              location.reload();
            }, 2000);
          }
        })
        .catch(function (error) {
          console.error(error);
        });
    }

    // function createLiveModal () {
    //   fetch("?handler=AfterLoadLivePreview")
    //     .then(function (response) {
    //       if (!response.ok) {
    //         throw new Error("Erro ao enviar a solicitação GET.");
    //       }
    //       return response.json();
    //     })
    //     .then(async function (data) {
    //       var livePreview = document.querySelector("#destaques")

    //       if (data.preview) {
    //         livePreview.innerHTML = data.preview;
    //         $("#destaque-tittle").show();
    //       }
    //     })
    // }
    });


