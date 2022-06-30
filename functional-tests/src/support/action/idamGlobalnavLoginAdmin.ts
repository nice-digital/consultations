import idamLogin from "./idamLogin";
import { checkCookieExists } from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkCookieExists";

export async function idamGlobalnavLoginAdmin(
	username: string,
	password: string
): Promise<void> {
	// You're already logged in if you have the nrpa auth cookie, so no need to do anything more
	const accountsAuthCookie = await browser.getCookies("__nrpa_2.2");
	if (accountsAuthCookie.length > 0) return;

	const headerElement = await $("header[aria-label='Site header']");
	await headerElement.waitForDisplayed();

	const mobileMenuButton = await $("#header-menu-button");

	if (await mobileMenuButton.isDisplayed()) {
		// This means we're on a smaller screen size
		await mobileMenuButton.click();

		const accountsLink = await $(
			"body #header-menu a[href*='/consultations/account/login?returnURL=']"
		);
		await accountsLink.click();
	} else {
		await $(
			"body #header-menu-button+* a[href*='/consultations/account/login?returnURL=']"
		);
		await $("body #header-menu-button+* a[href*='/consultations/account/login?returnURL=']").isClickable();
		await browser.pause(2000)
		await $("body #header-menu-button+* a[href*='/consultations/account/login?returnURL=']").click();
		await browser.pause(2000);
	}
	idamLogin(username, password);
};

export default idamGlobalnavLoginAdmin;
