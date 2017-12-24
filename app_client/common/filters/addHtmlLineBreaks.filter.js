(function() {
    angular 
    .module('MTLSpot')
    .filter('addHtmlLineBreaks', addHtmlLineBreaks);

    function addHtmlLineBreaks(){
        return function(text) {
            varoutpur = text.replace(/\n/g, '<br/>');
            return output;
        };
    }
})();