document.addEventListener("DOMContentLoaded", function (e) {
    const browserOpt = document.querySelector("#browser-opt");
    const scheduleTab = document.getElementById("scheduleLive-tab");
    const obsOpt = document.querySelector("#obs-opt");
    const hiddenInput = document.querySelector("#stream-opt");
    const thumbnailLabel = document.querySelector("#thumb-label");
    const thumbnailInput = document.querySelector("#Live_Thumbnail");

    // se o usuário não estiver no Chrome, o stream escolhido por padrão é o OBS Studio
    if(!browserOpt.classList.contains("chosen-platform")) {
        obsRules();
    }

    function obsRules () {
        if (!obsOpt.classList.contains("chosen-platform")) {
            obsOpt.classList.add("chosen-platform");
            hiddenInput.value = "true";
            thumbnailInput.setAttribute("required", "true");
            thumbnailLabel.innerHTML = "Pré-Vizualização<span style='color:firebrick'>*</span>"
        }
        if (browserOpt.classList.contains("chosen-platform")) {
            browserOpt.classList.remove("chosen-platform");
            thumbnailLabel.innerHTML = "Pré-Vizualização"
        }
    }

    function scheduleThumbnailRules() {
        thumbnailInput.setAttribute("required", "true");
        thumbnailLabel.innerHTML = `Pré-Vizualização<span style="color:firebrick">*</span>`

    }

    browserOpt.addEventListener("click", function () {
        if (!browserOpt.classList.contains("chosen-platform")) {
            browserOpt.classList.add("chosen-platform");
            hiddenInput.value = "false";
        }
        if (obsOpt.classList.contains("chosen-platform")) {
            obsOpt.classList.remove("chosen-platform");
            thumbnailInput.removeAttribute("required");
            thumbnailLabel.innerHTML = `Pré-Vizualização (Opcional)`
        }
    });

    obsOpt.addEventListener("click", obsRules);
    scheduleTab.addEventListener("click", scheduleThumbnailRules)
});



function validarFormulario () {
    const streamOption = document.querySelectorAll(".stream-chosen");
    const peloMenosUmaSelecionada = Array.from(streamOption).some(function (div) {
        return div.classList.contains("chosen-platform");
    });

    if (!peloMenosUmaSelecionada) {
        alert("Escolha pelo menos uma plataforma de stream.");
        return false;
    }

    return true;
}