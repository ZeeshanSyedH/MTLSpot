/* GET HOME PAGE */
module.exports.about = function (req, res) {
	res.render('about-display', {
		title: 'About'
	});
};
