function getQueryParam(param) {
  return new URLSearchParams(window.location.search).get(param);
}

function loginCallbackOneToOne() {
  window.location.href =
    "/Identity/Account/Login?returnUrl=" +
    encodeURIComponent(window.location.pathname) +
    "?callbackValue=1:1";
}

function loginCallbackCursosAndEventos() {
  window.location.href =
    "/Identity/Account/Login?returnUrl=" +
    encodeURIComponent(window.location.pathname) +
    "?callbackValue=cursos";
}

function loginCallbackRequestHelp() {
  console.log("rh chamando");
  window.location.href =
    "/Identity/Account/Login?returnUrl=" +
    encodeURIComponent(window.location.pathname) +
    "?callbackValue=requestHelp";
}

function loginCallback() {
  const callbackValue = $("#tipoTempoLivre").val();
  window.location.href =
    "/Identity/Account/Login?returnUrl=" +
    encodeURIComponent(window.location.pathname) +
    "?callbackValue=" +
    callbackValue;
}

function loginCallbackForLive() {
  $("#tipoTempoLivre").val("liveModal");
  loginCallback();
}
function validCalllbackValue(callbackValue) {
  const validCallbacks = ["1:1", "cursos", "requestHelp", "liveModal"];
  return validCallbacks.includes(callbackValue);
}

document.addEventListener("DOMContentLoaded", function () {
  const callbackValue = getQueryParam("callbackValue");
  const isValidCallback = validCalllbackValue(callbackValue);

  if ((!isValidCallback && callbackValue === "") || callbackValue == null) {
    return;
  } else if (callbackValue === "1:1") {
    $("#eventModalOneToOne").modal("show");
  } else if (callbackValue === "requestHelp") {
    $("#eventModalRequestHelp").modal("show");
  } else if (callbackValue === "cursos") {
    $("#eventModalCustosAndEventos").modal("show");
  } else if (callbackValue !== "liveModal") {
    $("#timePickerModal").modal("show");
    $("#tipoTempoLivre").val(callbackValue);
  } else if (callbackValue === "liveModal") {
    $("#liveModal").modal("show");
  }
});
