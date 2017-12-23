$(function () {
    var $blocksList = $('#blocksList');

    $blocksList.on('click', '.expand', function () {
        openBlock($(this).closest('.block'));
    });
    $blocksList.on('click', '.delete', function () {
        var $block = $(this).closest('.block');
        $block.removeClass('open');
        var id = $block.data('id');

        $.ajax({
            method: 'DELETE',
            url: '/dashboard/blocks/delete/'+id,
            success: function () {
                $block.slideUp(function () {
                    $block.remove();
                    $(this).dequeue();
                });
            },
            error: function () {
                console.log(arguments);
                $block.removeClass('deleting');
            },
        });
        $block.addClass('deleting');
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

    var resetStatusTextTimeout = null;
    $('[data-blocks-submit]').on('click', function () {
        var selector = $(this).data('blocks-submit');

        if (resetStatusTextTimeout !== null) {
            clearTimeout(resetStatusTextTimeout);
            resetStatusTextTimeout = null;
        }

        var $statusText = $('#statusText');
        $statusText.removeClass('text-danger').addClass('text-muted');
        $(selector).each(function () {
            tinymce.triggerSave();

            $statusText.text('Bezig met opslaan...');
            $.ajax({
                method: 'POST',
                url: $(this).attr('action'),
                data: $(this).serialize(),
                success: function (id) {
                    console.log(id);

                    var $blocks = $blocksList.find('.block');
                    var saved = 0;
                    $statusText.text('Opslaan van de blokken: ' + saved + ' / ' + $blocks.length);
                    $blocks.each(function (index, element) {
                        $(this).find('form').find('input[name=Order]').val(index + 1);

                        $.ajax({
                            method: 'POST',
                            url: $(this).find('form').attr('action') + '?contentType=Case&contentId=' + id,
                            data: $(this).find('form').serialize(),
                            success: function () {
                                saved += 1;
                                $statusText.text('Opslaan van de blokken: ' + saved + ' / ' + $blocks.length);

                                if (saved === $blocks.length) {
                                    $statusText.text('Opgeslagen.');
                                    resetStatusTextTimeout = setTimeout(function () {
                                        $statusText.text('');
                                    }, 5000);
                                }
                            },
                        });
                    });
                },
                error: function (err) {
                    console.log(err);
                    $statusText.text('Er is een fout opgetreden tijdens het opslaan.').addClass('text-danger').removeClass('text-muted');
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
