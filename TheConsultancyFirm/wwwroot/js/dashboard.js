$(function () {
    var $blocksList = $('#blocksList');

    $blocksList.on('click', '.expand', function () {
        openBlock($(this).closest('.block'));
    });
    $blocksList.on('click', '.delete', function () {
        var $block = $(this).closest('.block');
        $block.remove();
    });

    $blocksList.each(function (idx, element) {
        Sortable.create(element, {
            handle: '.handle',
            animation: 150,
            onChoose: function (e) {
                tinymce.execCommand('mceRemoveEditor', false, $(e.item).find('textarea').attr('id'));
            },
            onEnd: function (e) {
                tinymce.execCommand('mceAddEditor', false, $(e.item).find('textarea').attr('id'));
            }
        });
    });

    $('#chooseBlockModal').find('.block-choice').on('click', function () {
        var block = $(this).data('block');

        $.ajax({
            method: 'GET',
            url: '/dashboard/blocks/'+block,
            type: 'html',
            success: function (html) {
                $blocksList.append(html);
                $('#chooseBlockModal').modal('hide');

                var $block = $blocksList.find('.block').last();
                openBlock($block);

                if (block === 'Text') {
                    initTinyMCE();
                }
            },
        });
    });

    $('[data-blocks-submit]').on('click', function () {
        var selector = $(this).data('blocks-submit');

        $(selector).each(function () {
            tinymce.triggerSave();

            $.ajax({
                method: 'POST',
                url: $(this).attr('action'),
                data: $(this).serialize(),
                success: function (id) {
                    console.log(id);

                    $('#blocksList').find('.block').each(function (index, element) {
                        $(this).find('form').find('input[name=Order]').val(index + 1);

                        $.ajax({
                            method: 'POST',
                            url: $(this).find('form').attr('action') + '?contentType=Case&contentId=' + id,
                            data: $(this).find('form').serialize(),
                            success: function () {
                            },
                        });
                    });
                },
                error: function (err) {
                    console.log(err);
                },
            });
        });
    });

    function openBlock($block) {
        $block.toggleClass('open');
        $block.find('.block-content').slideToggle();
    }

    function initTinyMCE() {
        tinymce.init({
            selector: '.text-block textarea.form-control',
            content_css: '/css/site.min.css,https://fonts.googleapis.com/css?family=Raleway:300%2C400%2C700|Ubuntu:400%2C500%2C700',
            height: 400,
        });
    }

    initTinyMCE();

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
