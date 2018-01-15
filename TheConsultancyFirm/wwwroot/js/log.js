function LogDownload(id) {
    $.ajax({
        url: '/api/downloads/LogDownload/' + id,
        dataType: 'json',
        success: function (result) {
            console.log('Added: ' + id);
        },
        complete: function () {
            console.log('Failed: ' + id);
        }
    });
}
