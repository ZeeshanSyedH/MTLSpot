(function () {

	angular
		.module('MTLSpot')
		.service('MTLSpotData', MTLSpotData);

	MTLSpotData.$inject = ['$http'];

	function MTLSpotData($http) {
		var locationByCoords = function (lat, lng) {
			return $http.get('/api/locations?lng=' + lng + '&lat=' + lat + '&maxDistance=20');
		};
		return {
			locationByCoords: locationByCoords
		};
	}

})();
