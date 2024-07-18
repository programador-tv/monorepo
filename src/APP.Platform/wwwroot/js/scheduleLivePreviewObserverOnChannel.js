document.getElementById('thumbnailInput').addEventListener('change', function (event) {
    var file = event.target.files[0];
    if (file) {
        var reader = new FileReader();
        reader.onload = function () {
            var previewDiv = document.getElementById('sched-preview');
            previewDiv.src = reader.result;
        }
        reader.readAsDataURL(file);
    }

});