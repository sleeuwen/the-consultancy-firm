jQuery(function ($) {
    $('time').each(function () {
        var date = new Date($(this).text() + ' UTC');
        $(this).text(date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear() + ' ' + date.getHours() + ':' + date.getMinutes());
    });
});
