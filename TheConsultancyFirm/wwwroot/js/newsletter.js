function ReplaceWeekWithNumber() {
    var date = new Date();

    var dayNr = (date.getDay() + 6) % 7;

    date.setDate(date.getDate() - dayNr + 3);

    var jan4 = new Date(date.getFullYear(), 0, 4);

    var year = date.getFullYear();

    var dayDiff = (date - jan4) / 86400000;

    var weekNr = Math.ceil(dayDiff / 7);

    $('#newsletterPreview').contents().find('#week').text(weekNr);
    $('#newsletterPreview').contents().find('#year').text(year);
}

function UpdateIntroTextOnTemplate() {
    if ($('#NewsletterIntro').val() == null) return;
    
    tinymce.triggerSave();
    var text = $('#NewsletterIntro').val();
    console.log(text.replace('<p>', ""));
    text.replace('</p>', "");
    $('iframe').contents().find('#NewsletterTemplateIntro').text(text.replace('<p>', "").replace('</p>', ""));
}

jQuery(function($) {
    $('#newsletterPreview').on('load', function() {
        setTimeout(ReplaceWeekWithNumber(),150);
    });
});

