jQuery(function ($) {
    var $navbar = $('.navbar');
    var $searchform = $('.search');

    $navbar.find('.menu-toggle').on('click', function () {
        // Close search form when menu toggle is clicked
        $navbar.find('.navbar-collapse').toggleClass('open');
        $searchform.removeClass('open');
    });

    $searchform.find('button[type=submit]').on('click', function (e) {
        // Submit search form when opened and click on the magnifying glass
        if ($searchform.find('input').width() > 0 && $searchform.find('input').val() !== '') {
            return;
        }

        // Open search form but don't submit when the searchform is closed
        e.preventDefault();
        $searchform.toggleClass('open');
        $searchform.find('input').focus();
        $navbar.find('.navbar-collapse').removeClass('open');
    });

    $searchform.find('.fa-times').on('click', function () {
        // Clear and close search form when close icon is clicked
        $searchform.find('input').val('');
        $searchform.removeClass('open');
    });
});
