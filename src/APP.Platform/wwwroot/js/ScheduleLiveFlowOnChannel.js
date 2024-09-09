document.addEventListener("DOMContentLoaded", function () {
  let form = document.getElementById("scheduleLiveFormOnChannel");

  let dateFormSL = document.getElementById("dateTimeSL");
  const today = new Date();
  const formattedDate = today.toISOString().split("T")[0];
  dateFormSL.value = formattedDate;

  let hoursStartSL = document.getElementById("hoursStartSL");
  hoursStartSL.value = new Date().getHours() + 1;
  let minutesStartSL = document.getElementById("minutesStartSL");
  let isAmStartSL = document.getElementById("ampmStartSL");
  isAmStartSL.value = hoursStartSL.value >= 12 ? "pm" : "am";

  let inicioHorarioSL = `${
    hoursStartSL.value < 10 ? "0" + hoursStartSL.value : hoursStartSL.value
  }:${
    minutesStartSL.value < 10
      ? "0" + minutesStartSL.value
      : minutesStartSL.value
  }`;

  let hoursEndSL = document.getElementById("hoursEndSL");
  hoursEndSL.value = new Date().getHours() + 2;
  let minutesEndSL = document.getElementById("minutesEndSL");
  let isAmEndSL = document.getElementById("ampmEndSL");
  isAmEndSL.value =
    hoursEndSL.value >= 12 && minutesEndSL.value > 0 ? "pm" : "am";

  let fimHorarioSL = `${
    hoursEndSL.value < 10 ? "0" + hoursEndSL.value : hoursEndSL.value
  }:${minutesEndSL.value < 10 ? "0" + minutesEndSL.value : minutesEndSL.value}`;

  const saveScheduleLiveForm = document.getElementById(
    "scheduleLiveFormOnChannel"
  );

  function SaveTime(aspForm, _hideEventModal) {
    $("#liveModal").modal("hide");

    const descricaoValue = document.getElementById("LiveDescricao-Sched").value;

    const formData = new FormData(aspForm);
    formData.append("ScheduleLiveForTimeSelection.Descricao", descricaoValue);

    formData.set(
      "ScheduleTimeSelection.StartTime",
      `${dateFormSL.value}T${inicioHorarioSL}-03:00`
    );
    formData.set(
      "ScheduleTimeSelection.EndTime",
      `${dateFormSL.value}T${fimHorarioSL}-03:00`
    );

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
      .then(async (eventData) => {
        if (
          location.href === "https://programador.tv" ||
          location.href === "https://programador.tv/index" ||
          location.href === "https://localhost:7150/index" ||
          location.href === "https://localhost:7150/"
        ) {
          const content = eventData.content;
          if (content !== undefined) {
            tsId = content.id;
            content.backgroundColor = "rgba(222, 164, 156, 0.45)";

            await createLiveModal(tsId);

            calendar.addEvent(content);
            // deve abrir um alert para pessoa decidir se quer ver o modal
            Swal.fire({
              title: "Live criada com sucesso !",
              icon: "success",
              showCancelButton: true,
              showConfirmButton: true,
              cancelButtonText: "Fechar",
              confirmButtonText: "Ver evento",
              showLoaderOnConfirm: true,
              preConfirm: () => {
                openModal(tsId);
              },
            });
          }
        } else {
          const content = eventData.content;
          if (content !== undefined) {
            setTimeout(() => {
              location.reload();
            }, 1000);
          }
        }
      })
      .catch(function (error) {
        console.error(error);
      });
  }

  async function createLiveModal(id) {
    const response = await fetch("?handler=AfterLoadLivePreview");
    if (!response.ok) {
      throw new Error("Erro ao enviar a solicitação GET.");
    }
    const data = await response.json();

    const content = document.querySelector("#savedVideos");

    const contentLive = document.querySelector("#myLives");

    if (data.preview) {
      document.querySelectorAll(".schedule").forEach((e) => e.remove());
      content.innerHTML = data.preview + content.innerHTML;
      $("#destaque-tittle").show();
    }

    fetch("/ScheduleActions?handler=PartialLivePanel&timeSelectionId=" + id)
      .then((response) => response.text())
      .then((data) => {
        contentLive.innerHTML += data;
      })
      .catch((error) => {
        console.error("Error:", error);
      });

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

  hoursStartSL.addEventListener("change", updateTimeStartSL);
  minutesStartSL.addEventListener("change", updateTimeStartSL);
  isAmStartSL.addEventListener("change", updateTimeStartSL);
  hoursEndSL.addEventListener("change", updateTimeEndSL);
  minutesEndSL.addEventListener("change", updateTimeEndSL);
  isAmEndSL.addEventListener("change", updateTimeEndSL);

  function updateTimeStartSL() {
    let hoursStartInt = parseInt(hoursStartSL.value, 10);
    if (isAmStartSL.value === "pm" && hoursStartInt !== 12) {
      hoursStartInt += 12;
    } else if (isAmStartSL.value === "am" && hoursStartInt === 12) {
      hoursStartInt = 0; // Ajuste para 12 AM ser 00
    }

    let minutesInt = parseInt(minutesStartSL.value, 10);

    inicioHorarioSL = `${
      hoursStartInt < 10 ? "0" + hoursStartInt : hoursStartInt
    }:${minutesInt < 10 ? "0" + minutesInt : minutesInt}`;
  }

  function updateTimeEndSL() {
    let hoursEndInt = parseInt(hoursEndSL.value, 10);
    if (isAmEndSL.value === "pm" && hoursEndInt !== 12) {
      hoursEndInt += 12;
    } else if (isAmEndSL.value === "am" && hoursEndInt === 12) {
      hoursEndInt = 0; // Ajuste para 12 AM ser 00
    }

    let minutesEndInt = parseInt(minutesEndSL.value, 10);

    fimHorarioSL = `${hoursEndInt < 10 ? "0" + hoursEndInt : hoursEndInt}:${
      minutesEndInt < 10 ? "0" + minutesEndInt : minutesEndInt
    }`;
  }

  form.addEventListener("submit", (event) => {
    event.preventDefault();
    const selectElement = document.getElementById(
      "TagsForScheduleLiveOnChannel"
    );
    const selectedOptions = selectElement.selectedOptions;
    const hideEventModal = selectedOptions.length !== 0;

    SaveTime(form, hideEventModal);
  });
});
