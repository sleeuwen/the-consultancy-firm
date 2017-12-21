$(function () {
    $('.block').find('.expand').on('click', function () {
        var $block = $(this).closest('.block');
        $block.toggleClass('open');
        $block.find('.block-content').slideToggle();
    });

    $('.block-reorder').each(function (idx, element) {
        Sortable.create(element, {
            handle: '.handle',
            animation: 150,
            onChoose: function (e) {
                console.log(e.item);
                tinymce.execCommand('mceRemoveEditor', false, $(e.item).find('textarea').attr('id'));
            },
            onEnd: function (e) {
                tinymce.execCommand('mceAddEditor', false, $(e.item).find('textarea').attr('id'));
            }
        });
    });

    tinymce.init({
        selector: '.text-block textarea.form-control',
        content_css: '/css/site.min.css,https://fonts.googleapis.com/css?family=Raleway:300%2C400%2C700|Ubuntu:400%2C500%2C700',
        height: 400,
    });

    // Dashboard berichten badge
    function updateUnreadCounter() {
        $.ajax({
            url: '/api/dashboard/contacts/unread',
            dataType: 'json',
            success: function (result) {
                $('.badge').text(result === 0 ? '' : result);
            },
            complete: function () {
                setTimeout(updateUnreadCounter, 20000);
            },
            timeout: 5000
        });
    }

    updateUnreadCounter();
});
