document.addEventListener("DOMContentLoaded", function () {
  let dateForm = document.getElementById("dateTime");

  let hoursStart = document.getElementById("hoursStart");
  let minutesStart = document.getElementById("minutesStart");
  let isAmStart = document.getElementById("ampmStart");

  let inicioHorario = `${
    hoursStart.value < 10 ? "0" + hoursStart.value : hoursStart.value
  }:${minutesStart.value < 10 ? "0" + minutesStart.value : minutesStart.value}`;

  let hoursEnd = document.getElementById("hoursEnd");
  let minutesEnd = document.getElementById("minutesEnd");
  let isAmEnd = document.getElementById("ampmEnd");

  let fimHorario = `${
    hoursEnd.value < 10 ? "0" + hoursEnd.value : hoursEnd.value
  }:${minutesEnd.value < 10 ? "0" + minutesEnd.value : minutesEnd.value}`;

  let tituloResumo = document.getElementById("tituloResumo");
  let horariosResumo = document.getElementById("horarios-resumo");
  let diaResumo = document.getElementById("dia-resumo");

  let modalTags = document.getElementById("modal-tags");

  const saveTimeForm = document.getElementById("saveTimeForm");

  function SaveTime(aspForm, _hideEventModal) {
    $(".eventModal").modal("hide");

    const formData = new FormData(aspForm);
    formData.set(
      "ScheduleTimeSelection.StartTime",
      `${dateTime.value}T${inicioHorario}-03:00`
    );
    formData.set(
      "ScheduleTimeSelection.EndTime",
      `${dateTime.value}T${fimHorario}-03:00`
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
            timeSelectionIdForLive = content.id;
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

  function createTimeModal(id) {
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

  hoursStart.addEventListener("change", updateTimeStart);
  minutesStart.addEventListener("change", updateTimeStart);
  isAmStart.addEventListener("change", updateTimeStart);
  hoursEnd.addEventListener("change", updateTimeEnd);
  minutesEnd.addEventListener("change", updateTimeEnd);
  isAmEnd.addEventListener("change", updateTimeEnd);

  function updateTimeStart() {
    let hoursStartInt = parseInt(hoursStart.value, 10);
    if (isAmStart.value === "pm" && hoursStartInt !== 12) {
      hoursStartInt += 12;
    } else if (isAmStart.value === "AM" && hoursStartInt === 12) {
      hoursStartInt = 0; // Ajuste para 12 AM ser 00
    }

    let minutesInt = parseInt(minutesStart.value, 10);

    inicioHorario = `${
      hoursStartInt < 10 ? "0" + hoursStartInt : hoursStartInt
    }:${minutesInt < 10 ? "0" + minutesInt : minutesInt}`;
  }

  function updateTimeEnd() {
    let hoursEndInt = parseInt(hoursEnd.value, 10);
    if (isAmEnd.value === "pm" && hoursEndInt !== 12) {
      hoursEndInt += 12;
    } else if (isAmEnd.value === "AM" && hoursEndInt === 12) {
      hoursEndInt = 0; // Ajuste para 12 AM ser 00
    }

    let minutesEndInt = parseInt(minutesEnd.value, 10);

    fimHorario = `${hoursEndInt < 10 ? "0" + hoursEndInt : hoursEndInt}:${
      minutesEndInt < 10 ? "0" + minutesEndInt : minutesEndInt
    }`;
  }

  saveTimeForm.addEventListener("submit", (event) => {
    event.preventDefault();
    const selectElement = document.getElementById("TagsSelected");
    const selectedOptions = selectElement.selectedOptions;
    const hideEventModal = selectedOptions.length !== 0;

    //limpar os valores

    SaveTime(saveTimeForm, hideEventModal);
  });

  const switchStep = (currentStep, nextStep) => {
    $(currentStep).removeClass("active");
    $(nextStep).addClass("active");
  };

  // Passa da etapa 1 para a etapa 2
  $("#btnOneToOne1").click(() => {
    switchStep(".body-oneToOne-1", ".body-oneToOne-2");
  });

  // Passa da etapa 2 para a etapa 3
  $("#btnOneToOne2").click(() => {
    switchStep(".body-oneToOne-2", ".body-oneToOne-3");
    let titulo = document.getElementById("TimeSelectionMentoriaTitulo").value;
    tituloResumo.innerText = titulo;
    horariosResumo.innerText = inicioHorario + " - " + fimHorario;

    const date = new Date(dateForm.value);
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
    diaResumo.innerText = day.toString() + " " + dayName;

    const ulElement = document.getElementById("select2-TagsSelected-container");
    const liElements = ulElement.querySelectorAll("li");

    const mainTag = document.createElement("span");
    mainTag.className = "modal-tag-main";
    mainTag.textContent = "Dúvidas";
    modalTags.appendChild(mainTag);

    liElements.forEach((li) => {
      const span = document.createElement("span");
      span.className = "modal-tag";
      span.textContent = li.textContent.substring(1).trim();
      modalTags.appendChild(span);
    });
  });

  // Passa da etapa 3 para a etapa 2
  $("#btnBack1").click(() => {
    switchStep(".body-oneToOne-2", ".body-oneToOne-1");
  });

  // Passa da etapa 2 para a etapa 1
  $("#btnBack2").click(() => {
    switchStep(".body-oneToOne-3", ".body-oneToOne-2");
    modalTags.innerHTML = "";
  });

  // Se qualquer modal fechar, o display deve ser do body 1
  $("#eventModalOneToOne").on("hidden.bs.modal", () => {
    setTimeout(() => {
      $(".modal-select").removeClass("active");
      $(".body-oneToOne-1").addClass("active");
    }, 100);
  });
  let dateTime = document.getElementById("dateTime");

  let pendentesContainer;
});
