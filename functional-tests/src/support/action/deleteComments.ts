/*! https://github.com/webdriverio/cucumber-boilerplate/blob/master/src/support/action/clearInputField.js */
/**
 * Clear a given input field (placeholder for WDIO's clearElement)
 * @param  {String}   element Element selector
 */
export const deleteComments = () => {

	if (!$("[data-qa-sel='delete-comment-button']").isExisting()) return;

	if ($("[data-qa-sel='comment-list-wrapper']").isDisplayedInViewport()) {
		clickAllDeleteButtons();
	} else if (
		!$("[data-qa-sel='delete-comment-button']").isDisplayedInViewport()
	) {
		$("[data-qa-sel='delete-comment-button']").scrollIntoView();
		clickAllDeleteButtons();
	} else {
		$("[data-qa-sel='open-commenting-panel']").click();
		browser.pause(1000);
		$("[data-qa-sel='comment-list-wrapper']").waitForExist();
		clickAllDeleteButtons();
	}

	async function clickAllDeleteButtons() {
		const deleteButtons = await $$("[data-qa-sel='delete-comment-button']");
		await deleteButtons.forEach(element => {
			element.click();
		});
	}
};
