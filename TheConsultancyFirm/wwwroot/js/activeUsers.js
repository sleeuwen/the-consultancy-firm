jQuery(function ($) {

    // Only start activeusers is found 
    // Start once as it will reschedule itself
    if ($('#activeUsers').length) updateActiveUsers();

    // Interval at which the badge should update, in milliseconds
    var interval = 20 * 1000;

    // Update the contact unread count badge
    function updateActiveUsers() {
        $.ajax({
            url: '/api/dashboard/home/GetCurrentActiveUsers',
            dataType: 'json',
            success: function (result) {
                //sets the amount in the span
                $('#activeUsers').text(result);
            },
            complete: function () {
                // Always reschedule a new update every interval
                setTimeout(updateActiveUsers, interval);
            },
            error: function (request) {
                console.log(request);
            }
        });
    }
});
