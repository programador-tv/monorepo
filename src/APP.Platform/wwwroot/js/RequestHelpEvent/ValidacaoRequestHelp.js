document.addEventListener("DOMContentLoaded", function () {
  let hoursStartRH = document.getElementById("hoursStartRH");
  let minutesStartRH = document.getElementById("minutesStartRH");
  let isAmStartRH = document.getElementById("ampmStartRH");
  let inicioHorarioRH = `${
    hoursStartRH.value < 10 ? "0" + hoursStartRH.value : hoursStartRH.value
  }:${
    minutesStartRH.value < 10
      ? "0" + minutesStartRH.value
      : minutesStartRH.value
  }`;

  let hoursEndRH = document.getElementById("hoursEndRH");
  let minutesEndRH = document.getElementById("minutesEndRH");
  let isAmEndRH = document.getElementById("ampmEndRH");
  let fimHorarioRH = `${
    hoursEndRH.value < 10 ? "0" + hoursEndRH.value : hoursEndRH.value
  }:${minutesEndRH.value < 10 ? "0" + minutesEndRH.value : minutesEndRH.value}`;

  let dateFormRH = document.getElementById("dateTimeRH");
  let btnNext3 = document.getElementById("btnRequestHelp3");
  let btnNext2 = document.getElementById("btnRequestHelp2");
  let errorSpanDateRH = document.getElementById("errorMsgDateRH");
  let errorSpanTimeRH = document.getElementById("errorMsgTimeRH");
  let primeiroBotaoRH = document.getElementById("btnRequestHelp1");
  let tituloMentoriaRH = document.getElementById(
    "TimeSelectionMentoriaTituloRH"
  );
  let descriptionRH = document.getElementById("descriptionRH");

  if (
    !dateFormRH ||
    !btnNext3 ||
    !errorSpanDateRH ||
    !errorSpanTimeRH ||
    !primeiroBotaoRH ||
    !tituloMentoriaRH
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
      errorSpanTimeRH.innerText = message;
      errorSpanTimeRH.style.display = "block";
    } else {
      errorSpanTimeRH.innerText = "";
      errorSpanTimeRH.style.display = "none";
    }
  }

  function validateDateTime(date, firstTime, lastTime) {
    const currentDate = new Date();
    const eventDate = new Date(new Date(date).getTime() + TWENTY_FOUR_HOURS);
    eventDate.setHours(firstTime.getHours());
    eventDate.setMinutes(firstTime.getMinutes());

    const eventEndDate = new Date(new Date(date).getTime() + TWENTY_FOUR_HOURS);
    eventEndDate.setHours(lastTime.getHours());
    eventEndDate.setMinutes(lastTime.getMinutes());

    if (date === "") {
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

  const validateDatesRH = () => {
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

    const startDate = inicioHorarioRH;
    const endDate = fimHorarioRH;
    const dateFormValue = dateFormRH.value.trim();

    if (startDate === "" || endDate === "" || dateFormValue === "") {
      btnNext3.disabled = true;
      showMessage("Os campos de data e horário não podem estar vazios.", true);
      return;
    }

    const startTime = new Date(`1970-01-01T${startDate}`);
    const endTime = new Date(`1970-01-01T${endDate}`);

    if (!validateDateTime(dateFormValue, startTime, endTime)) {
      btnNext3.disabled = true;
      return;
    }

    btnNext3.disabled = false;
    errorSpanDateRH.innerText = "";
    errorSpanDateRH.style.display = "none";
    errorSpanTimeRH.innerText = "";
    errorSpanTimeRH.style.display = "none";
  };

  const validateEventNameRH = () => {
    return tituloMentoriaRH.value.trim() !== "";
  };

  const validateEmptyTagsRH = () => {
    const ulElementForTags = document.getElementById(
      "select2-TagsSelectedHelp-container"
    );
    if (!ulElementForTags) {
      console.error(
        "Elemento 'select2-TagsSelectedHelp-container' não encontrado."
      );
      return false;
    }
    const liElementsForTags = ulElementForTags.querySelectorAll("li");
    return liElementsForTags.length > 0;
  };

  const validateFirstModalRH = () => {
    if (validateEventNameRH() && validateEmptyTagsRH()) {
      primeiroBotaoRH.disabled = false;
    } else {
      primeiroBotaoRH.disabled = true;
    }
  };

  const validateSecondModalRH = () => {
    if (descriptionRH.value.length < 50) {
      btnNext2.disabled = true;
    } else {
      btnNext2.disabled = false;
    }
  };
  dateFormRH.addEventListener("change", validateDatesRH);
  tituloMentoriaRH.addEventListener("input", validateFirstModalRH);
  $("#TagsSelectedHelp").on("select2:select", validateFirstModalRH);
  $("#TagsSelectedHelp").on("select2:unselect", validateFirstModalRH);

  descriptionRH.addEventListener("input", validateSecondModalRH);
  hoursStartRH.addEventListener("change", validateDatesRH);
  minutesStartRH.addEventListener("change", validateDatesRH);
  isAmStartRH.addEventListener("change", validateDatesRH);
  hoursEndRH.addEventListener("change", validateDatesRH);
  minutesEndRH.addEventListener("change", validateDatesRH);
  isAmEndRH.addEventListener("change", validateDatesRH);

  $("#eventModalRequestHelp").on("shown.bs.modal", function () {
    validateSecondModalRH();
    validateDatesRH();
    validateFirstModalRH();
  });
});
