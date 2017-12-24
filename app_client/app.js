angular.module('MTLSpot', ['ngRoute']);

function config($routeProvider) {
	$routeProvider
		.when('/', {
			templateUrl: 'home/home.view.html'
		})
		.otherwise({
			redirectTo: '/'
		});
}

angular.module('MTLSpot')
	.config(['$routeProvider', config]);
