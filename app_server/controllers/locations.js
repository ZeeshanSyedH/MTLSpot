/* GET HOME PAGE */
module.exports.homelist=function(req,res)
{
    res.render('locations-list',
    {
        title:'MTLSpot - Find a place to work with wifi',
        pageHeader: 
        {
            title:'MTLSpot',
            strapline: 'Find places in Montreal to eat, meet or just hang out !'
        },
        sidebar: 'Looking for wifi and a seat? MTLSpot helps you find places to work when out and about. Perhaps with coffee, cake or a pint? Let MTLSpot help you find the place you are looking for.',
        locations:
        [
            {
                name:'Starcups',
                address: '125 High Street, Reading, RG6 1PS',
                rating:3,
                facilities:['Hot drinks', 'Food', 'Premium Wifi'],
                distance:'100m'
            },
            {
                name:'Cafe Hero',
                address: '125 High Street, Reading, RG6 1PS',
                rating:5,
                facilities:['Hot drinks', 'Food', 'Premium Wifi'],
                distance:'100m'
            },
            {
                name:'Burger Queen',
                address: '125 High Street, Reading, RG6 1PS',
                rating:2,
                facilities:['Hot drinks', 'Food', 'Premium Wifi'],
                distance:'250'
            }
        ]
    });
};

module.exports.locationInfo=function(req,res)
{
    res.render('location-info',{title:'Location Info'});
};

module.exports.addReview = function(req,res)
{
    res.render('location-review-form', {title: 'Add Review'});
};