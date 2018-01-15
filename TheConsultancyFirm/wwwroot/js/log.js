function LogDownload(id) {
    $.ajax({
        url: '/api/downloads/LogDownload/' + id,
        dataType: 'json',
        success: function (result) {
            // Set to empty string when there are no unread messages to hide the element.
            console.log('Added: ' + id);
        },
        complete: function () {
            // Always reschedule a new update every interval
            console.log('Failed: ' + id);
        }
    });
}
