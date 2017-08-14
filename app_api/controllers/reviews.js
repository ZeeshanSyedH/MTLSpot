var mongoose = require('mongoose');
var Loc = mongoose.model('Location');

var sendJsonResponse = function (res, status, content) {
	res.status(status);
	res.json(content);
};

module.exports.reviewsCreate = function (req, res) {
	var locationid = req.params.locationid;
	if (locationid) {
		Loc
			.findById(locationid)
			.select('reviews')
			.exec(
				function (err, location) {
					if (err) {
						sendJsonResponse(res, 400, err);
					} else {
						doAddReview(req, res, location);
					}
				}
			);
	} else {
		sendJsonResponse(res, 404, {
			"Message": "Not Found, Location Id Required"
		});
	}
};

var doAddReview = function (req, res, location) {
	if (!location) {
		sendJsonResponse(res, 400, {
			"Message": "Location ID not found"
		});
	} else {
		location.reviews.push({
			author: req.body.author.author,
			rating: req.body.rating,
			reviewText: req.body.reviewText
		});
		location.save(function (err, location) {
			var thisReview;
			if (err) {
				sendJsonResponse(res, 400, err);
			} else {
				updateAverageRating(location._id);
				thisReview = location.reviews[location.reviews.length - 1];
				sendJsonResponse(res, 201, thisReview);
			}
		});
	}
};

var updateAverageRating = function (locationid) {
	Loc
		.findById(locationid)
		.select('rating reviews')
		.exec(
			function (err, location) {
				if (!err) {
					doSetAverageRating(location);
				}
			});
};

var doSetAverageRating = function (location) {
	var i, reviewCount, ratingAverage, ratingTotal;
	if (location.reviews && location.reviews.length > 0) {
		reviewCount = location.reviews.length;
		ratingTotal = 0;
		for (i = 0; i < reviewCount; i++) {
			ratingTotal = ratingTotal + location.reviews[i].rating;
		}
		ratingAverage = parseInt(ratingTotal / reviewCount, 10);
		location.rating = ratingAverage;
		location.save(function (err) {
			if (err) {
				console.log(err);
			} else {
				console.log("Average rating updated to", ratingAverage);
			}
		});
	}
};

module.exports.reviewsReadOne = function (req, res) {
	console.log("Getting single review");
	if (req.params && req.params.locationid && req.params.reviewid) {
		Loc
			.findById(req.params.locationid)
			.select('name reviews')
			.exec(
				function (err, location) {
					console.log(location);
					var response, review;
					if (!location) {
						sendJSONresponse(res, 404, {
							"message": "locationid not found"
						});
						return;
					} else if (err) {
						sendJSONresponse(res, 400, err);
						return;
					}
					if (location.reviews && location.reviews.length > 0) {
						review = location.reviews.id(req.params.reviewid);
						if (!review) {
							sendJSONresponse(res, 404, {
								"message": "reviewid not found"
							});
						} else {
							response = {
								location: {
									name: location.name,
									id: req.params.locationid
								},
								review: review
							};
							sendJSONresponse(res, 200, response);
						}
					} else {
						sendJSONresponse(res, 404, {
							"message": "No reviews found"
						});
					}
				}
			);
	} else {
		sendJSONresponse(res, 404, {
			"message": "Not found, locationid and reviewid are both required"
		});
	}
};

module.exports.reviewUpdateOne = function (req, res) {
	if (!req.params.locationid || req.params.reviewid) {
		sendJsonResponse(res, 404, {
			"Message": "Not found, location ID & review ID are required"
		});
		return;
	}
	Loc
		.findById(req.params.locationid)
		.select('reviews')
		.exec(
			function (err, location) {
				var thisReview;
				if (!location) {
					sendJsonResponse(res, 404, {
						"Message": "Location ID not found"
					});
					return;
				} else if (err) {
					sendJsonResponse(res, 400, err);
					return;
				}
				if (location.reviews && location.reviews.length > 0) {
					thisReview = location.reviews.id(req.params.reviewid);
					if (!thisReview) {
						sendJsonResponseres(res, 404, {
							"Message": "Review ID not found"
						});
					} else {
						thisReview.author = req.body.author;
						thisReview.rating = req.body.rating;
						thisReview.reviewText = req.body.reviewText;
						location.save(function (err, location) {
							if (err) {
								sendJsonResponse(res, 404, err);
							} else {
								updateAverageRating(location._id);
								sendJsonResponse(res, 200, thisReview);
							}
						});
					}
				} else {
					sendJsonResponse(res, 202, {
						"Message": "No review to update"
					});
				}
			}
		);
};

module.exports.reviewsDeleteOne = function (req, res) {
	if (!req.params.locationid || !req.params.reviewid) {
		sendJsonResponse(res, 404, {
			"Message": "Not found, location Id & review Id are required"
		});
		return;
	}
	Loc
		.findById(req.params.locationid)
		.select('reviews')
		.exec(
			function (err, location) {
				if (!location) {
					sendJsonResponse(res, 404, {
						"Message": "Location Id not found"
					});
					return;
				} else if (err) {
					sendJsonResponse(res, 400, err);
					return;
				}
				if (location.reviews && location.reviews.length > 0) {
					if (!location.reviews.id(req, params, reviewid)) {
						sendJsonResponse(res, 404, {
							"Message": "Review id not found"
						});
					} else {
						location.reviews.id(req, params.reviewid).remove();
						location.save(function (err) {
							if (err) {
								sendJsonResponse(res, 404, err);
							} else {
								updateAverageRating(location._id);
								sendJsonResponse(res, 204, null);
							}
						});
					}
				} else {
					sendJsonResponse(res, 404, {
						"Message": "No review to delete"
					});
				}
			}
		);
};