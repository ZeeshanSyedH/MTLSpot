(function () {
    angular
    .module('MTLSpot')
    .controller('aboutCtrl', aboutCtrl);

    function aboutCtrl() {
        var vm = this;

        vm.pageHeader = {
            title: 'About MTLSpot',
        };
        vm.main={
            content: 'MTLSpot was created to help people find places to sit down and get a bit of work done.'
        };
    }
})();