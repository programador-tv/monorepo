document.addEventListener("DOMContentLoaded", function () {
  let dateFormRH = document.getElementById("dateTimeRH");
  const today = new Date();
  const formattedDate = today.toISOString().split("T")[0];
  dateFormRH.value = formattedDate;
  let submitFormRh = document.getElementById("saveTimeFormHelp");

  let hoursStartRH = document.getElementById("hoursStartRH");
  hoursStartRH.value = new Date().getHours() + 1;
  let minutesStartRH = document.getElementById("minutesStartRH");
  let isAmStartRH = document.getElementById("ampmStartRH");
  isAmStartRH.value = hoursStartRH.value >= 12 ? "pm" : "am";

  let inicioHorarioRH = `${
    hoursStartRH.value < 10 ? "0" + hoursStartRH.value : hoursStartRH.value
  }:${
    minutesStartRH.value < 10
      ? "0" + minutesStartRH.value
      : minutesStartRH.value
  }`;

  let hoursEndRH = document.getElementById("hoursEndRH");
  hoursEndRH.value = new Date().getHours() + 2;
  let minutesEndRH = document.getElementById("minutesEndRH");
  let isAmEndRH = document.getElementById("ampmEndRH");
  isAmEndRH.value = hoursEndRH.value >= 12 ? "pm" : "am";

  let fimHorarioRH = `${
    hoursEndRH.value < 10 ? "0" + hoursEndRH.value : hoursEndRH.value
  }:${minutesEndRH.value < 10 ? "0" + minutesEndRH.value : minutesEndRH.value}`;

  let tituloResumoRH = document.getElementById("tituloResumoRH");
  let horariosResumoRH = document.getElementById("horarios-resumoRH");
  let diaResumoRH = document.getElementById("dia-resumoRH");

  let modalTagsRH = document.getElementById("modal-tagsRH");

  const saveTimeFormRH = document.getElementById("saveTimeFormHelp");

  function SaveTime(aspForm, _hideEventModal) {
    $(".eventModal").modal("hide");
    const formData = new FormData(aspForm);

    formData.set(
      "ScheduleTimeSelection.StartTime",
      `${dateFormRH.value}T${inicioHorarioRH}-03:00`
    );
    formData.set(
      "ScheduleTimeSelection.EndTime",
      `${dateFormRH.value}T${fimHorarioRH}-03:00`
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
      .then((data) => {
        const content = data.content;
        createTimeModal(content.id);
        calendar.addEvent(content);
        firstForm = content;
        alertTimeSelectionCreatedSucessfully(content.id);
      })
      .catch(function (error) {
        console.error(error);
      });
  }

  submitFormRh.addEventListener("submit", (event) => {
    event.preventDefault();
    const selectElementRH = document.getElementById("TagsSelectedHelp");
    const selectedOptionsRH = selectElement.selectedOptions;
    const hideEventModalRH = selectedOptionsRH.length !== 0;

    //limpar os valores

    SaveTime(saveTimeFormRH, hideEventModalRH);
  });

  const switchStep = (currentStep, nextStep) => {
    $(currentStep).removeClass("active");
    $(nextStep).addClass("active");
  };

  // Passa da etapa 1 para a etapa 2
  $("#btnRequestHelp1").click(() => {
    switchStep(".body-requestHelp-1", ".body-requestHelp-2");
  });

  // Passa da etapa 2 para a etapa 3
  $("#btnRequestHelp2").click(() => {
    switchStep(".body-requestHelp-2", ".body-requestHelp-3");
  });

  // Passa da etapa 2 para a etapa 3
  $("#btnRequestHelp3").click(() => {
    switchStep(".body-requestHelp-3", ".body-requestHelp-4");

    let tituloRH = document.getElementById(
      "TimeSelectionMentoriaTituloRH"
    ).value;
    tituloResumoRH.innerText = tituloRH;
    horariosResumoRH.innerText = inicioHorarioRH + " - " + fimHorarioRH;

    const date = new Date(dateFormRH.value);
    date.setDate(date.getDate() + 1);

    const day = date.getDate();
    const dayOfWeek = date.getDay();
    const daysOfWeek = [
      "Domingo",
      "Segunda-feira",
      "Terça-feira",
      "Quarta-feira",
      "Quinta-feira",
      "Sexta-feira",
      "Sábado",
    ];
    const dayName = daysOfWeek[dayOfWeek];
    diaResumoRH.innerText = day.toString() + " " + dayName;

    const ulElement = document.getElementById(
      "select2-TagsSelectedHelp-container"
    );
    const liElements = ulElement.querySelectorAll("li");

    const mainTag = document.createElement("span");
    mainTag.className = "modal-tag-main";
    mainTag.textContent = "Ajuda";
    modalTagsRH.appendChild(mainTag);

    liElements.forEach((li) => {
      const span = document.createElement("span");
      span.className = "modal-tag";
      span.textContent = li.textContent.substring(1).trim();
      modalTagsRH.appendChild(span);
    });
  });

  hoursStartRH.addEventListener("change", updateTimeStartRH);
  minutesStartRH.addEventListener("change", updateTimeStartRH);
  isAmStartRH.addEventListener("change", updateTimeStartRH);
  hoursEndRH.addEventListener("change", updateTimeEndRH);
  minutesEndRH.addEventListener("change", updateTimeEndRH);
  isAmEndRH.addEventListener("change", updateTimeEndRH);

  function updateTimeStartRH() {
    let hoursStartInt = parseInt(hoursStartRH.value, 10);
    if (isAmStartRH.value === "pm" && hoursStartInt !== 12) {
      hoursStartInt += 12;
    } else if (isAmStartRH.value === "AM" && hoursStartInt === 12) {
      hoursStartInt = 0; // Ajuste para 12 AM ser 00
    }

    let minutesInt = parseInt(minutesStartRH.value, 10);

    inicioHorarioRH = `${
      hoursStartInt < 10 ? "0" + hoursStartInt : hoursStartInt
    }:${minutesInt < 10 ? "0" + minutesInt : minutesInt}`;
  }

  function updateTimeEndRH() {
    let hoursEndInt = parseInt(hoursEndRH.value, 10);
    if (isAmEndRH.value === "pm" && hoursEndInt !== 12) {
      hoursEndInt += 12;
    } else if (isAmEndRH.value === "AM" && hoursEndInt === 12) {
      hoursEndInt = 0; // Ajuste para 12 AM ser 00
    }

    let minutesEndInt = parseInt(minutesEndRH.value, 10);

    fimHorarioRH = `${hoursEndInt < 10 ? "0" + hoursEndInt : hoursEndInt}:${
      minutesEndInt < 10 ? "0" + minutesEndInt : minutesEndInt
    }`;
  }

  // Passa da etapa 3 para a etapa 2
  $("#btnRequestHBack1").click(() => {
    switchStep(".body-requestHelp-2", ".body-requestHelp-1");
  });

  // Passa da etapa 2 para a etapa 1
  $("#btnRequestHBack2").click(() => {
    switchStep(".body-requestHelp-3", ".body-requestHelp-2");
    modalTagsRH.innerHTML = "";
  });

  $("#btnRequestHBack3").click(() => {
    switchStep(".body-requestHelp-4", ".body-requestHelp-3");
  });

  // Se qualquer modal fechar, o display deve ser do body 1
  $("#eventModalRequestHelp").on("hidden.bs.modal", () => {
    setTimeout(() => {
      $(".modal-selectRH").removeClass("active");
      $(".body-requestHelp-1").addClass("active");
    }, 100);
  });

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
  }
});
