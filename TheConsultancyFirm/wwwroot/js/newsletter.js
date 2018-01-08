jQuery(function ($) {
    var $newsletterForm = $('#newsletterForm');
    var $newsletterInput = $newsletterForm.find('input[type=text]');

    // Submit newsletter via ajax
    $newsletterForm.on('submit', function (e) {
        e.preventDefault();

        $.ajax({
            type: 'POST',
            url: '/api/Newsletter/Subscribe',
            data: $newsletterForm.serialize(),
            success: function () {
                $newsletterInput.val('');
                toastr.options.positionClass = 'toast-bottom-right';
                toastr.success('Je bent nu aangemeld voor de nieuwsbrief!', 'Success!');
            },
            error: function () {
                $newsletterInput.addClass('invalid');
                $newsletterInput.popover('show');
            }
        });
    });

    // Remove the popover when clicked anywhere
    $(document).on('click', function () {
        $newsletterInput.popover('hide');
    });

    // Removes the red border when any key is pressed
    $newsletterInput.on('keypress', function () {
        $newsletterInput.removeClass('invalid');
        $newsletterInput.popover('hide');
    });
});
