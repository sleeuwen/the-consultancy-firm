﻿// Write your JavaScript code.
jQuery(function ($) {
    var $searchform = $('.search');
    var $navbar = $('.navbar');
    var $newsletterForm = $('#newsletterForm');

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

    $('[data-carousel-follow]').each(function (idx, el) {
        var $following = $('#' + $(el).data('carousel-follow'));

        $following.on('slide.bs.carousel', function (e) {
            $(el).carousel(e.direction === "left" ? "next" : "prev");
        });
        $(el).carousel($following.find('.carousel-item.active').index());
    });

    $newsletterForm.find('button[type=submit]').click(function (e) {
        e.preventDefault();

        $.ajax({
            type: $newsletterForm.attr('method'),
            url: '/Newsletter/Index',
            data: $newsletterForm.serialize(),
            success: function (msg) {
                $newsletterForm.find('input[type=text]').css("border-bottom", "1px solid #5cb85c");
                $newsletterForm.find('input[type=text]').val('');
            },
            error: function (msg) {
                $newsletterForm.find('input[type=text]').css("border-bottom", "1px solid #FF7777");
                //$newsletterForm.find('input[type=text]').css("border-bottom", "1px solid #ADABAA");
                $newsletterForm.find('input[type=text]').popover('show');
            }
        });
    });

    //remove the popover
    $('#newsletter').click(function (e) {
        $newsletterForm.find('input[type=text]').popover('hide');
    });

    //Removes the red border
    $newsletterForm.find('input[type=text]').keypress(function (e) {
        $newsletterForm.find('input[type=text]').css("border-bottom", "1px solid #ADABAA");
        $newsletterForm.find('input[type=text]').popover('hide');
    });
});

function allowCookies(value) {
    var days = value === 'true' ? 365 : 1;
    setCookie('allow', value, days);
    $('.cookiecontainer').hide();
}

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}