// Write your JavaScript code.
jQuery(function ($) {
    var $searchform = $('.search');
    var $navbar = $('.navbar');

    $('.buttonHover').each(function () {
        $(this).append('<span></span><span></span>');
    });

    $navbar.find('.menu-toggle').click(function () {
        $navbar.find('.navbar-collapse').toggleClass('open');
        $searchform.removeClass('open');
    });

    $searchform.find('button[type=submit]').click(function (e) {
        console.log($searchform.find('input').val() !== '');
        if ($searchform.find('input').width() > 0 && $searchform.find('input').val() !== '') {
            return;
        }

        e.preventDefault();
        $searchform.toggleClass('open');
        $navbar.find('.navbar-collapse').removeClass('open');
    });

    $searchform.find('.fa-times').click(function () {
        $searchform.find('input').val('');
        $searchform.removeClass('open');
    });

    var $cookiecontainer = $('.cookiecontainer');
    $cookiecontainer.find('> .fa-times').click(function () {
        $cookiecontainer.hide();
    });
});
