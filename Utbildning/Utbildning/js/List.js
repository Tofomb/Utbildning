function search() {
    var input, filter, list, li, title, i;
    input = document.getElementById("search");
    filter = input.value.toUpperCase();
    list = document.getElementById("listbox");
    li = list.getElementsByClassName("container-list");

    console.log(list.childElementCount);

    for (i = 0; i < li.length; i++) {
        console.log("i = " + i);
        title = li[i].getElementsByClassName("container-info")[0].getElementsByClassName("text-title")[0];
        if (title.innerHTML.toUpperCase().indexOf(filter) > -1) {
            console.log("title = " + title);
            li[i].style.display = "";
        } else {
            console.log("title = " + title);
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
    var t0 = performance.now();
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
    var t1 = performance.now();
    if (anyShown === true) {
        document.getElementById("noobjects").style.display = "none";
        document.getElementById("resultat").innerHTML = shown + " resultat.";
    }
    else {
        document.getElementById("noobjects").style.display = "";
        document.getElementById("resultat").innerHTML = "";
    }
}