document.addEventListener("DOMContentLoaded", function () {
  let form = document.getElementById("scheduleLiveFormOnChannel");
  let dateFormSL = document.getElementById("dateTimeSL");
  let hoursStartSL = document.getElementById("hoursStartSL");
  let minutesStartSL = document.getElementById("minutesStartSL");
  let isAmStartSL = document.getElementById("ampmStartSL");
  let hoursEndSL = document.getElementById("hoursEndSL");
  let minutesEndSL = document.getElementById("minutesEndSL");
  let isAmEndSL = document.getElementById("ampmEndSL");
  let btnNext = document.getElementById("next-step-live");
  let errorTimeSpan = document.getElementById("errorLS");
  let tituloMentoria = document.getElementById("LiveTitulo-SchedOnChannel");

  const TWENTY_FOUR_HOURS = 24 * 60 * 60 * 1000;
  const ONE_MONTH = 30 * 24 * 60 * 60 * 1000;
  const TWO_HOURS = 2 * 60 * 60 * 1000;
  const THIRTY_MINUTES = 30 * 60 * 1000;

  const validateEventName = () => tituloMentoria.value.trim() !== "";

  const validateEmptyTags = () => {
    const ulElementForTags = document.getElementById(
      "select2-TagsForScheduleLiveOnChannel-container"
    );
    return (
      ulElementForTags && ulElementForTags.querySelectorAll("li").length > 0
    );
  };

  function showMessage(message, isError) {
    console.log("ERROR");
    if (isError) {
      console.log("true");

      errorTimeSpan.innerText = message;
      errorTimeSpan.style.display = "block";
      console.log(errorTimeSpan);
      console.log(errorTimeSpan.innerText);
    } else {
      console.log("NAO ERROR");

      errorTimeSpan.innerText = "";
      errorTimeSpan.style.display = "none";
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
      console.log("E agora?");
      showMessage("O evento não pode durar mais de duas horas.", true);
      return false;
    } else if (firstTime.getTime() >= lastTime.getTime()) {
      showMessage("O horário inicial é maior que o final.", true);
      return false;
    } else if (lastTime.getTime() - firstTime.getTime() < THIRTY_MINUTES) {
      showMessage("O evento não pode durar menos de 30 minutos.", true);
      return false;
    } else {
      showMessage("", false);
      return true;
    }
  }

  function validateDates() {
    let hoursStartInt = parseInt(hoursStartSL.value, 10);
    if (isAmStartSL.value === "pm" && hoursStartInt !== 12) {
      hoursStartInt += 12;
    } else if (isAmStartSL.value === "am" && hoursStartInt === 12) {
      hoursStartInt = 0; // Ajuste para 12 AM ser 00
    }
    let minutesInt = parseInt(minutesStartSL.value, 10);

    inicioHorario = `${
      hoursStartInt < 10 ? "0" + hoursStartInt : hoursStartInt
    }:${minutesInt < 10 ? "0" + minutesInt : minutesInt}`;

    let hoursEndInt = parseInt(hoursEndSL.value, 10);
    if (isAmEndSL.value === "pm" && hoursEndInt !== 12) {
      hoursEndInt += 12;
    } else if (isAmEndSL.value === "am" && hoursEndInt === 12) {
      hoursEndInt = 0; // Ajuste para 12 AM ser 00
    }

    let minutesEndInt = parseInt(minutesEndSL.value, 10);
    fimHorario = `${hoursEndInt < 10 ? "0" + hoursEndInt : hoursEndInt}:${
      minutesEndInt < 10 ? "0" + minutesEndInt : minutesEndInt
    }`;

    const startDate = inicioHorario;
    const endDate = fimHorario;
    const dateFormValueSL = dateFormSL.value.trim();

    const startTime = new Date(`1970-01-01T${startDate}`);
    const endTime = new Date(`1970-01-01T${endDate}`);

    if (!validateDateTime(dateFormValueSL, startTime, endTime)) {
      return false;
    }
    return true;
  }

  function validateFirstModal() {
    if (validateEventName() && validateEmptyTags() && validateDates()) {
      btnNext.disabled = false;
    } else {
      btnNext.disabled = true;
    }
  }

  // Initialize validation on load
  validateDates();
  validateFirstModal();

  // Event listeners
  dateFormSL.addEventListener("change", validateFirstModal);
  tituloMentoria.addEventListener("input", validateFirstModal);
  $("#TagsForScheduleLiveOnChannel").on("select2:select", validateFirstModal);
  $("#TagsForScheduleLiveOnChannel").on("select2:unselect", validateFirstModal);
  hoursStartSL.addEventListener("change", validateFirstModal);
  minutesStartSL.addEventListener("change", validateFirstModal);
  isAmStartSL.addEventListener("change", validateFirstModal);
  hoursEndSL.addEventListener("change", validateFirstModal);
  minutesEndSL.addEventListener("change", validateFirstModal);
  isAmEndSL.addEventListener("change", validateFirstModal);
});
