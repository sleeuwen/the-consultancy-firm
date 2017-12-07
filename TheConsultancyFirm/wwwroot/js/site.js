// Write your JavaScript code.
$(document).ready(function () {
    $('.button').each(function () {
        $(this).append('<span></span><span></span>');
    });

    $('#menu-toggle').click(function () {
        $('.menu').toggleClass('open');
    });

    var $searchbar = $('#searchbar');
    $searchbar.find('> .fa-search').click(function () {
        $searchbar.toggleClass('open');
    });

    $searchbar.find('> .fa-times').click(function () {
        $searchbar.find('input').val('');
        $searchbar.toggleClass('open');
    });

});
