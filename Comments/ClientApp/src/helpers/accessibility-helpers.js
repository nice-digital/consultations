/**
 * Pull focus to an element, principally for accessibility
 * @param {string} selection - The CSS-style query selector
 * @param {boolean} scroll - Whether or not to force the element to scroll into view
 */
export const pullFocusByQuerySelector = (selection: string, scroll: boolean) => {
	setTimeout(() => {
		const el = document.querySelector(selection);
		if (el) {
			if (el.getAttribute("tabindex") === null) {
				el.setAttribute("tabindex", "-1");
			}
			el.focus();
			scroll && el.scrollIntoView();
		}
	}, 100);
};
