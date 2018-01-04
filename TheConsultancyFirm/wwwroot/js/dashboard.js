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
                if (block === 'Carousel') {
                    initDragndrop();
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
                            enctype: 'multipart/form-data',
                            data: new FormData($(this).find('form')[0]),
                            processData: false,
                            contentType: false,
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

    $('.select2').select2({ theme: 'bootstrap' });

    var tabId = 10;
    $blocksList.on('click', '.carousel-block #add-slide', function (e) {
        e.preventDefault();
        e.stopPropagation();

        tabId += 1;

        var $nav = $(this).closest('.nav-tabs');

        var tabLinkTemplate = '<a class="nav-item nav-link" id="carousel-nav-slide-{{tabId}}" data-toggle="tab" role="tab" href="#carousel-tab-slide-{{tabId}}">Slide {{tabIndex}} <i class="fa fa-times"></i></a>';
        var tabContentTemplate = '<div class="tab-pane fade" id="carousel-tab-slide-{{tabId}}" role="tabpanel"><div class="row"><div class="col-8"><div class="box"><div class="box__input"><svg class="box__icon" xmlns="http://www.w3.org/2000/svg" width="50" height="43" viewBox="0 0 50 43"><path d="M48.4 26.5c-.9 0-1.7.7-1.7 1.7v11.6h-43.3v-11.6c0-.9-.7-1.7-1.7-1.7s-1.7.7-1.7 1.7v13.2c0 .9.7 1.7 1.7 1.7h46.7c.9 0 1.7-.7 1.7-1.7v-13.2c0-1-.7-1.7-1.7-1.7zm-24.5 6.1c.3.3.8.5 1.2.5.4 0 .9-.2 1.2-.5l10-11.6c.7-.7.7-1.7 0-2.4s-1.7-.7-2.4 0l-7.1 8.3v-25.3c0-.9-.7-1.7-1.7-1.7s-1.7.7-1.7 1.7v25.3l-7.1-8.3c-.7-.7-1.7-.7-2.4 0s-.7 1.7 0 2.4l10 11.6z"></path></svg><input type="file" name="Slides[{{tabIndex0}}].Image" id="file" class="box__file" accept="image/png,image/jpeg"><label for="file"><strong>Choose a file</strong><span class="box__dragndrop"> or drag it here</span>.</label></div><div class="box__uploading">Uploading…</div><div class="box__success">Done! <a class="box__restart" role="button">Upload more?</a></div><div class="box__error">Error! <span></span>. <a class="box__restart" role="button">Try again!</a></div><input type="hidden" name="ajax" value="1"></div></div><div class="col-4"><label>Description</label><textarea class="carousel-description form-control" placeholder="Place carousel slide text here" name="Slides[{{tabIndex0}}].Text"></textarea></div></div></div>';

        var tabIndex = $nav.children().length;
        $(this).before(tabLinkTemplate.replace(/\{\{tabId\}\}/g, tabId).replace(/\{\{tabIndex\}\}/g, tabIndex));
        $(this).closest('.slides').find('.tab-content').append(tabContentTemplate.replace(/\{\{tabId\}\}/g, tabId).replace(/\{\{tabIndex\}\}/g, tabIndex).replace(/\{\{tabIndex0\}\}/g, tabIndex - 1));

        initDragndrop();

        $nav.children().eq(tabIndex - 1).click();
    });
    $blocksList.on('click', '.fa-times', function (e) {
        e.preventDefault();
        e.stopPropagation();

        var $tabItem = $(this).parent();
        var $tabList = $tabItem.parent();
        var $tabContent = $($tabItem.attr('href'));
        var idx = $tabItem.index();
        $tabItem.remove();

        if ($tabList.children().length > 1) {
            if (idx === 0 || idx === $tabList.children().length) {
                $tabList.children().first().click();
            } else {
                $tabList.children().eq(idx - 1).click();
            }
        }

        $tabContent.delay(150).queue(function () {
            $tabContent.remove();
            $(this).dequeue();
        });
    });

    var isAdvancedUpload = function() {
        var div = document.createElement('div');
        return (('draggable' in div) || ('ondragstart' in div && 'ondrop' in div)) && 'FormData' in window && 'FileReader' in window;
    }();

    function initDragndrop() {
        if (isAdvancedUpload) {
            var $boxes = $('.carousel-block .box');
            $boxes.addClass('has-advanced-upload');

            $boxes.on('drag dragstart dragend dragover dragenter dragleave drop', function(e) {
                e.preventDefault();
                e.stopPropagation();
            })
                .on('dragover dragenter', function() {
                    $(this).closest('.box').addClass('is-dragover');
                })
                .on('dragleave dragend drop', function() {
                    $(this).closest('.box').removeClass('is-dragover');
                })
                .on('drop', function(e) {
                    var droppedFiles = e.originalEvent.dataTransfer.files;
                    if (droppedFiles.length > 0) {
                        $(this).find('input[type=file]')[0].files = droppedFiles;
                    }
                });

            $boxes.find('input[type=file]').on('change', function (e) {
                if (e.target.files.length > 0) {
                    $(e.target).parent().find('label').text(e.target.files[0].name);

                    if ('createObjectURL' in window.URL) {
                        $(e.target).closest('.box').css('background-image', 'linear-gradient(rgba(0,0,0,.45),rgba(0,0,0,.45)), url('+window.URL.createObjectURL(e.target.files[0])+')')
                    }
                } else {
                    $(e.target).parent().find('label').html('<strong>Choose a file</strong><span class="box__dragndrop"> or drag it here</span>.');
                }
            });
        }
    }

    initDragndrop();
});
