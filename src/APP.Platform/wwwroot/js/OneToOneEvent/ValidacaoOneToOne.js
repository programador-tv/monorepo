document.addEventListener("DOMContentLoaded", function () {
  let formInicio = document.getElementById("startTime");
  let hoursStart = document.getElementById("hoursStart");
  let minutesStart = document.getElementById("minutesStart");
  let isAmStart = document.getElementById("ampmStart");
  let inicioHorario = `${
    hoursStart.value < 10 ? "0" + hoursStart.value : hoursStart.value
  }:${minutesStart.value < 10 ? "0" + minutesStart.value : minutesStart.value}`;

  let formFim = document.getElementById("endTime");
  let hoursEnd = document.getElementById("hoursEnd");
  let minutesEnd = document.getElementById("minutesEnd");
  let isAmEnd = document.getElementById("ampmEnd");
  let fimHorario = `${
    hoursEnd.value < 10 ? "0" + hoursEnd.value : hoursEnd.value
  }:${minutesEnd.value < 10 ? "0" + minutesEnd.value : minutesEnd.value}`;

  let dateForm = document.getElementById("dateTime");
  let btnNext2 = document.getElementById("btnOneToOne2");
  let errorSpanDate = document.getElementById("errorMsgDate");
  let errorSpanTime = document.getElementById("errorMsgTime");
  let primeiroBotao = document.getElementById("btnOneToOne1");
  let tituloMentoria = document.getElementById("TimeSelectionMentoriaTitulo");

  if (
    !dateForm ||
    !btnNext2 ||
    !errorSpanDate ||
    !errorSpanTime ||
    !primeiroBotao ||
    !tituloMentoria
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
      errorSpanTime.innerText = message;
      errorSpanTime.style.display = "block";
    } else {
      errorSpanTime.innerText = "";
      errorSpanTime.style.display = "none";
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

  const validateDates = () => {
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

    const startDate = inicioHorario;
    const endDate = fimHorario;
    const dateFormValue = dateForm.value.trim();

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
    errorSpanDate.innerText = "";
    errorSpanDate.style.display = "none";
    errorSpanTime.innerText = "";
    errorSpanTime.style.display = "none";
  };

  const validateEventName = () => {
    return tituloMentoria.value.trim() !== "";
  };

  const validateEmptyTags = () => {
    const ulElementForTags = document.getElementById(
      "select2-TagsSelected-container"
    );
    if (!ulElementForTags) {
      console.error(
        "Elemento 'select2-TagsSelected-container' não encontrado."
      );
      return false;
    }
    const liElementsForTags = ulElementForTags.querySelectorAll("li");
    return liElementsForTags.length > 0;
  };

  const validateFirstModal = () => {
    if (validateEventName() && validateEmptyTags()) {
      primeiroBotao.disabled = false;
    } else {
      primeiroBotao.disabled = true;
    }
  };

  dateForm.addEventListener("change", validateDates);
  tituloMentoria.addEventListener("input", validateFirstModal);
  $("#TagsSelected").on("select2:select", validateFirstModal);
  $("#TagsSelected").on("select2:unselect", validateFirstModal);

  hoursStart.addEventListener("change", validateDates);
  minutesStart.addEventListener("change", validateDates);
  isAmStart.addEventListener("change", validateDates);
  hoursEnd.addEventListener("change", validateDates);
  minutesEnd.addEventListener("change", validateDates);
  isAmEnd.addEventListener("change", validateDates);

  validateDates();
  validateFirstModal();
});
