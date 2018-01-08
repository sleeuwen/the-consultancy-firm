jQuery(function ($) {
    $('[data-carousel-follow]').each(function (idx, el) {
        var $following = $('#' + $(el).data('carousel-follow'));

        $following.on('slide.bs.carousel', function (e) {
            $(el).carousel(e.direction === "left" ? "next" : "prev");
        });

        $(el).carousel($following.find('.carousel-item.active').index());
    });
});
