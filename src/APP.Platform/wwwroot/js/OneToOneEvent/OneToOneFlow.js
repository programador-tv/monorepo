document.addEventListener("DOMContentLoaded", function () {
  let dateForm = document.getElementById("dateTime");
  let formInicio = document.getElementById("startTime");
  let formFim = document.getElementById("endTime");

  let tituloResumo = document.getElementById("tituloResumo");
  let horariosResumo = document.getElementById("horarios-resumo");
  let diaResumo = document.getElementById("dia-resumo");

  let modalTags = document.getElementById("modal-tags");

  var tagsSelected = $("#TagsSelected").attr("id");

  const saveTimeForm = document.getElementById("saveTimeForm");

  function SaveTime(aspForm, _hideEventModal) {
    $(".eventModal").modal("hide");

    const formData = new FormData(aspForm);
    formData.set(
      "ScheduleTimeSelection.StartTime",
      `${dateTime.value}T${formData.get(
        "ScheduleTimeSelection.StartTime"
      )}-03:00`
    );
    formData.set(
      "ScheduleTimeSelection.EndTime",
      `${dateTime.value}T${formData.get("ScheduleTimeSelection.EndTime")}-03:00`
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

  saveTimeForm.addEventListener("submit", (event) => {
    console.log("chamado");
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
    let dia = dateForm.value;
    tituloResumo.innerText = titulo;
    horariosResumo.innerText = formInicio.value + " - " + formFim.value;

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

  console.log(saveTimeForm);
  let dateTime = document.getElementById("dateTime");

  let pendentesContainer;
});
