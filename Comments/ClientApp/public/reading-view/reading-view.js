var standard = "12 md:6";
var reading = "12 md:7 md:push:2";

function toggleReadingMode(){
	var $documentColumn = $('.documentColumn');
	var $h2 = $('.page-header h2');
	var $body = $('body');

	$('body').toggleClass('readingMode');

	if ($('body').hasClass('readingMode')) {
		console.log('setting standard mode');
		$documentColumn.attr("data-g", reading);
		$h2.attr("data-g", reading);
	} else {
		console.log('setting reading mode');
		$h2.removeAttr("data-g");
		$documentColumn.attr("data-g", standard);
	}
}

$(document).on('click', '#js-reading-mode-toggle', function(){
	toggleReadingMode();
});
