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
}