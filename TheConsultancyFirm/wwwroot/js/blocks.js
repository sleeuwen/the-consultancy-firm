function template(tmpl, vars) {
    // Replace all text inside {{}} with the value from the object with the same key
    return tmpl.replace(/\{\{(\w+)\}\}/g, function (_, key) {
        // `key` is what was within the brackets
        return vars[key] || '{{' + key + '}}';
    });
}

jQuery(function ($) {
    var $blocksList = $('#blocksList');

    // Toggle block when clicked on expand
    $blocksList.on('click', '.expand', function () {
        var $block = $(this).closest('.block');

        $block.toggleClass('open');
        $block.find('.block-content').slideToggle();
    });

    // Delete block via ajax when clicked on delete
    $blocksList.on('click', '.delete', function () {
        var $block = $(this).closest('.block');

        $block.removeClass('open');
        $block.addClass('deleting');

        var id = $block.data('id');
        $.ajax({
            method: 'DELETE',
            url: '/dashboard/blocks/delete/' + id,
            success: function () {
                // Remove block when successful
                $block.slideUp(function () {
                    $block.remove();
                });
            },
            error: function () {
                // Log error
                console.error(arguments);
                $block.removeClass('deleting');
            },
        });
    });

    // Enable drag and drop of blocks through Sortable.js
    $blocksList.each(function (idx, element) {
        Sortable.create(element, {
            handle: '.handle',
            animation: 150,
            onChoose: function (e) {
                // Remove tinymce when element is chosen as it can't be dragged
                tinymce.execCommand('mceRemoveEditor', false, $(e.item).find('.text-block textarea').attr('id'));
            },
            onEnd: function (e) {
                // Readd tinymce when element is placed back in the list
                tinymce.execCommand('mceAddEditor', false, $(e.item).find('.text-block textarea').attr('id'));
            }
        });
    });

    /****** Carousel block ******/

    // These templates should be equivalent to the markup in Area/Dashboard/Views/Components/Block/Carousel.html
    // @formatter:off
    var slideTabTemplate =
        '<a class="nav-item nav-link" id="carousel-nav-slide-{{blockId}}-{{order}}" data-toggle="tab" role="tab" href="#carousel-tab-slide-{{blockId}}-{{order}}">' +
            'Slide <span>{{order1}}</span>' +
            '<i class="fa fa-times"></i>' +
        '</a>';
    var slideContentTemplate =
        '<div class="tab-pane fade" id="carousel-tab-slide-{{blockId}}-{{order}}" role="tabpanel" ' +
                'data-id="0" data-order="{{order}}">' +
            '<input type="hidden" id="" data-value="order" name="Slides[{{order}}].Order" value="{{order}}" ' +
                    'data-val="true" data-val-required="The Order field is required." />' +
            '<input type="hidden" id="" name="Slides[{{order}}].PhotoPath" value="" />' +
            '<div class="row">' +
                '<div class="col-8">' +
                    '<div class="box">' +
                        '<div class="box__input">' +
                            '<svg class="box__icon" xmlns="http://www.w3.org/2000/svg" width="50" height="43" viewBox="0 0 50 43">' +
                                '<path d="M48.4 26.5c-.9 0-1.7.7-1.7 1.7v11.6h-43.3v-11.6c0-.9-.7-1.7-1.7-1.7s-1.7.7-1.7 1.7v13.2c0 .9.7 1.7 1.7 1.7h46.7c.9 0 1.7-.7 1.7-1.7v-13.2c0-1-.7-1.7-1.7-1.7zm-24.5 6.1c.3.3.8.5 1.2.5.4 0 .9-.2 1.2-.5l10-11.6c.7-.7.7-1.7 0-2.4s-1.7-.7-2.4 0l-7.1 8.3v-25.3c0-.9-.7-1.7-1.7-1.7s-1.7.7-1.7 1.7v25.3l-7.1-8.3c-.7-.7-1.7-.7-2.4 0s-.7 1.7 0 2.4l10 11.6z" />' +
                            '</svg>' +
                            '<input class="box__file" accept="image/png,image/jpeg" type="file" ' +
                                    'id="{{blockId}}-Slides-{{order}}-Image" name="Slides[{{order}}].Image">' +
                            '<label for="{{blockId}}-Slides-{{order}}-Image">' +
                                '<strong>Choose a file</strong>' +
                                '<span class="box__dragndrop"> or drag it here</span>.' +
                            '</label>' +
                        '</div>' +
                    '</div>' +
                '</div>' +
                '<div class="col-4">' +
                    '<label for="{{blockId}}-Slides-{{order}}-Text">Description</label>' +
                    '<textarea class="carousel-description form-control" placeholder="Place carousel slide text here" id="{{blockId}}-Slides-{{order}}-Text" name="Slides[{{order}}].Text"></textarea>' +
                '</div>' +
            '</div>' +
        '</div>';
    // @formatter:on

    $blocksList.on('click', '.carousel-block .add-slide', function (e) {
        e.preventDefault();
        e.stopPropagation();

        var $nav = $(this).closest('.nav-tabs');

        // variables to use within the templates
        var templateVariables = {
            blockId: $(this).closest('.block').attr('data-id'),
            order: $nav.children().length - 1,
            order1: $nav.children().length,
        };

        // Append tab link
        $(this).before(template(slideTabTemplate, templateVariables));
        // Append tab content
        $(this).closest('.slides').find('.tab-content').append(template(slideContentTemplate, templateVariables));

        initDragDrop();

        // Activate the newly added slide tab
        $nav.children().eq(templateVariables.order).click();
    });

    $blocksList.on('click', '.fa-times', function (e) {
        e.preventDefault();
        e.stopPropagation();

        var $tabItem = $(this).parent();
        var $tabList = $tabItem.parent();
        var $tabContent = $($tabItem.attr('href'));
        var tabIndex = $tabItem.index();
        $tabItem.remove();
        // Flag this tab pane for removal
        $tabContent.addClass('removing');

        if ($tabItem.is('.active') && $tabList.children().length > 1) {
            // Select the previous tab is there is one, otherwise just select the first one
            if (tabIndex > 0) {
                $tabList.children().eq(tabIndex - 1).click();
            } else {
                $tabList.children().first().click();
            }

            // Remove the content after we know for sure the new tab is selected.
            setTimeout(function () {
                $tabContent.remove();
            }, 150);
        } else {
            // Just remove the content if it isn't the current selected tab
            $tabContent.remove();
        }

        var $block = $tabList.closest('.block');
        var blockId = $block.data('id');

        // Update the id, href and slide number on the tab item
        $block.find('.nav-link').not('.add-slide').each(function (newOrder) {
            $(this).attr('id', 'carousel-nav-slide-' + blockId + '-' + newOrder);
            $(this).attr('href', '#carousel-tab-slide-' + blockId + '-' + newOrder);
            $(this).find('span').text(newOrder + 1);
        });

        // Update the id and data-order on the tab panes
        // also update the id's, for's and name's for the input's and textarea's in every tab pane.
        $block.find('.tab-pane').not('.removing').each(function (newOrder) {
            var oldOrder = $(this).attr('data-order');

            $(this).attr('id', 'carousel-tab-slide-' + blockId + '-' + newOrder);
            $(this).attr('data-order', newOrder);

            $(this).find('input,textarea').each(function () {
                // Id is in the form of `{{blockId}}-Slides-{{order}}-{{name}}`
                var oldId = $(this).attr('id');
                var newId = oldId.replace(new RegExp('-' + oldOrder + '-'), '-' + newOrder + '-');
                $('[for="' + oldId + '"]').attr('for', newId);
                $(this).attr('id', newId);

                // Name is in the form of `Slides[{{order}}].{{name}}`
                var oldName = $(this).attr('name');
                var newName = oldName.replace(new RegExp('\\[' + oldOrder + '\\]'), '[' + newOrder + ']');
                $(this).attr('name', newName);
            });
        });
    });

    /****** Saving ******/

    $('[data-blocks-submit]').on('click', function () {
        var selector = $(this).data('blocks-submit');
        var $saveButton = $(this);
        $saveButton.prop('disabled', true);

        $(selector).each(function () {
            tinymce.triggerSave();

            var type = $(this).attr('data-type');

            setStatusText('Bezig met opslaan...');

            saveForm(this)
                .then(function (id) {
                    var $blocks = $blocksList.find('.block');
                    if ($blocks.length === 0) {
                        return $.Deferred().resolve();
                    }

                    var blocksSaved = 0;
                    setStatusText('Opslaan van de blokken: ' + blocksSaved + ' / ' + $blocks.length);
                    var promises = [];
                    $blocks.each(function () {
                        promises.push(
                            saveBlock(this, type, id).then(function () {
                                blocksSaved += 1;
                                console.log(blocksSaved);
                                setStatusText('Opslaan van de blokken: ' + blocksSaved + ' / ' + $blocks.length);
                            })
                        );
                    });

                    $.when(promises).then(function () {
                        console.log('done');
                        setStatusText('Opgeslagen.');
                        clearStatusText(5000);
                    }).catch(function (err) {
                        console.error(err);
                        setStatusText('Er is een fout opgetreden tijdens het opslaan.', true);
                    }).always(function () {
                        $saveButton.prop('disabled', false);
                    });
                })
                .catch(function (err) {
                    console.error(err);
                    setStatusText('Er is een fout opgetreden tijdens het opslaan.', true);
                    $saveButton.prop('disabled', false);
                })
        });
    });

    var clearStatusTextTimeout = null;
    var $statusText = $('#statusText');
    function setStatusText(msg, isError) {
        if (clearStatusTextTimeout != null) {
            clearTimeout(clearStatusTextTimeout);
            clearStatusTextTimeout = null;
        }

        isError = !!isError;

        $statusText.text(msg)
            .toggleClass('text-muted', !isError)
            .toggleClass('text-danger', isError);
    }

    function clearStatusText(delay) {
        delay = delay || 0;

        clearStatusTextTimeout =  setTimeout(function () {
            setStatusText('');
        }, delay);
    }

    function updateDataValueElements(el) {
        $(el).find('[data-value]').each(function () {
            var name = $(this).attr('data-value');
            var value = $(this).parent().attr('data-' + name);

            $(this).val(value);
        });
    }

    function saveForm(form) {
        return $.ajax({
            method: 'POST',
            url: $(form).attr('action'),
            enctype: 'multipart/form-data',
            processData: false,
            contentType: false,
            data: new FormData(form),
        });
    }

    function saveBlock(block, contentType, contentId) {
        updateDataValueElements(block);

        return $.ajax({
            method: 'POST',
            url: $(block).find('form').attr('action') + '?contentType=' + contentType + '&contentId=' + contentId,
            enctype: 'multipart/form-data',
            processData: false,
            contentType: false,
            data: new FormData($(block).find('form')[0]),
        });
    }
});
