function showDisableModal(email, id) {
    $("#disableModal  .modal-body p").text("Email: " + email);
    $("#disableModal input[name=id]").val(id);
    $("#disableModal").modal();
}

function showEnableModal(email, id) {
    $("#enableModal  .modal-body p").text("Email: " + email);
    $("#enableModal input[name=id]").val(id);
    $("#enableModal").modal();
}
