jQuery(function ($) {
    $('time').each(function () {
        var date = new Date($(this).attr('datetime'));
        $(this).text(date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear() + ' ' + date.getHours() + ':' + date.getMinutes());
    });
});
