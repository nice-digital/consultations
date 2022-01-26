export async function acceptCookieBanner(): Promise<void> {
	const cookieBannerShown = await $("body #ccc-content");
	await cookieBannerShown.waitForExist({ timeout: 2000 });
	const acceptCookiesButtonElement = await cookieBannerShown.$("body #ccc-close");
    // If cookies have already been chosen then the accept button doesn't show
    if (await acceptCookiesButtonElement.isDisplayed()) {
        await acceptCookiesButtonElement.click();
    }
};
export default acceptCookieBanner;
