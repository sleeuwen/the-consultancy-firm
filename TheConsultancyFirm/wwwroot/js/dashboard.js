function showDeleteModal(data) {
    $("#deleteModal .modal-body").html("Email: " + data)
    $("#deleteModal").modal()
    $('#deleteModal').modal('show')
    console.log("hai")
}

//Dashboard berichten badge
function updateUnreadCounter() {
    $.ajax({
        url: '/api/dashboard/contacts/unread',
        dataType: 'json',
        success: function (result) {
            $('.badge').text(result === 0 ? '' : result);
        },
        complete: function () {
            setTimeout(updateUnreadCounter, 20000);
        },
        timeout: 5000
    });
}

updateUnreadCounter();


