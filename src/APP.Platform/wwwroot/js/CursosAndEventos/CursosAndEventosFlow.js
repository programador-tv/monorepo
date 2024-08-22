document.addEventListener("DOMContentLoaded", function () {
  let dateFormCE = document.getElementById("dateTimeCE");
  const today = new Date();
  const formattedDate = today.toISOString().split("T")[0];
  dateFormCE.value = formattedDate;

  let submitFormCE = document.getElementById("saveTimeFormEventosAndCursos");

  let hoursStartCE = document.getElementById("hoursStartCE");
  hoursStartCE.value = new Date().getHours() + 1;
  let minutesStartCE = document.getElementById("minutesStartCE");
  let isAmStartCE = document.getElementById("ampmStartCE");
  isAmStartCE.value = hoursStartCE.value >= 12 ? "pm" : "am";

  let inicioHorarioCE = `${
    hoursStartCE.value < 10 ? "0" + hoursStartCE.value : hoursStartCE.value
  }:${
    minutesStartCE.value < 10
      ? "0" + minutesStartCE.value
      : minutesStartCE.value
  }`;

  let hoursEndCE = document.getElementById("hoursEndCE");
  hoursEndCE.value = new Date().getHours() + 2;
  let minutesEndCE = document.getElementById("minutesEndCE");
  let isAmEndCE = document.getElementById("ampmEndCE");
  isAmEndCE.value = hoursEndCE.value >= 12 ? "pm" : "am";

  let fimHorarioCE = `${
    hoursEndCE.value < 10 ? "0" + hoursEndCE.value : hoursEndCE.value
  }:${minutesEndCE.value < 10 ? "0" + minutesEndCE.value : minutesEndCE.value}`;

  let tituloResumoCE = document.getElementById("tituloResumoCE");
  let horariosResumoCE = document.getElementById("horarios-resumoCE");
  let diaResumoCE = document.getElementById("dia-resumoCE");

  let modalTagsCE = document.getElementById("modal-tagsCE");

  const saveTimeFormCE = document.getElementById(
    "saveTimeFormEventosAndCursos"
  ); //talvez inuti

  function SaveTime(aspForm, _hideEventModal) {
    $(".eventModal").modal("hide");

    const formData = new FormData(aspForm);

    formData.set(
      "ScheduleTimeSelection.StartTime",
      `${dateFormCE.value}T${inicioHorarioCE}-03:00`
    );
    formData.set(
      "ScheduleTimeSelection.EndTime",
      `${dateFormCE.value}T${fimHorarioCE}-03:00`
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
        const content = eventData.content;
        if (content !== undefined) {
          if (content.tipo == 0) {
            content.backgroundColor = "rgba(222, 164, 156, 0.45)";
            await createLiveModal(content.id);
          } else {
            createTimeModal(content.id);
          }
          calendar.addEvent(content);
          firstForm = content;
          alertTimeSelectionCreatedSucessfully(content.id);
        }
      })
      .catch(function (error) {
        console.error(error);
      });
  }

  submitFormCE.addEventListener("submit", (event) => {
    event.preventDefault();
    const selectElementCE = document.getElementById("TagsSelectedHelp"); //PM-a tag do select2
    const selectedOptionsCE = selectElementCE.selectedOptions;
    const hideEventModalCE = selectedOptionsCE.length !== 0;

    //limpar os valores

    SaveTime(saveTimeFormCE, hideEventModalCE);
  });

  const switchStep = (currentStep, nextStep) => {
    $(currentStep).removeClass("active");
    $(nextStep).addClass("active");
  };

  // Passa da etapa 1 para a etapa 2
  $("#btnCE1").click(() => {
    switchStep(".body-CE-1", ".body-CE-2");
  });

  // Passa da etapa 2 para a etapa 3
  $("#btnCE2").click(() => {
    switchStep(".body-CE-2", ".body-CE-3");

    let tituloCE = document.getElementById(
      "TimeSelectionMentoriaTituloCE"
    ).value;
    tituloResumoCE.innerText = tituloCE;
    horariosResumoCE.innerText = inicioHorarioCE + " - " + fimHorarioCE;

    const date = new Date(dateFormCE.value);
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
    diaResumoCE.innerText = day.toString() + " " + dayName;

    const ulElement = document.getElementById(
      "select2-TagsSelectedCursosAndEventos-container"
    );
    const liElements = ulElement.querySelectorAll("li");

    const mainTag = document.createElement("span");
    mainTag.className = "modal-tag-main";
    mainTag.textContent = "Evento";
    modalTagsCE.appendChild(mainTag);

    liElements.forEach((li) => {
      const span = document.createElement("span");
      span.className = "modal-tag";
      span.textContent = li.textContent.substring(1).trim();
      modalTagsCE.appendChild(span);
    });
  });

  hoursStartCE.addEventListener("change", updateTimeStartCE);
  minutesStartCE.addEventListener("change", updateTimeStartCE);
  isAmStartCE.addEventListener("change", updateTimeStartCE);
  hoursEndCE.addEventListener("change", updateTimeEndCE);
  minutesEndCE.addEventListener("change", updateTimeEndCE);
  isAmEndCE.addEventListener("change", updateTimeEndCE);

  function updateTimeStartCE() {
    let hoursStartInt = parseInt(hoursStartCE.value, 10);
    if (isAmStartCE.value === "pm" && hoursStartInt !== 12) {
      hoursStartInt += 12;
    } else if (isAmStartCE.value === "AM" && hoursStartInt === 12) {
      hoursStartInt = 0; // Ajuste para 12 AM ser 00
    }

    let minutesInt = parseInt(minutesStartCE.value, 10);

    inicioHorarioCE = `${
      hoursStartInt < 10 ? "0" + hoursStartInt : hoursStartInt
    }:${minutesInt < 10 ? "0" + minutesInt : minutesInt}`;
  }

  function updateTimeEndCE() {
    let hoursEndInt = parseInt(hoursEndCE.value, 10);
    if (isAmEndCE.value === "pm" && hoursEndInt !== 12) {
      hoursEndInt += 12;
    } else if (isAmEndCE.value === "AM" && hoursEndInt === 12) {
      hoursEndInt = 0; // Ajuste para 12 AM ser 00
    }

    let minutesEndInt = parseInt(minutesEndCE.value, 10);

    fimHorarioCE = `${hoursEndInt < 10 ? "0" + hoursEndInt : hoursEndInt}:${
      minutesEndInt < 10 ? "0" + minutesEndInt : minutesEndInt
    }`;
  }

  // Passa da etapa 3 para a etapa 2
  $("#btnCEBack2").click(() => {
    switchStep(".body-CE-2", ".body-CE-1");
  });

  // Passa da etapa 2 para a etapa 1
  $("#btnBackCE3").click(() => {
    switchStep(".body-CE-3", ".body-CE-2");
    modalTagsCE.innerHTML = "";
  });

  // Se qualquer modal fechar, o display deve ser do body 1
  $("#eventModalCustosAndEventos").on("hidden.bs.modal", () => {
    setTimeout(() => {
      $(".modal-selectCE").removeClass("active");
      $(".body-CE-1").addClass("active");
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
