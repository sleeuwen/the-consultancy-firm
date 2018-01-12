jQuery(function ($) {
    var $blocksList = $('#blocksList');

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
                $block.toggleClass('open');
                $block.find('.block-content').slideToggle();

                if (block === 'Text') {
                    initTinyMCE();
                }

                if (block === 'Carousel' || block === 'SolutionAdvantages') {
                    initDragDrop();
                }
            }
        });
    });
});
