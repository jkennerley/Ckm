(function () {
    "use strict";
    angular.module("simpleControls", []).directive("waitCursor", waitCursor);
    function waitCursor() {

        // http://cssload.net/
        var d = [
            "/views/waitCursor1.html",
            "/views/waitCursor2.html",
            "/views/waitCursor3.html",
            "/views/waitCursor_3d_mesh_box.html",
            "/views/cssload_en_spinner_5.html"];

        var max = d.length;
        var rnd = Math.floor(Math.random() * max) + 1
        var url = d[rnd - 1];
        return {
            scope: {show: "=displayWhen"},
            restrict:"E",
            templateUrl: url
        };

    }
}
)();

