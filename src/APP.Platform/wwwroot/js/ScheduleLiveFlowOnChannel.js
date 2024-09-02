document.addEventListener("DOMContentLoaded", function () {
  let dateFormSL = document.getElementById("dateTimeSL");

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
  isAmEndSL.value = hoursEndSL.value >= 12 ? "pm" : "am";

  let fimHorarioSL = `${
    hoursEndSL.value < 10 ? "0" + hoursEndSL.value : hoursEndSL.value
  }:${minutesEndSL.value < 10 ? "0" + minutesEndSL.value : minutesEndSL.value}`;

  const saveScheduleLiveForm = document.getElementById(
    "scheduleLiveFormOnChannel"
  );

  console.log(saveScheduleLiveForm);

  saveScheduleLiveForm.addEventListener("submit", (event) => {
    event.preventDefault();
    SaveTime(saveScheduleLiveForm);
  });

  function SaveTime(aspForm, _hideEventModal) {
    $(".eventModal").modal("hide");

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
    } else if (isAmStartSL.value === "AM" && hoursStartInt === 12) {
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
    } else if (isAmEndSL.value === "AM" && hoursEndInt === 12) {
      hoursEndInt = 0; // Ajuste para 12 AM ser 00
    }

    let minutesEndInt = parseInt(minutesEndSL.value, 10);

    fimHorarioSL = `${hoursEndInt < 10 ? "0" + hoursEndInt : hoursEndInt}:${
      minutesEndInt < 10 ? "0" + minutesEndInt : minutesEndInt
    }`;
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
  //       const livePreview = document.querySelector("#destaques")

  //       if (data.preview) {
  //         livePreview.innerHTML = data.preview;
  //         $("#destaque-tittle").show();
  //       }
  //     })
  // }
});
