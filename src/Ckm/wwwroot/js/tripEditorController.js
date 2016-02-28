(function () {
    "use strict";

    function tripEditorController($routeParams, $http) {
        var vm = this;

        vm.tripName = $routeParams.tripName;
        vm.stops = [];
        vm.errorMessage = "";
        vm.isBusy = true;

        vm.newStop = {} ;

        var url = "/api/trips/" + vm.tripName + "/stops";

        $http.get(url)
        .then(
            function (response) {

                console.log(url);
                console.log(response.data);

                angular.copy(response.data, vm.stops);
                _showMap(vm.stops);
            },
            function (error) {
                vm.errorMessage = "Failed to load" + error;
            }
        ).finally(function () {
            vm.isBusy = false;
        });

        vm.addStop = function () {
            vm.isBusy = true;
            vm.errorMessage = "";

            //var newStop = vm.newStop;
            $http.post(url, vm.newStop)
            .then(
                function (response) {
                    vm.stops.push(response.data)
                    _showMap(vm.stops);
                    vm.newStop= {};
                },
                function (error) {
                    vm.errorMessage = "Failed to add new stop";
                }
            ).finally(function () {
                vm.isBusy = false;
            });

        };


    }

    function _showMap(stops) {
        if (stops && stops.length > 0) {
            var mapStops = _.map(stops, function (x) {
                return { lat: x.latitude, long: x.longitude, info: x.name };
            });

            travelMap.createMap({
                selector: "#map",
                stops: mapStops,
                currentStop: 1,
                initialZoom: 3
            });
        }
    };

    // get existing module
    angular.module("app-trips")
        .controller("tripEditorController", tripEditorController);
})();