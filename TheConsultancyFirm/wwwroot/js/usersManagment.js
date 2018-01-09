function showDeleteModal(email, id) {
    $("#deleteModal  .modal-body p").text("Email: " + email);
    $("#deleteModal input[name=id]").val(id);
    $("#deleteModal").modal()
}
