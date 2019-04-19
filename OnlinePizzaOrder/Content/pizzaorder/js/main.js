
(function($) {

	/*------------------
		Background Set
	--------------------*/
	$('.set-bg').each(function() {
		var bg = $(this).data('setbg');
		$(this).css('background-image', 'url(' + bg + ')');
	});



	/*------------------
		Hero Slider
	--------------------*/
	$('.hero-slider').owlCarousel({
        loop: true,
        margin: 0,
        nav: true,
        items: 1,
        dots: false,
        mouseDrag: false,
        autoplay: true,
        animateOut: 'fadeOut',
    	animateIn: 'fadeIn',
    	navText: [' ', ''],
    });

})(jQuery);

