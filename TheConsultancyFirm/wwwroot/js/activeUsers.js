jQuery(function ($) {
    if ($('#activeUsers').length) {
        setInterval(function () {
            $.ajax({
                url: '/api/dashboard/home/GetCurrentActiveUsers',
                dataType: 'json',
                success: function (result) {
                    console.log(result);
                    $('#activeUsers').text(result);
                },
                error: function (request, status, error) {
                    console.log(request);
                    console.log(status);
                    console.log(error);
                }
            });
        }, 5000);
    }
});
