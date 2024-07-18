function alertTimeSelectionCreatedSucessfully(id){
    Swal.fire({
        text: "Evento criado com sucesso!",
        showCancelButton: true,
        icon: 'success',
        cancelButtonText: "Fechar",
        confirmButtonText: "Ver evento",
        showLoaderOnConfirm: true,
        preConfirm: () => {
          openModal(id);

        }
    });
}

function openModal(modelId) {
    $("#calendarModal").modal("show");
    $('#eventModal-' + modelId).modal('show');
}

