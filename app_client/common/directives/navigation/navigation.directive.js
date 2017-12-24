(function () {
    angular 
    .module('MTLSpot')
    .directive('navigation', navigation);

    function navigation() {
        return {
            restrict: 'EA',
            templateUrl: '/common/directives/navigation/navigation.remplate.html'
        };
    }
})();