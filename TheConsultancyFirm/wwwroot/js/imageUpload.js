window.isAdvancedUpload = function() {
    // Can i drag & drop and use FormData
    var div = document.createElement('div');
    return (('draggable' in div) || ('ondragstart' in div && 'ondrop' in div)) && 'FormData' in window && 'createObjectURL' in window.URL;
}();

var emptyFileListTemplate = '' +
    '<strong>Choose a file</strong>' +
    '<span class="box__dragndrop"> or drag it here</span>.';

function initDragDrop() {
    var $boxes = $('.block .box');
    $boxes.find('input[type=file]').on('change', function (e) {
        if (e.target.files.length > 0) {
            // Set the label text to the filename
            $(e.target).parent().find('label').text(e.target.files[0].name);

            if (isAdvancedUpload) {
                // Show the selected file as background image
                var objectUrl = window.URL.createObjectURL(e.target.files[0]);
                $(e.target).closest('.box')
                    .css('background-image',
                        'linear-gradient(rgba(0, 0, 0, 0.45), rgba(0, 0, 0, 0.45)), url(' + objectUrl + ')')
            }
        } else {
            $(e.target).parent().find('label').html(emptyFileListTemplate);
        }
    });

    if (!isAdvancedUpload) return;

    $boxes.addClass('has-advanced-upload');

    // Remove all registered drag/drop event handlers
    $boxes.off('drag dragstart dragend dragover dragenter dragleave drop');

    // Add the required event handlers
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
                // Set the dropped file on the input when dropped
                $(this).find('input[type=file]')[0].files = droppedFiles;
            }
        });
}

jQuery(function () {
    // Init drag & drop once the document has loaded
    initDragDrop();
});
