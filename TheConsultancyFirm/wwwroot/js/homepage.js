jQuery(function ($) {
    $('#selectNewsItems').on('show.bs.modal', function () {
        var $modal = $(this);

        $.ajax({
            url: '/api/NewsItems',
            method: 'GET',
            success: function (items) {
                var $body = $modal.find('.table tbody');
                $body.empty();

                for (var i = 0; i < items.length; i++) {
                    $body.append(
                        '<tr>' +
                            '<th>' +
                                '<div class="custom-control custom-checkbox">' +
                                    '<input class="custom-control-input" type="checkbox" id="newsItemCheckbox' + items[i].id + '" />' +
                                    '<label class="custom-control-label" id="newsItemCheckbox' + items[i].id + '" />' +
                                '</div>' +
                            '</th>' +
                            '<td><label class="mb-0" for="newsItemCheckbox' + items[i].id + '">' + items[i].title + '</label></td>' +
                        '</tr>'
                    );
                }
            }
        });
    });

    $('#selectNewsItems').on('click', 'tr', function (e) {
        e.preventDefault();
        var $checkbox = $(this).find('input[type=checkbox]');
        $checkbox.prop('checked', !$checkbox.prop('checked'));
    });
});
