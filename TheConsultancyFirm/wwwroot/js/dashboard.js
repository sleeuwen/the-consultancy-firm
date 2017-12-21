//Dahboard berichten badge
$.ajax({
    url: '/Dashboard/Contacts/CountUnreaded',
    dataType: 'json',
    success: function (result) {
        console.log(result);
        if (result !== 0)
        {
            $(".badge").css('display', 'inline-block');
            $(".badge").text(result);
        }
        else
            $(".badge").css('display', 'none');
    },
    timeout: 5000
});