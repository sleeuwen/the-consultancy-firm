function showCreationModal() {
    console.log(document.getElementById("inputForm").action);
    if (!/\/(Customers|Vacancies)\//.test(document.getElementById("inputForm").action)) {
        $("#createModal").modal();
    } else {
        window.location = document.getElementById("inputForm").action.replace("TranslationChoice", "Create");    
    }
    

}

$('#modalYes').on('change', function () {
    $('#selectBox').removeClass('d-none');
});

$('#modalNo').on('change', function () {
    $('#selectBox').addClass('d-none');
});
