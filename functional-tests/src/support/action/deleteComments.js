/*! https://github.com/webdriverio/cucumber-boilerplate/blob/master/src/support/action/clearInputField.js */
/**
 * Clear a given input field (placeholder for WDIO's clearElement)
 * @param  {String}   element Element selector
 */
module.exports = () => {
	// const findDelete = browser.selectorExecute("[data-qa-sel='delete-comment-button']", function(selectedButtons) {
	// 	return selectedButtons;
	// })
	// console.log(findDelete);
	// browser.click(findDelete);

	if (!browser.isExisting("[data-qa-sel='delete-comment-button']")) return;

	if (browser.isVisibleWithinViewport("[data-qa-sel='comment-list-wrapper']")) {
		clickAllDeleteButtons();
	} else if (
		!browser.isVisibleWithinViewport("[data-qa-sel='delete-comment-button']")
	) {
		browser.scroll("[data-qa-sel='delete-comment-button']");
		clickAllDeleteButtons();
	} else {
		browser.click("[data-qa-sel='open-commenting-panel']");
		browser.pause(1000);
		browser.waitForExist("[data-qa-sel='comment-list-wrapper']");
		clickAllDeleteButtons();
	}

	function clickAllDeleteButtons() {
		const deleteButtons = browser.elements(
			"[data-qa-sel='delete-comment-button']"
		).value;
		browser.waitForExist("[data-qa-sel='delete-comment-button']");
		for (let button of deleteButtons) {
			browser.elementIdClick(button.ELEMENT);
		}
	}

	// browser.click([data-qa-sel='delete-comment-button']);
};
