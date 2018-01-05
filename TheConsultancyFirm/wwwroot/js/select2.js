jQuery(function ($) {
    // Make all select element with the `select2` class a select2 element
    // Configuration will be done via data attributes
    $('select.select2').select2({
        theme: 'bootstrap',
    });
});
