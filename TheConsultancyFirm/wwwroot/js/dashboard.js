$(function () {
    var $blocksList = $('#blocksList');

    $blocksList.on('click', '.expand', function () {
        openBlock($(this).closest('.block'));
    });
    $blocksList.on('click', '.delete', function () {
        var $block = $(this).closest('.block');
        $block.removeClass('open');
        $block.addClass('deleting');

        var id = $block.data('id');

        if (id === 0) {
            $block.slideUp(function () {
                $block.remove();
                $(this).dequeue();
            });
        }

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
    });

    $blocksList.each(function (idx, element) {
        Sortable.create(element, {
            handle: '.handle',
            animation: 150,
            onChoose: function (e) {
                tinymce.execCommand('mceRemoveEditor', false, $(e.item).find('.text-block textarea').attr('id'));
            },
            onEnd: function (e) {
                tinymce.execCommand('mceAddEditor', false, $(e.item).find('.text-block textarea').attr('id'));
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
                enctype: 'multipart/form-data',
                data: new FormData(this),
                processData: false,
                contentType: false,
                success: function (id) {
                    var $blocks = $blocksList.find('.block');

                    if ($blocks.length === 0) {
                        $statusText.text('Opgeslagen.');
                        resetStatusTextTimeout = setTimeout(function () {
                            $statusText.text('');
                        }, 5000);
                        return;
                    }

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
            height: 400,
            plugins: 'anchor autolink emoticons link lists image paste save charmap hr',
            body_class: 'content',
            content_css: [
                'https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-beta.2/css/bootstrap.min.css',
                '/css/site.min.css',
                '/css/tinymce.min.css',
                'https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css',
                'https://fonts.googleapis.com/css?family=Raleway:300,400,700|Ubuntu:400,500,700',
            ],
            branding: false,
            block_formats: 'Header 1=h2;Header 2=h3;Header3=h4;Paragraph=p;Preformatted=pre',
            style_formats: [
                {title: 'Headers', items: [
                        {title: 'Header 1', block: 'h2'},
                        {title: 'Header 2', block: 'h3'},
                        {title: 'Header 3', block: 'h4'},
                ]},
                {title: 'Blocks', items: [
                        {title: 'Paragraph', block: 'p'},
                        {title: 'Introtext', inline: 'span', classes: 'introtext'},
                ]},
                {title: 'Inline', items: [
                        {title: 'Bold', icon: 'bold', format: 'bold'},
                        {title: 'Italic', icon: 'italic', format: 'italic'},
                        {title: 'Underline', icon: 'underline', format: 'underline'},
                        {title: 'Strikethrough', icon: 'strikethrough', format: 'strikethrough'},
                        {title: 'Superscript', icon: 'superscript', format: 'superscript'},
                        {title: 'Subscript', icon: 'subscript', format: 'subscript'},
                        {title: 'Code', icon: 'code', format: 'code'},
                ]},
            ],
            images_upload_url: '/api/dashboard/blocks/upload',
            image_upload_credentials: true,
            image_dimensions: false,
            relative_urls: false,
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

    $('.select2').each(function () {
        var self = this;
        var ajaxPreload = $(this).data('select2-preload');
        if (ajaxPreload != null) {
            $.ajax($(self).data('select2-preload'), {
                success: function (data) {
                    console.log($(self).select2({data: data.results}));
                }
            })
        } else {
            $(self).select2();
        }
    });
});
