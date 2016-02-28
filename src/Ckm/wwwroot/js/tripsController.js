(function () {
    "use strict";

    function tripsController($http) {
        var vm = this;

        vm.name = "";

        //var stub = [{ name: 'US Trip', created: new Date() }, { name: 'Bbbb', created: new Date() }]
        vm.trips = [];

        vm.newTrip = {};

        vm.errorMessage = "";

        vm.isBusy = true;

        $http.get("/api/trips")
        .then(
            function (response) {
                angular.copy(response.data, vm.trips);
            },
            function (error) {
                vm.errorMessage = "Failed to load" + error;
            }
        ).finally(function () {
            vm.isBusy = false;
        });

        vm.addTrip = function () {
            vm.isBusy = true;
            vm.errorMessage = "";
            var newTrip = vm.newTrip;
            $http.post("/api/trips", vm.newTrip)
            .then(
                function (response) {
                    vm.trips.push(response.data)
                    vm.newTrip = {};
                },
                function (error) {
                    vm.errorMessage = "Failed to save new trip";
                }
            ).finally(function () {
                vm.isBusy = false;
            });
        };
    };

    // get existing module
    angular.module("app-trips").controller("tripsController", tripsController);
})();