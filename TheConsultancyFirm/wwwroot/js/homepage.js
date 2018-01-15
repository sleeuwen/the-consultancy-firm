jQuery(function ($) {
    var $selectNewsItems = $('#selectNewsItems');

    $selectNewsItems.on('show.bs.modal', function () {
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
                                    '<input class="custom-control-input" type="checkbox" id="newsItemCheckbox' + items[i].id + '" value="' + items[i].id + '" aria-flowto=""' + (items[i].homepageOrder != null ? 'checked' : '') + ' />' +
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

    $selectNewsItems.on('click', 'tr', function (e) {
        e.preventDefault();
        var $checkbox = $(this).find('input[type=checkbox]');
        $checkbox.prop('checked', !$checkbox.prop('checked'));
    });

    $selectNewsItems.on('click', '.btn-primary', function () {
        var ids = [];

        $selectNewsItems.find('input[type=checkbox]:checked').each(function () {
            ids.push($(this).val());
        });

        console.log(ids);

        updateHomepageNewsItems(ids);
    });

    function updateHomepageNewsItems(ids) {
        return $.ajax({
            method: 'POST',
            url: '/api/dashboard/Homepage/NewsItems',
            data: 'ids=' + ids.join(','),
        })
    }
});
