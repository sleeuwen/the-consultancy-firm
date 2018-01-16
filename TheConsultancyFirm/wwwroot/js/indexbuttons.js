function showCreationModal() {
    $("#createModal").modal();
}

$('#modalYes').on('change', function () {
    $('#selectBox').removeClass('d-none');
});

$('#modalNo').on('change', function () {
    $('#selectBox').addClass('d-none');
});
