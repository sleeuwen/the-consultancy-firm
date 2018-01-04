jQuery(function ($) {
    // Interval at which the badge should update, in milliseconds
    var interval = 20 * 1000;

    // Update the contact unread count badge
    function updateUnreadCounter() {
        $.ajax({
            url: '/api/dashboard/contacts/unread',
            dataType: 'json',
            success: function (result) {
                // Set to empty string when there are no unread messages to hide the element.
                $('.badge').text(result === 0 ? '' : result);
            },
            complete: function () {
                // Always reschedule a new update every interval
                setTimeout(updateUnreadCounter, interval);
            },
        });
    }

    // Start once as it will reschedule itself
    updateUnreadCounter();
});
