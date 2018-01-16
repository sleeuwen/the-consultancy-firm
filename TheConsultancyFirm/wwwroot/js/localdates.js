var count = document.getElementById("count").value;
function setLastModifiedDates() {
    for (i = 0; i < count; i++) {
        var value = document.getElementsByClassName("date");
        var date = new Date(value[i].value * 1000);
        var day = date.getDate();
        var monthIndex = date.getMonth() + 1;
        var year = date.getFullYear();
        var hours = date.getHours();
        var minutes = "0" + date.getMinutes();
        var seconds = "0" + date.getSeconds();
        var formattedTime = day + "-" + monthIndex + "-" + year + ' ' + hours + ':' + minutes.substr(-2) + ':' + seconds.substr(-2);
        document.getElementsByClassName("lastModified")[i].innerHTML = formattedTime;
    }
}
function setDates() {
    for (i = 0; i < count; i++) {
        var value = document.getElementsByClassName("dateCreated");
        var date = new Date(value[i].value * 1000);
        var day = date.getDate();
        var monthIndex = date.getMonth() + 1;
        var year = date.getFullYear();
        var hours = date.getHours();
        var minutes = "0" + date.getMinutes();
        var seconds = "0" + date.getSeconds();
        var formattedTime = day + "-" + monthIndex + "-" + year + ' ' + hours + ':' + minutes.substr(-2) + ':' + seconds.substr(-2);
        document.getElementsByClassName("created")[i].innerHTML = formattedTime;
    }
}
setDates();
setLastModifiedDates();
