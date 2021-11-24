function search() { //TODO: söka namn med taggar
    var input, filter, list, li, title, tags, i;
    input = document.getElementById("search");
    filter = input.value.toUpperCase();
    list = document.getElementById("listbox");
    li = list.getElementsByClassName("container-list");

    console.log(list.childElementCount);

    for (i = 0; i < li.length; i++) {
        console.log("i = " + i);
        title = li[i].getElementsByClassName("container-info")[0].getElementsByClassName("text-title")[0];
        tags = li[i].getElementsByClassName("container-info")[0].getElementsByClassName("tags")[0];

        if (getFilter() !== -1) {
            if (title.innerHTML.toUpperCase().indexOf(filter) > -1 && tags.innerHTML.toUpperCase().indexOf(getFilter()) > -1) {
                li[i].style.display = "";
            } else {
                li[i].style.display = "none";
            }
        }
        else if (title.innerHTML.toUpperCase().indexOf(filter) > -1) {
            li[i].style.display = "";
        } else {
            li[i].style.display = "none";
        }
    }
    checkList();
}

function isHidden(el) {
    var style = window.getComputedStyle(el);
    return ((style.display === 'none') || (style.visibility === 'hidden'));
}

function checkList() {
    var anyShown = false;
    var shown = 0;
    list = document.getElementById("listbox");
    li = list.getElementsByClassName("container-list");

    for (i = 0; i < li.length; i++) {
        if (!isHidden(li[i])) {
            anyShown = true;
            shown++;
        }
    }
    if (anyShown === true) {
        document.getElementById("noobjects").style.display = "none";
        document.getElementById("resultat").innerHTML = shown + " resultat.";
    } else {
        document.getElementById("noobjects").style.display = "";
        document.getElementById("resultat").innerHTML = "";
    }
}

function filter(i) {
    var filtercontainer = document.getElementById("filtercontainer");
    var buttons = filtercontainer.getElementsByClassName("filter");
    if (buttons[i].style.backgroundColor === "rgb(0, 182, 221)")
        var turnOn = true;
    for (var n = 0; n < buttons.length; n++) {
        buttons[n].style.backgroundColor = "rgb(0, 182, 221)";
    }
    if (turnOn === true) {
        buttons[i].style.backgroundColor = "rgb(0, 146, 179)";
    }
    search();
}

function getFilter() {
    var buttons = document.getElementById("filtercontainer").getElementsByClassName("filter");
    for (var n = 0; n < buttons.length; n++) {
        if (buttons[n].style.backgroundColor === "rgb(0, 146, 179)")
            return buttons[n].innerHTML.toUpperCase();
    }
    return -1;
}