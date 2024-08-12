let asyncEvents = [];

document.addEventListener("DOMContentLoaded", () => {
  createSelect2("#TagsSelected");

  createSelect2("#TagsSelectedCursosAndEventos");

  createSelect2("#TagsForScheduleLive");

  createSelect2("#TagsSelectedHelp");

  createSelect2("#TagsForScheduleLiveOnChannel");

  createSelect2("#TagsSelectedMentoria");

  createSelect2("#TagsForLive");

  const todayDate = new Date();
  let actualDay = String(todayDate.getDate()).padStart(2, "0");
  let actualMonth = String(todayDate.getMonth() + 1).padStart(2, "0");
  let actualYear = todayDate.getFullYear();
  let formatedDate = `${actualYear}-${actualMonth}-${actualDay}`;
  const actualHour = todayDate.getHours();
  const actualMinutes = todayDate.getMinutes();

  firstPickerHoursInput.value = String(actualHour + 1).padStart(2, "0");
  firstPickerMinutesInput.value = String(actualMinutes).padStart(2, "0");

  lastPickerHoursInput.value = String(actualHour + 2).padStart(2, "0");
  lastPickerMinutesInput.value = String(actualMinutes).padStart(2, "0");

  if (actualHour >= 23) {
    firstPickerHoursInput.value = "01";
    lastPickerHoursInput.value = "02";

    todayDate.setDate(todayDate.getDate() + 1);
    actualDay = String(todayDate.getDate()).padStart(2, "0");
    actualMonth = String(todayDate.getMonth() + 1).padStart(2, "0");
    actualYear = todayDate.getFullYear();
    formatedDate = `${actualYear}-${actualMonth}-${actualDay}`;
  } else if (actualHour == 22) {
    firstPickerHoursInput.value = "23";
    firstPickerMinutesInput.value = "00";
    lastPickerHoursInput.value = "23";
    lastPickerMinutesInput.value = "59";

    todayDate.setDate(todayDate.getDate() + 1);
    actualDay = String(todayDate.getDate()).padStart(2, "0");
    actualMonth = String(todayDate.getMonth() + 1).padStart(2, "0");
    actualYear = todayDate.getFullYear();
    formatedDate = `${actualYear}-${actualMonth}-${actualDay}`;
  }
  dateInput.value = formatedDate;
});
const firstPickerHoursInput = document.querySelector(".time-picker .hours1");
const firstPickerMinutesInput = document.querySelector(
  ".time-picker .minutes1"
);
const lastPickerHoursInput = document.querySelector(".time-picker .hours2");
const lastPickerMinutesInput = document.querySelector(".time-picker .minutes2");
const dateInput = document.querySelector(".date-picker .dateTime-picker-input");
const alertEl = document.querySelector(".timePickerAlert");
const firstPickerUpHoursButton = document.querySelector(
  ".time-picker .firstPickerUpHH"
);
const firstPickerDownHoursButton = document.querySelector(
  ".time-picker .firstPickerDownHH"
);
const firstPickerUpMinutesButton = document.querySelector(
  ".time-picker .firstPickerUpMM"
);
const firstPickerDownMinutesButton = document.querySelector(
  ".time-picker .firstPickerDownMM"
);
const lastPickerUpHoursButton = document.querySelector(
  ".time-picker .lastPickerUpHH"
);
const lastPickerDownHoursButton = document.querySelector(
  ".time-picker .lastPickerDownHH"
);
const lastPickerUpMinutesButton = document.querySelector(
  ".time-picker .lastPickerUpMM"
);
const lastPickerDownMinutesButton = document.querySelector(
  ".time-picker .lastPickerDownMM"
);
const dateTimePickerContainer = document.querySelector(".dateTime-picker");
const displayTimeAlert = document.querySelector(".displayTimeAlert");
const okBtn = document.querySelector(".ok");
let initialClickTime = 0;
let finalClickTime = 0;
let totalClickTime = 0;
let lastFPHour = 0;
let lastLPHour = 0;
let lastFPMinutes = 0;
let lastLPMinutes = 0;
let initialDate;
let finalDate;
let intervalId;
let keyPressed = false;

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

function showMessage(message, isError) {
  if (isError) {
    alertEl.innerHTML = message;
    alertEl.style.visibility = "visible";
    alertEl.classList.add("shake");
    setTimeout(() => {
      alertEl.classList.remove("shake");
    }, 1500);
  } else {
    alertEl.style.visibility = "hidden";
  }
}

// Time constants
const THIRTY_MINUTES = 1800000;
const TWO_HOURS = 7200000;
const TWENTY_FOUR_HOURS = 86400000;
const ONE_MONTH = 2592000000;

function validateDateTime(date, firstTime, lastTime, eventType = undefined) {
  const currentDate = new Date();
  const altDate = new Date(new Date(date).getTime() + TWENTY_FOUR_HOURS);
  altDate.setHours(firstTime.getHours());
  altDate.setMinutes(firstTime.getMinutes());
  altDate.toLocaleDateString();

  if (date === "") {
    showMessage("Data inválida.", true);
    return false;
  } else if (altDate.getTime() - currentDate.getTime() > ONE_MONTH) {
    showMessage(
      "Um evento não pode ser agendado com mais de um mês de antecedência.",
      true
    );
    return false;
  } else if (altDate.getTime() - currentDate.getTime() < 0) {
    showMessage(
      "De volta ao futuro? A data do evento não pode ser no passado.",
      true
    );
    return false;
  } else if (
    altDate.getTime() < currentDate.getTime() ||
    altDate.getTime() - currentDate.getTime() > ONE_MONTH ||
    firstTime - lastTime >= 0 ||
    firstTime.getTime() + TWENTY_FOUR_HOURS - currentDate.getTime() <= 0 ||
    lastTime.getTime() - firstTime.getTime() > TWO_HOURS ||
    lastTime.getTime() - firstTime.getTime() < THIRTY_MINUTES
  ) {
    return false;
  } else {
    showMessage("", false);
    return true;
  }
}

// Event listeners

okBtn.addEventListener("click", () => {
  const firstPickersTimeVerification = new Date(dateInput.value);
  const lastPickersTimeVerification = new Date(dateInput.value);
  firstPickersTimeVerification.setHours(firstPickerHoursInput.value);
  firstPickersTimeVerification.setMinutes(firstPickerMinutesInput.value);
  lastPickersTimeVerification.setHours(lastPickerHoursInput.value);
  lastPickersTimeVerification.setMinutes(lastPickerMinutesInput.value);

  if (
    validateDateTime(
      dateInput.value,
      firstPickersTimeVerification,
      lastPickersTimeVerification,
      $("#tipoTempoLivre").val()
    )
  ) {
    let hours = parseInt(firstPickerHoursInput.value);
    let minutes = parseInt(firstPickerMinutesInput.value);

    initialDate = new Date(dateInput.value);
    initialDate.setUTCHours(hours, minutes);

    hours = parseInt(lastPickerHoursInput.value);
    minutes = parseInt(lastPickerMinutesInput.value);

    finalDate = new Date(dateInput.value);
    finalDate.setUTCHours(hours, minutes);

    const startStr = transformDateFormat(initialDate.toISOString());
    const endStr = transformDateFormat(finalDate.toISOString());

    var event = {
      id: "temp",
      title: "",
      start: startStr,
      end: endStr,
      backgroundColor: "lightblue",
      borderColor: "darkblue",
    };

    if (validateTimeOverlap(event)) {
      // calendar.addEvent(event);

      // document.querySelector('#ScheduleTimeSelection_StartTime').value = startStr;
      // document.querySelector('#ScheduleTimeSelection_EndTime').value = endStr;
      document
        .querySelectorAll(".startLive")
        .forEach((e) => (e.value = startStr));
      document.querySelectorAll(".endLive").forEach((e) => (e.value = endStr));

      $("#timePickerModal").modal("hide");

      switch ($("#tipoTempoLivre").val()) {
        //PM- Lógica do flow (nao vai mais existir, cada um vai ter a propria)
        case "1:1":
          $("#eventModalOneToOne").modal("show");
          break;
        case "cursos":
          $("#eventModalCustosAndEventos").modal("show");
          break;
        case "requestHelp":
          $("#eventModalRequestHelp").modal("show");
          break;
        case "scheduled":
          $("#eventModalLiveSchedule").modal("show");
          break;
        default:
          $("#eventModalLiveScheduleOnChannel").modal("show");
          break;
      }
    } else {
      showMessage("Conflito entre datas de eventos.", true);
    }
  }
});

function changeTime(input, action, maxValue, inputLocation) {
  let hours = parseInt(input.value);
  if (action) {
    if (hours < maxValue) {
      input.value = ("00" + (hours + 1)).slice(-2);
    } else {
      input.value = "00";
      switch (inputLocation) {
        case 2:
          changeTime(firstPickerHoursInput, action, 23, 1);
          break;
        case 4:
          changeTime(lastPickerHoursInput, action, 23, 3);
          break;
      }
    }
  } else {
    if (hours > 0) {
      input.value = ("00" + (hours - 1)).slice(-2);
    } else {
      input.value = maxValue;
      switch (inputLocation) {
        case 2:
          changeTime(firstPickerHoursInput, action, 23, 1);
          break;
        case 4:
          changeTime(lastPickerHoursInput, action, 23, 3);
          break;
      }
    }
  }
}

function preventInvalidCharacter(input, inputLocation, maxValue, event) {
  if (event.key === "Enter" || event.key === "ArrowRight") {
    changeFocus(inputLocation);
    return;
  }
  if (event.key === "ArrowLeft") {
    changeFocus(inputLocation - 2);
    return;
  }
  if (!keyPressed) {
    input.value = "";
    keyPressed = true;
  }
  let strValue = String(input.value);
  if (strValue.length == 2) {
    strNewValue = event.key.padStart(2, strValue.charAt(1));
    if (strValue.charAt(1) === "0") {
      strNewValue = "0" + event.key;
    }
    if (parseInt(strNewValue) > maxValue) {
      event.preventDefault();
      input.value = maxValue;
      changeFocus(inputLocation);
    } else {
      event.preventDefault();
      input.value = strNewValue;
      changeFocus(inputLocation);
    }
  } else {
    event.preventDefault();
    input.value = event.key.padStart(2, "0");
  }
}

function inputFocus(input, inputLocation) {
  keyPressed = false;
}

function changeFocus(inputLocation) {
  switch (inputLocation) {
    case 0:
      firstPickerHoursInput.focus();
      inputFocus(firstPickerHoursInput, 1);
      break;
    case 1:
      firstPickerMinutesInput.focus();
      inputFocus(firstPickerMinutesInput, 2);
      break;
    case 2:
      lastPickerHoursInput.focus();
      inputFocus(lastPickerHoursInput, 3);
      break;
    case 3:
      lastPickerMinutesInput.focus();
      inputFocus(lastPickerMinutesInput, 4);
      break;
    case 4:
      okBtn.focus();
      keyPressed = false;
      break;
    default:
      break;
  }
}

function valueListener() {
  lastFPHour = firstPickerHoursInput.value;
  lastLPHour = lastPickerHoursInput.value;
  lastFPMinutes = firstPickerMinutesInput.value;
  lastLPMinutes = lastPickerMinutesInput.value;
  setInterval(() => {
    if (
      lastFPHour != firstPickerHoursInput.value ||
      lastLPHour != lastPickerHoursInput.value ||
      lastFPMinutes != firstPickerMinutesInput.value ||
      lastLPMinutes != lastPickerMinutesInput.value
    ) {
      lastFPHour = firstPickerHoursInput.value;
      lastLPHour = lastPickerHoursInput.value;
      lastFPMinutes = firstPickerMinutesInput.value;
      lastLPMinutes = lastPickerMinutesInput.value;
      displayTotalTime();
    }
  }, 10);
}

function displayTotalTime() {
  const deltaH =
    parseInt(lastPickerHoursInput.value) -
    parseInt(firstPickerHoursInput.value);
  const deltaM =
    parseInt(lastPickerMinutesInput.value) -
    parseInt(firstPickerMinutesInput.value);
  const totalTime = deltaH * 60 + deltaM;
  finalM = totalTime % 60;
  finalH = (totalTime - finalM) / 60;
  if (totalTime > 120) {
    showMessage("Não é possível criar um evento com mais de duas horas.", true);
    displayTimeAlert.innerHTML =
      "Duração total: " +
      finalH +
      "h" +
      (finalM > 0 ? " e " + finalM + "min" : "");
  } else if (totalTime < 30 && totalTime > 0) {
    showMessage(
      "Não é possível criar um evento com menos de trinta minutos.",
      true
    );
    displayTimeAlert.innerHTML =
      finalM > 0 ? "Duração total: " + finalM + "min" : "";
  } else if (totalTime <= 0) {
    showMessage("Horário de início e fim inválidos.", true);
    displayTimeAlert.innerHTML = "";
  } else {
    if (finalH > 0) {
      displayTimeAlert.innerHTML =
        "Duração total: " +
        finalH +
        "h" +
        (finalM > 0 ? " e " + finalM + "min" : "");
    } else {
      displayTimeAlert.innerHTML =
        finalM > 0 ? "Duração total: " + finalM + "min" : "";
    }
    showMessage("", false);
  }
}

// Event listeners

firstPickerUpHoursButton.addEventListener("mousedown", function () {
  initialClickTime = Date.now();
  intervalId = setInterval(() => {
    changeTime(firstPickerHoursInput, true, 23, 1);
  }, 100);
});
firstPickerUpHoursButton.addEventListener("mouseup", function () {
  clearInterval(intervalId);
  finalClickTime = Date.now();
  totalClickTime = finalClickTime - initialClickTime;
  if (totalClickTime < 100) {
    changeTime(firstPickerHoursInput, true, 23, 1);
  }
});
firstPickerUpHoursButton.addEventListener("mouseout", function () {
  clearInterval(intervalId);
  finalClickTime = Date.now();
  totalClickTime = finalClickTime - initialClickTime;
  if (totalClickTime < 100) {
    changeTime(firstPickerHoursInput, true, 23, 1);
  }
});

lastPickerUpHoursButton.addEventListener("mousedown", function () {
  initialClickTime = Date.now();
  intervalId = setInterval(() => {
    changeTime(lastPickerHoursInput, true, 23, 3);
  }, 100);
});
lastPickerUpHoursButton.addEventListener("mouseup", function () {
  clearInterval(intervalId);
  finalClickTime = Date.now();
  totalClickTime = finalClickTime - initialClickTime;
  if (totalClickTime < 100) {
    changeTime(lastPickerHoursInput, true, 23, 3);
  }
});
lastPickerUpHoursButton.addEventListener("mouseout", function () {
  clearInterval(intervalId);
  finalClickTime = Date.now();
  totalClickTime = finalClickTime - initialClickTime;
  if (totalClickTime < 100) {
    changeTime(lastPickerHoursInput, true, 23, 3);
  }
});

firstPickerDownHoursButton.addEventListener("mousedown", function () {
  initialClickTime = Date.now();
  intervalId = setInterval(() => {
    changeTime(firstPickerHoursInput, false, 23, 1);
  }, 100);
});
firstPickerDownHoursButton.addEventListener("mouseup", function () {
  clearInterval(intervalId);
  finalClickTime = Date.now();
  totalClickTime = finalClickTime - initialClickTime;
  if (totalClickTime < 100) {
    changeTime(firstPickerHoursInput, false, 23, 1);
  }
});
firstPickerDownHoursButton.addEventListener("mouseout", function () {
  clearInterval(intervalId);
  finalClickTime = Date.now();
  totalClickTime = finalClickTime - initialClickTime;
  if (totalClickTime < 100) {
    changeTime(firstPickerHoursInput, false, 23, 1);
  }
});

lastPickerDownHoursButton.addEventListener("mousedown", function () {
  initialClickTime = Date.now();
  intervalId = setInterval(() => {
    changeTime(lastPickerHoursInput, false, 23, 3);
  }, 100);
});
lastPickerDownHoursButton.addEventListener("mouseup", function () {
  clearInterval(intervalId);
  finalClickTime = Date.now();
  totalClickTime = finalClickTime - initialClickTime;
  if (totalClickTime < 100) {
    changeTime(lastPickerHoursInput, false, 23, 3);
  }
});
lastPickerDownHoursButton.addEventListener("mouseout", function () {
  clearInterval(intervalId);
  finalClickTime = Date.now();
  totalClickTime = finalClickTime - initialClickTime;
  if (totalClickTime < 100) {
    changeTime(lastPickerHoursInput, false, 23, 3);
  }
});

firstPickerUpMinutesButton.addEventListener("mousedown", function () {
  initialClickTime = Date.now();
  intervalId = setInterval(() => {
    changeTime(firstPickerMinutesInput, true, 59, 2);
  }, 100);
});
firstPickerUpMinutesButton.addEventListener("mouseup", function () {
  clearInterval(intervalId);
  finalClickTime = Date.now();
  totalClickTime = finalClickTime - initialClickTime;
  if (totalClickTime < 100) {
    changeTime(firstPickerMinutesInput, true, 59, 2);
  }
});
firstPickerUpMinutesButton.addEventListener("mouseout", function () {
  clearInterval(intervalId);
  finalClickTime = Date.now();
  totalClickTime = finalClickTime - initialClickTime;
  if (totalClickTime < 100) {
    changeTime(firstPickerMinutesInput, true, 59, 2);
  }
});

lastPickerUpMinutesButton.addEventListener("mousedown", function () {
  initialClickTime = Date.now();
  intervalId = setInterval(() => {
    changeTime(lastPickerMinutesInput, true, 59, 4);
  }, 100);
});
lastPickerUpMinutesButton.addEventListener("mouseup", function () {
  clearInterval(intervalId);
  finalClickTime = Date.now();
  totalClickTime = finalClickTime - initialClickTime;
  if (totalClickTime < 100) {
    changeTime(lastPickerMinutesInput, true, 59, 4);
  }
});
lastPickerUpMinutesButton.addEventListener("mouseout", function () {
  clearInterval(intervalId);
  finalClickTime = Date.now();
  totalClickTime = finalClickTime - initialClickTime;
  if (totalClickTime < 100) {
    changeTime(lastPickerMinutesInput, true, 59, 4);
  }
});

firstPickerDownMinutesButton.addEventListener("mousedown", function () {
  initialClickTime = Date.now();
  intervalId = setInterval(() => {
    changeTime(firstPickerMinutesInput, false, 59, 2);
  }, 100);
});
firstPickerDownMinutesButton.addEventListener("mouseup", function () {
  clearInterval(intervalId);
  finalClickTime = Date.now();
  totalClickTime = finalClickTime - initialClickTime;
  if (totalClickTime < 100) {
    changeTime(firstPickerMinutesInput, false, 59, 2);
  }
});
firstPickerDownMinutesButton.addEventListener("mouseout", function () {
  clearInterval(intervalId);
  finalClickTime = Date.now();
  totalClickTime = finalClickTime - initialClickTime;
  if (totalClickTime < 100) {
    changeTime(firstPickerMinutesInput, false, 59, 2);
  }
});

lastPickerDownMinutesButton.addEventListener("mousedown", function () {
  initialClickTime = Date.now();
  intervalId = setInterval(() => {
    changeTime(lastPickerMinutesInput, false, 59, 4);
  }, 100);
});
lastPickerDownMinutesButton.addEventListener("mouseup", function () {
  clearInterval(intervalId);
  finalClickTime = Date.now();
  totalClickTime = finalClickTime - initialClickTime;
  if (totalClickTime < 100) {
    changeTime(lastPickerMinutesInput, false, 59, 4);
  }
});
lastPickerDownMinutesButton.addEventListener("mouseout", function () {
  clearInterval(intervalId);
  finalClickTime = Date.now();
  totalClickTime = finalClickTime - initialClickTime;
  if (totalClickTime < 100) {
    changeTime(lastPickerMinutesInput, false, 59, 4);
  }
});

firstPickerHoursInput.addEventListener("keydown", function () {
  preventInvalidCharacter(firstPickerHoursInput, 1, 23, event);
});
firstPickerMinutesInput.addEventListener("keydown", function () {
  preventInvalidCharacter(firstPickerMinutesInput, 2, 59, event);
});
lastPickerHoursInput.addEventListener("keydown", function () {
  preventInvalidCharacter(lastPickerHoursInput, 3, 23, event);
});
lastPickerMinutesInput.addEventListener("keydown", function () {
  preventInvalidCharacter(lastPickerMinutesInput, 4, 59, event);
});

firstPickerHoursInput.addEventListener("keydown", function (event) {
  if (event.key === "e" || event.key === "." || event.key === "-") {
    event.preventDefault();
  }
});

firstPickerMinutesInput.addEventListener("keydown", function (event) {
  if (event.key === "e" || event.key === "." || event.key === "-") {
    event.preventDefault();
  }
});

lastPickerHoursInput.addEventListener("keydown", function (event) {
  if (event.key === "e" || event.key === "." || event.key === "-") {
    event.preventDefault();
  }
});

lastPickerMinutesInput.addEventListener("keydown", function (event) {
  if (event.key === "e" || event.key === "." || event.key === "-") {
    event.preventDefault();
  }
});

firstPickerHoursInput.addEventListener("focus", () => {
  inputFocus(firstPickerHoursInput, 1);
});
firstPickerMinutesInput.addEventListener("focus", () => {
  inputFocus(firstPickerMinutesInput, 2);
});
lastPickerHoursInput.addEventListener("focus", () => {
  inputFocus(lastPickerHoursInput, 3);
});
lastPickerMinutesInput.addEventListener("focus", () => {
  inputFocus(lastPickerMinutesInput, 4);
});

valueListener();

function transformDateFormat(input) {
  let output = input.slice(0, -5);

  output += "-03:00";

  return output;
}
