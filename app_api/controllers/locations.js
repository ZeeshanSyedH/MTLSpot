var mongoose = require('mongoose');
var Loc = mongoose.model('Location');
var request = require('request');

var apiOptions = {
	server: "http://localhost:3000"
};

if (process.env.NODE_ENV === 'production') {
	apiOptions.server = "https://mtlspot.herokuapp.com";
}

var theEarth = (function () {
	var earthRadius = 6371; // IN KM

	var getDistanceFromRads = function (rads) {
		return parseFloat(rads * earthRadius);
	};

	var getRadsFromDistance = function (distance) {
		return parseFloat(distance / earthRadius);
	};

	return {
		getDistanceFromRads: getDistanceFromRads,
		getRadsFromDistance: getRadsFromDistance
	};
})();

var sendJsonResponse = function (res, status, content) {
	res.status(status);
	res.json(content);
};

module.exports.locationsListByDistance = function (req, res) {
	var lng = parseFloat(req.query.lng);
	var lat = parseFloat(req.query.lat);
	var point = {
		type: "Point",
		coordinates: [lng, lat]
	};
	var geoOptions = {
		spherical: true,
		maxDistance: theEarth.getRadsFromDistance(20),
		num: 10
	};
	if (!lng || !lat) {
		sendJsonResponse(res, 404, {
			"Message": "Longitude(lng) & Latitude(lat) are both required"
		});
		return;
	}
	Loc.geoNear(point, geoOptions, function (err, results, stats) {
		var locations = [];
		if (err) {
			sendJsonResponse(res, 404, err);
		} else {
			results.forEach(function (doc) {
				locations.push({
					distance: theEarth.getDistanceFromRads(doc.dis),
					name: doc.obj.name,
					address: doc.obj.address,
					rating: doc.obj.rating,
					facilities: doc.obj.facilities,
					_id: doc.obj._id
				});
			});
			sendJsonResponse(res, 200, locations);
		}
	});
};

module.exports.locationsCreate = function (req, res) {
	Loc.create({
		name: req.body.name,
		address: req.body.address,
		facilities: req.body.facilities.split(","),
		coords: [parseFloat(req.body.lng), parseFloat(req.body.lat)],
		openingTimes: [{
			days: req.body.days1,
			opening: req.body.opening1,
			closing: req.body.closing1,
			closed: req.body.closed1
		}, {
			days: req.body.days2,
			opening: req.body.opening2,
			closing: req.body.closing2,
			closed: req.body.closed2
		}]
	}, function (err, location) {
		if (err) {
			sendJsonResponse(res, 400, err);
		} else {
			sendJsonResponse(res, 201, location);
		}
	});
};

module.exports.locationsReadOne = function (req, res) {
	if (req.params && req.params.locationid) {
		Loc
			.findById(req.params.locationid)
			.exec(function (err, location) {
				if (!location) {
					sendJsonResponse(res, 404, {
						"Message": "No matches for Location ID"
					});
					return;
				} else if (err) {
					sendJsonResponse(res, 404, err);
					return;
				}
				sendJsonResponse(res, 200, location);
			});
	} else {
		sendJsonResponse(res, 404, {
			"Message": "No Location ID in request"
		});
	}
};

module.exports.locationsUpdateOne = function (req, res) {
	if (!req.params.locationid) {
		sendJsonResponse(res, 404, {
			"Message": " Not found, Location ID is required"
		});
		return;
	}
	Loc
		.findById(req.params.locationid)
		.select('-reviews -rating')
		.exec(
			function (err, location) {
				if (!location) {
					sendJsonResponse(res, 400, err);
					return;
				}
				location.name = req.body.name;
				location.address = req.params.address;
				location.facilities = req.params.facilities.split(",");
				location.coords = [parseFloat(req.body.lng),
    parseFloat(req.body.lat)];
				location.openingTimes = [{
						days: req.body.days1,
						opening: req.body.opening1,
						closing: req.bosy.closing1,
						closed: req.body.closed1
    },
					{
						days: req.body.days2,
						opening: req.body.opening2,
						closing: req.bosy.closing2,
						closed: req.body.closed2
  }];
				location.save(function (err, location) {
					if (err) {
						sendJsonResponse(res, 404, err);
					} else {
						sendJsonResponse(res, 200, location);
					}
				});
			}
		);
};

module.exports.locationsDeleteOne = function (req, res) {
	var locationid = req.params.locationid;
	if (locationid) {
		Loc
			.findByIdAndRemove(locationid)
			.exec(
				function (err, location) {
					if (err) {
						sendJsonResponse(res, 404, err);
						return;
					}
					sendJsonResponse(res, 204, null);
				}
			);
	} else {
		sendJsonResponse(res, 404, {
			"Message": "No location ID"
		});
	}
};

var renderHomepage = function (req, res) {
	res.render('locations-list', {
		title: 'MTLSpot - Find a place to work with wifi',
		pageHeader: {
			title: 'MTLSpot',
			strapline: 'Find places in Montreal to eat, meet or just hang out !'
		},
		sidebar: 'Looking for wifi and a seat? MTLSpot helps you find places to work when out and about. Perhaps with coffee, cake or a pint? Let MTLSpot help you find the place you are looking for.',
		locations: [
			{
				name: 'Starcups',
				address: '125 High Street, Reading, RG6 1PS',
				rating: 3,
				facilities: ['Hot drinks', 'Food', 'Premium Wifi'],
				distance: '100m'
            },
			{
				name: 'Cafe Hero',
				address: '125 High Street, Reading, RG6 1PS',
				rating: 5,
				facilities: ['Hot drinks', 'Food', 'Premium Wifi'],
				distance: '100m'
            },
			{
				name: 'Burger Queen',
				address: '125 High Street, Reading, RG6 1PS',
				rating: 2,
				facilities: ['Hot drinks', 'Food', 'Premium Wifi'],
				distance: '250'
            }
        ]
	});
};

module.exports.homelist = function (req, res) {
	var requestOptions, path;
	path = '/api/locations';
	requestOptions = {
		url: apiOptions.server + path,
		method: "GET",
		json: {},
		qs: {
			lng: -0.7992599,
			lat: 51.378091,
			maxDistance: 20
		}
	};
	request(
		requestOptions,
		function (err, response, body) {
			renderHomepage(req, res);
		}
	);
};
