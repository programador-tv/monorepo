let timeToView = null;
document.addEventListener('DOMContentLoaded', function () {

    $(document).on("click", ".showToAccept", function () {
        const joinId = $(this).data('jointime');
        const perfilId = $(this).data('perfil');
        const timeId = $(this).data('modal-id');

        fetch("/ScheduleActions?handler=Acceptance&id=" + perfilId)
            .then(function (response) {
                if (!response.ok) {
                    throw new Error("Erro ao enviar a solicitação POST.");
                }
                return response.json();
            })
            .then(function (data) {
                loadConfirmModal(joinId, data, timeId)
            })

        $("#JoinTimeModal").modal("show")
        $("#eventModal-" + timeId).modal("hide")
        timeToView = timeId;
    });

})

function loadConfirmModal(joinId, perfil, timeId) {
    $('#acceptanceForm').append('<input type="hidden" name="joinId" value="' + joinId + '">');
    let prepared = ""
    prepared += `
                          <div class="" >
                              <h5 style="display: flex;align-items: center; justify-content: space-around;">
                                  ${renderAvatar(perfil)}

                                  ${perfil.nome}`
    if (perfil.linkedin) {
        prepared += `<a style="text-decoration:none"
                               href="https://www.linkedin.com/in/${perfil.linkedin}/" target="_blank">
                                  <img style="width:30px;cursor:pointer" src="https://eduardoworrel.com/imgs/social/linkedin.png">
                                  </a>`
    }

    if (perfil.gitHub) {
        prepared += `<a style="text-decoration:none"
                               href="https://github.com/${perfil.gitHub}/" target="_blank">
                                  <img style="width:30px;cursor:pointer" src="https://eduardoworrel.com/imgs/social/github (1).png">
                                  </a>
                                  <br>`
    }

    prepared += `</h5>
                          <h6 style="text-align: center;">
                              ${perfil.bio ?? ""}
                          </h6>
                      `
    $("#JoinTimeMatch").html(prepared)
}


function cancelMyInvitation(jointTimeId) {
    const formRecebe = document.querySelector("#CancelTime-" + jointTimeId)
    const formData = new FormData(formRecebe);

    if (!formData.has("__RequestVerificationToken")) {
        const token = document.querySelector("#forgery input").value
        formData.append("__RequestVerificationToken", token)
    }
    formData.append("IdToCancel", jointTimeId)
    const url = "/ScheduleActions?handler=cancelMyInvitation";
    const options = {
        method: "POST",
        body: formData
    };

    fetch(url, options)
        .then((response) => {
            if (!response.ok) {
                throw new Error(`Erro: ${response.status}`);
            }
        })
        .catch(function (error) {
            console.error(error);
        }
        );
}
function cancelEvent(timeSelectionId) {
    const formRecebe = document.querySelector("#CancelTime-" + timeSelectionId)
    const formData = new FormData(formRecebe);

    if (!formData.has("__RequestVerificationToken")) {
        const token = document.querySelector("#forgery input").value
        formData.append("__RequestVerificationToken", token)
    }
    formData.append("IdToCancel", timeSelectionId)
    const url = "/ScheduleActions?handler=CancelTimeSelection";
    const options = {
        method: "POST",
        body: formData
    };

    $('#eventModal-' + timeSelectionId).modal('hide');
    setTimeout(function () {
        calendar.getEventById(timeSelectionId).remove();
        $('#card-' + timeSelectionId).remove();
        var tsIndext = asyncEvents.findIndex(el => el.id === timeSelectionId);
        if (tsIndext > -1) {
            asyncEvents.splice(tsIndext, 1);
        }
    }, 200);
    fetch(url, options)
        .then((response) => {
            if (!response.ok) {
                throw new Error(`Erro: ${response.status}`);
            }
        })
        .catch(function (error) {
            console.error(error);
        });
    removeTimeSelection(timeSelectionId);
}
function removeTimeSelection(timeSelectionId) {
    $('#eventModal-' + timeSelectionId).modal('hide')
    setTimeout(function () {
        calendar.getEventById(timeSelectionId).remove();
    }, 200);
}