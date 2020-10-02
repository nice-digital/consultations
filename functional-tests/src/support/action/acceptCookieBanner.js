export const acceptCookieBanner = () => {
	var cookieBannerShown = browser.isVisible("body #ccc-content");
	if (cookieBannerShown) {
		browser.click("body #ccc-close");
		browser.pause(2000);
	} else {
		return;
	}
};
export default acceptCookieBanner;
