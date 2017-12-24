(function () {

	angular
		.module('MTLSpot')
		.controller('homeCtrl', homeCtrl);

	homeCtrl.$inject = ['$scope', 'MTLSpotData', 'geolocation'];

	function homeCtrl($scope, MTLSpotData, geolocation) {
		var vm = this;
		vm.pageHeader = {
			title: 'MTLSpot',
			strapline: 'Find places to work with wifi near you!'
		};
		vm.sidebar = {
			content: "Looking for wifi and a seat? MTLSpot helps you find places to work when out and about. Perhaps with coffee, cake or a pint? Let MTLSpot help you find the place you're looking for."
		};
		vm.message = "Checking your location";

		vm.getData = function (position) {
			var lat = position.coords.latitude,
				lng = position.coords.longitude;
			vm.message = "Searching for nearby places";
			MTLSpotData.locationByCoords(lat, lng)
				.success(function (data) {
					vm.message = data.length > 0 ? "" : "No locations found nearby";
					vm.data = {
						locations: data
					};
					console.log(vm.data);
				})
				.error(function (e) {
					vm.message = "Sorry, something's gone wrong, please try again later";
				});
		};

		vm.showError = function (error) {
			$scope.$apply(function () {
				vm.message = error.message;
			});
		};

		vm.noGeo = function () {
			$scope.$apply(function () {
				vm.message = "Geolocation is not supported by this browser.";
			});
		};

		geolocation.getPosition(vm.getData, vm.showError, vm.noGeo);

	}

})();
