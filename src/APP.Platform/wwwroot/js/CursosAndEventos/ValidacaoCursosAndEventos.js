document.addEventListener("DOMContentLoaded", function () {
  let hoursStartCE = document.getElementById("hoursStartCE");
  let minutesStartCE = document.getElementById("minutesStartCE");
  let isAmStartCE = document.getElementById("ampmStartCE");
  let inicioHorarioCE = `${
    hoursStartCE.value < 10 ? "0" + hoursStartCE.value : hoursStartCE.value
  }:${
    minutesStartCE.value < 10
      ? "0" + minutesStartCE.value
      : minutesStartCE.value
  }`;

  let hoursEndCE = document.getElementById("hoursEndCE");
  let minutesEndCE = document.getElementById("minutesEndCE");
  let isAmEndCE = document.getElementById("ampmEndCE");
  let fimHorarioCE = `${
    hoursEndCE.value < 10 ? "0" + hoursEndCE.value : hoursEndCE.value
  }:${minutesEndCE.value < 10 ? "0" + minutesEndCE.value : minutesEndCE.value}`;

  let dateFormCE = document.getElementById("dateTimeCE");
  let btnNext3 = document.getElementById("btnCE3");
  let btnNext2 = document.getElementById("btnCE2");
  let errorSpanDateCE = document.getElementById("errorMsgDateCE");
  let errorSpanTimeCE = document.getElementById("errorMsgTimeCE");
  let primeiroBotaoCE = document.getElementById("btnCE1");
  let tituloMentoriaCE = document.getElementById(
    "TimeSelectionMentoriaTituloCE"
  );

  if (
    !dateFormCE ||
    !btnNext3 ||
    !errorSpanDateCE ||
    !errorSpanTimeCE ||
    !primeiroBotaoCE ||
    !tituloMentoriaCE
  ) {
    console.error("Elementos essenciais não encontrados na página.");
    return;
  }
  const TWENTY_FOUR_HOURS = 24 * 60 * 60 * 1000;
  const ONE_MONTH = 30 * 24 * 60 * 60 * 1000;
  const TWO_HOURS = 2 * 60 * 60 * 1000;
  const THIRTY_MINUTES = 30 * 60 * 1000;

  function showMessage(message, isError) {
    if (isError) {
      errorSpanTimeCE.innerText = message;
      errorSpanTimeCE.style.display = "block";
    } else {
      errorSpanTimeCE.innerText = "";
      errorSpanTimeCE.style.display = "none";
    }
  }

  function validateTimeOverlap(timeSelection) {
    for (let i = 0; i < asyncEvents.length; i++) {
      const item = asyncEvents[i];
      if (item.start <= timeSelection.start && item.end > timeSelection.start) {
        return false;
      }
      if (item.start < timeSelection.end && item.end >= timeSelection.end) {
        return false;
      }
      if (item.start >= timeSelection.start && item.end <= timeSelection.end) {
        return false;
      }
    }
    return true;
  }

  function validateDateTime(date, firstTime, lastTime) {
    const currentDate = new Date();
    const eventDate = new Date(new Date(date).getTime() + TWENTY_FOUR_HOURS);
    eventDate.setHours(firstTime.getHours());
    eventDate.setMinutes(firstTime.getMinutes());

    const eventEndDate = new Date(new Date(date).getTime() + TWENTY_FOUR_HOURS);
    eventEndDate.setHours(lastTime.getHours());
    eventEndDate.setMinutes(lastTime.getMinutes());

    const utcEventDate = new Date(
      eventDate.getTime() - eventDate.getTimezoneOffset() * 60000
    );
    const utcEventEndDate = new Date(
      eventEndDate.getTime() - eventEndDate.getTimezoneOffset() * 60000
    );

    const startStr = transformDateFormat(utcEventDate.toISOString());
    const endStr = transformDateFormat(utcEventEndDate.toISOString());

    let event = {
      id: "temp",
      title: "",
      start: startStr,
      end: endStr,
      backgroundColor: "lightblue",
      borderColor: "darkblue",
    };
    
    if (!validateTimeOverlap(event)) {
      showMessage("Já existe um evento nessa data", true);
      return false;
    } else if (date === "") {
      showMessage("Data inválida.", true);
      return false;
    } else if (eventDate.getTime() - currentDate.getTime() > ONE_MONTH) {
      showMessage(
        "Um evento não pode ser agendado com mais de um mês de antecedência.",
        true
      );
      return false;
    } else if (eventDate.getTime() < currentDate.getTime()) {
      showMessage(
        "De volta ao futuro? A data do evento não pode ser no passado.",
        true
      );
      return false;
    } else if (lastTime.getTime() - firstTime.getTime() > TWO_HOURS) {
      showMessage("O evento nao pode durar mais de duas horas.", true);
      return false;
    } else if (firstTime.getTime() >= lastTime.getTime()) {
      showMessage("O horário inicial é maior que o final.", true);
    } else if (lastTime.getTime() - firstTime.getTime() < THIRTY_MINUTES) {
      showMessage("O evento não pode durar menos de 30 minutos.", true);
    } else {
      showMessage("", false);
      return true;
    }
  }

  const validateDatesCE = () => {
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

    const startDate = inicioHorarioCE;
    const endDate = fimHorarioCE;
    const dateFormValue = dateFormCE.value.trim();

    if (startDate === "" || endDate === "" || dateFormValue === "") {
      btnNext2.disabled = true;
      showMessage("Os campos de data e horário não podem estar vazios.", true);
      return;
    }

    const startTime = new Date(`1970-01-01T${startDate}`);
    const endTime = new Date(`1970-01-01T${endDate}`);

    if (!validateDateTime(dateFormValue, startTime, endTime)) {
      btnNext2.disabled = true;
      return;
    }

    btnNext2.disabled = false;
    errorSpanDateCE.innerText = "";
    errorSpanDateCE.style.display = "none";
    errorSpanTimeCE.innerText = "";
    errorSpanTimeCE.style.display = "none";
  };

  const validateEventNameCE = () => {
    return tituloMentoriaCE.value.trim() !== "";
  };

  const validateEmptyTagsCE = () => {
    const ulElementForTags = document.getElementById(
      "select2-TagsSelectedCursosAndEventos-container"
    );
    if (!ulElementForTags) {
      console.error(
        "Elemento 'select2-TagsSelectedCursosAndEventos-container' não encontradoo."
      );
      return false;
    }
    const liElementsForTags = ulElementForTags.querySelectorAll("li");
    return liElementsForTags.length > 0;
  };

  const validateFirstModalCE = () => {
    if (validateEventNameCE() && validateEmptyTagsCE()) {
      primeiroBotaoCE.disabled = false;
    } else {
      primeiroBotaoCE.disabled = true;
    }
  };

  dateFormCE.addEventListener("change", validateDatesCE);
  tituloMentoriaCE.addEventListener("input", validateFirstModalCE);
  $("#TagsSelectedCursosAndEventos").on("select2:select", validateFirstModalCE);
  $("#TagsSelectedCursosAndEventos").on(
    "select2:unselect",
    validateFirstModalCE
  );

  hoursStartCE.addEventListener("change", validateDatesCE);
  minutesStartCE.addEventListener("change", validateDatesCE);
  isAmStartCE.addEventListener("change", validateDatesCE);
  hoursEndCE.addEventListener("change", validateDatesCE);
  minutesEndCE.addEventListener("change", validateDatesCE);
  isAmEndCE.addEventListener("change", validateDatesCE);

  $("#eventModalCustosAndEventos").on("shown.bs.modal", function () {
    validateDatesCE();
    validateFirstModalCE();
  });
});
