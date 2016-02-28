(function(){
    var sideBarToggle = $("#sideBarToggle");
    var icon = $("#sideBarToggle i.fa");

    sideBarToggle.on("click", function () {

        var sideBar = $("#sidebar");
        $("#sidebar").toggleClass("hide-sidebar");

        var sideBar = $("#wrapper");
        $("#wrapper").toggleClass("hide-sidebar");

        if ($("#wrapper").hasClass("hide-sidebar"))
        {
            $(icon).removeClass("fa-angle-left");
            $(icon).addClass("fa-angle-right");
        } else {
            $(icon).removeClass("fa-angle-right");
            $(icon).addClass("fa-angle-left");
        }
    });
})();
