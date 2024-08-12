document.getElementById('thumbnailInput').addEventListener('change', function (event) {
    const file = event.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function () {
            const previewDiv = document.getElementById('sched-preview');
            previewDiv.src = reader.result;
        }
        reader.readAsDataURL(file);
    }

});