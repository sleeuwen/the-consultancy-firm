function showDownloadGraph(id) {
    $.ajax({
        url: '/api/dashboard/downloadGraph/' + id,
        dataType: 'script',
        success: function (result) {
            //sets the download graph
            $("#downloadModal").modal();
        },
        error: function (request) {
            console.log(request);
        }
    });
}
