function initTinyMCE() {
    tinymce.init({
        selector: '.text-block textarea.form-control',
        height: 400,
        plugins: 'anchor autolink emoticons link lists image paste save charmap hr media',
        toolbar: 'undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image media',
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

    tinymce.init({
        selector: '.newsletter textarea.form-control',
        height: 400,
        plugins: 'anchor autolink link legacyoutput',
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
        setup: function (ed) {
            ed.on('change', function (e) {
                if (ed.id === 'NewsletterIntro') {
                    UpdateIntroTextOnTemplate();
                } else if (ed.id === 'NewsletterOtherNews') {
                    UpdateOtherNewsTextOnTemplate();
                }
            });
        }
    });
}

jQuery(function () {
    // Initialize tinymce once the document has loaded
    initTinyMCE();
});
