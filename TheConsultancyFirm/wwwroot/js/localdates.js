var count = document.getElementById("count").value;
function setLastModifiedDates() {
    for (i = 0; i < count; i++) {
        var value = document.getElementsByClassName("date");
        var date = new Date(value[i].value + ' UTC');
        document.getElementsByClassName("lastModified")[i].innerHTML = date;
    }
}
function setDates() {
    for (i = 0; i < count; i++) {
        var value = document.getElementsByClassName("dateCreated");
        var date = new Date(value[i].value + ' UTC');
        document.getElementsByClassName("created")[i].innerHTML = date;
    }
}
setDates();
setLastModifiedDates();
