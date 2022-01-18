export async function acceptCookieBanner(): Promise<void> {
	const cookieBannerShown = await $("body #ccc-content").isDisplayed();
	if (cookieBannerShown) {
		await $("body #ccc-close").click();
		await browser.pause(2000);
	} else {
		return;
	}
};
export default acceptCookieBanner;
