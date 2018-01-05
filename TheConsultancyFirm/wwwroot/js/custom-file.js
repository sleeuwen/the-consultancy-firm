jQuery(function ($) {
    $(document).on('change', '.custom-file input[type=file]', function (e) {
        var $input = $(this);
        var $label = $input.next();
        if (!$label.is('.custom-file-label')) return;


        if (!$label[0].hasAttribute('data-default-label')) {
            $label.attr('data-default-label', $label.text());
        }

        if (e.target.files.length < 1) {
            $label.text($label.attr('data-default-label'));
        } else {
            $label.text(e.target.files[0].name);
        }
    });
});
