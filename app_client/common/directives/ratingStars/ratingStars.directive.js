(function () {
	angular
		.module('MTLSpot')
		.directive('ratingStars', ratingStars);

	function ratingStars() {
		return {
			restrict: 'EA',
			scope: {
				thisRating: '=rating'
			},
			templateUrl: '/common/directives/ratingStars/rating-stars.template.html'
		};
	}
})();
