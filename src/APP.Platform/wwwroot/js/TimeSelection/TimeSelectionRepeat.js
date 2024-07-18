document.addEventListener("DOMContentLoaded", function () {
    let checkboxes = document.querySelectorAll(".opcao-checkbox");

    checkboxes.forEach(function (checkbox) {
        checkbox.addEventListener("change", function () {
            checkboxes.forEach(function (cb) {
                if (cb !== checkbox) {
                    cb.checked = false;
                }
            });
        });
    });
});

// const repeatEventForm = document.querySelector("#repeatEventForm");
// const submitButton = document.querySelector("#repeatTimeSelectionSubmitButtom");
// const optSpan = document.querySelector("#submitNotConfirmSpan");

// repeatEventForm.addEventListener("change", function () {
//     const primeiraParteSelecionada = document.querySelector(
//         "input[name='Opcao']:checked"
//     );
//     const segundaParteSelecionada = document.querySelector("#opcao5").checked;
//     const terceiraParteSelecionada = document.querySelector(
//         "input[name='Opcao1']:checked"
//     );

//     if (primeiraParteSelecionada && (segundaParteSelecionada || terceiraParteSelecionada)) {
//         submitButton.disabled = false;
//         $('#submitNotConfirmSpan').slideUp();
//     } else {
//         submitButton.disabled = true;
//         $('#submitNotConfirmSpan').slideDown();
//     }
// });

// repeatEventForm.addEventListener("submit", (event) => {
//     event.preventDefault();
//     $('#repeatModal').modal('hide');

//     const receiveForm = document.querySelector("#repeatEventForm");
//     const formData = new FormData(receiveForm);
//     const WeekRepeatPattern = document.querySelector('input[name="Opcao1"]:checked');
//     const WeekRepeatPatternValue = WeekRepeatPattern ? WeekRepeatPattern.value : null;
//     const tagsSelected = document.querySelectorAll("#TagsSelected option:checked");

//     for (let tag of tagsSelected)
//     {
//         formData.append("TagsSelected", tag.value);
//     }

//     formData.append("WeekPattern", document.querySelector('input[name="Opcao"]:checked').value);
//     formData.append("RepeatWeekParttern", WeekRepeatPatternValue);
//     formData.append("TimeSelection.Id", firstForm.id);
//     formData.append("TimeSelection.StartTime", firstForm.start);
//     formData.append("TimeSelection.EndTime", firstForm.end);
//     formData.append("TimeSelection.TituloTemporario", firstForm.title);
//     formData.append("TimeSelection.Tipo", firstForm.tipo);

//     if (!formData.has("__RequestVerificationToken")) {
//         const token = document.querySelector("#forgery input").value
//         formData.append("__RequestVerificationToken", token)
//     }

//     const url = "?handler=SaveRepeatTime";
//     const options = {
//         method: "POST",
//         body: formData
//     };

//     fetch(url, options)
//     .then((response) => {
//         if (!response.ok) {
//             throw new Error(`Erro: ${response.status}`);
//         }
//         return response.json();
//     })
//     .then((eventData) => {
//         eventData.content.forEach(ts => {
//             createTimeModal(ts.id);
//             calendar.addEvent(ts);
//         })
//         CreateModalAlert(eventData.added, eventData.notAdded)
//     })
//     .catch(function (error) {
//         console.error(error);
//     });
// });

function CreateModalAlert(addCount, notAddCount) {
    const addTimeSelectionSpan = document.querySelector("#addedTimeSelection");
    const notAddTimeSelectionSpan = document.querySelector("#notAddedTimeSelection");

    addTimeSelectionSpan.textContent = addCount;
    notAddTimeSelectionSpan.textContent = notAddCount;

    $('#modalCreateRepeatedTimeSelectionAlert').modal('show');
}

$('#repeatModal').on('hidden.bs.modal', function () {
    const checkboxes = document.querySelectorAll('input[type="checkbox"]');
    checkboxes.forEach((checkbox) => {
        checkbox.checked = false;
    });

    const radios = document.querySelectorAll('input[type="radio"]');
    radios.forEach((radio) => {
        radio.checked = false;
    });
});
