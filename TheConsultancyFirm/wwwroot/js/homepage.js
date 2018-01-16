jQuery(function ($) {
    var $selectNewsItems = $('#selectNewsItems');
    var $selectCases = $('#selectCases');
    var $newsItemsList = $('#NewsItemsList');

    $selectNewsItems.on('show.bs.modal', function () {
        var $modal = $(this);

        $.ajax({
            url: '/api/NewsItems',
            method: 'GET',
            success: function (items) {
                $modal.find('.statusText').text('');
                $modal.find('.btn-primary').prop('disabled', false);

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
    $selectCases.on('show.bs.modal', function () {
        var $modal = $(this);

        $.ajax({
            url: '/api/dashboard/Cases',
            method: 'GET',
            success: function (items) {
                $modal.find('.statusText').text('');
                $modal.find('.btn-primary').prop('disabled', false);

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
            },
        });
    });

    $selectNewsItems.on('click', 'tr', function (e) {
        e.preventDefault();
        var $checkbox = $(this).find('input[type=checkbox]');
        $checkbox.prop('checked', !$checkbox.prop('checked'));

        if ($selectNewsItems.find('input[type=checkbox]:checked').length > 3) {
            $selectNewsItems.find('.statusText').text('Je kan maximaal 3 items selecteren');
            $selectNewsItems.find('.btn-primary').prop('disabled', true);
        } else {
            $selectNewsItems.find('.statusText').text('');
            $selectNewsItems.find('.btn-primary').prop('disabled', false);
        }
    });
    $selectCases.on('click', 'tr', function (e) {
        e.preventDefault();
        var $checkbox = $(this).find('input[type=checkbox]');
        $checkbox.prop('checked', !$checkbox.prop('checked'));
    });

    $selectNewsItems.on('click', '.btn-primary', function () {
        var ids = [];

        $selectNewsItems.find('input[type=checkbox]:checked').each(function () {
            ids.push($(this).val());
        });

        updateHomepageNewsItems(ids);
    });
    $selectCases.on('click', '.btn-primary', function () {
        var ids = [];

        $selectCases.find('input[type=checkbox]:checked').each(function () {
            ids.push($(this).val());
        });

        updateHomepageCases(ids);
    });

    function updateHomepageCases(ids) {
        return $.ajax({
            method: 'POST',
            url: '/api/dashboard/Homepage/Cases',
            data: 'ids=' + ids.join(','),
            success: function (data) {
                $('#CasesCarousel').html(data);
                $selectCases.modal('hide');
            },
        });
    }

    function updateHomepageNewsItems(ids) {
        return $.ajax({
            method: 'POST',
            url: '/api/dashboard/Homepage/NewsItems',
            data: 'ids=' + ids.join(','),
            success: function (data) {
                $newsItemsList.html(data);
                $selectNewsItems.modal('hide');
            },
        })
    }

    $('#NewsItemsList').each(function (idx, element) {
        Sortable.create(element, {
            animation: 150,
            store: {
                get: function () {
                    return [];
                },
                set: function (sortable) {
                    $.ajax({
                        method: 'POST',
                        url: '/api/dashboard/Homepage/NewsItems',
                        data: 'ids=' + sortable.toArray().join(','),
                    });
                },
            },
        });
    });

    $('#solutionOuter').each(function (idx, element) {
        Sortable.create(element, {
            animation: 150,
            store: {
                get: function () {
                    return [];
                },
                set: function (sortable) {
                    $.ajax({
                        method: 'POST',
                        url: '/api/dashboard/Homepage/Solutions',
                        data: 'ids=' + sortable.toArray().join(','),
                    });
                },
            },
        });
    });

    $('#CasesCarousel').each(function (idx, element) {
        Sortable.create(element, {
            animation: 150,
            store: {
                get: function () {
                    return [];
                },
                set: function (sortable) {
                    $.ajax({
                        method: 'POST',
                        url: '/api/dashboard/Homepage/Cases',
                        data: 'ids=' + sortable.toArray().join(','),
                    });
                },
            },
        });
    });
});
