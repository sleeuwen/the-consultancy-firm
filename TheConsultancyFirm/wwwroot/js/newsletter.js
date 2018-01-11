function ReplaceTemplates() {
    var date = new Date();

    var dayNr = (date.getDay() + 6) % 7;

    date.setDate(date.getDate() - dayNr + 3);

    var jan4 = new Date(date.getFullYear(), 0, 4);

    var year = date.getFullYear();

    var dayDiff = (date - jan4) / 86400000;

    var weekNr = Math.ceil(dayDiff / 7);

    $('#newsletterPreview').contents().find('#week').text(weekNr);
    $('#newsletterPreview').contents().find('#NewsletterTemplateIntro').html(
        'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes');
    $('#newsletterPreview').contents().find('#NewsletterTemplateOtherNews').html(
        'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes');
    $('#newsletterPreview').contents().find('#year').text(year);
    $('#newsletterPreview').contents().find('#summaryCase').text('Hier komt automatisch de sharingdescription te staan van het nieuwste item, als deze mail verstuurd wordt');
    $('#newsletterPreview').contents().find('#summaryNews').text('Hier komt automatisch de sharingdescription te staan van het nieuwste item, als deze mail verstuurd wordt');
    $('#newsletterPreview').contents().find('#summaryDownload').text('Hier komt automatisch de sharingdescription te staan van het nieuwste item, als deze mail verstuurd wordt');
}

function UpdateIntroTextOnTemplate() {
    if ($('#NewsletterIntro').val() === null) return;
    
    tinymce.triggerSave();
    var text = $('#NewsletterIntro').val();
    //console.log('NewsletterIntro');
    $('iframe').contents().find('#NewsletterTemplateIntro').html(text.replace('<p>', "").replace('</p>', ""));
}

function UpdateOtherNewsTextOnTemplate() {
    if ($('#NewsletterOtherNews').val() === null) return;

    tinymce.triggerSave();
    var text = $('#NewsletterOtherNews').val();
    //console.log('NewsletterOtherNews');
    $('iframe').contents().find('#NewsletterTemplateOtherNews').html(text.replace('<p>', "").replace('</p>', ""));
}

jQuery(function($) {
    $('#newsletterPreview').on('load', function() {
        setTimeout(ReplaceTemplates(),150);
    });
});

