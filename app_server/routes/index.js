var express = require('express');
var router = express.Router();
var controllerLocation = require('../controllers/locations.js');
var controllerOthers = require('../controllers/others.js');

/* LOCATION PAGES */
router.get('/', controllerLocation.homelist);
router.get('/location', controllerLocation.locationInfo);
router.get('/location/review/new', controllerLocation.addReview);

/* OTHER PAGES */
router.get('/about', controllerOthers.about);

module.exports = router;