// @flow

/**
 * Pull focus to an element, principally for accessibility
 * @param {string} selection - The CSS-style query selector
 * @param {boolean} scroll - Whether or not to force the element to scroll into view
 * @param {string} fallback - fallback query selector for another element should the selection above be unavailable
 */
export const pullFocusByQuerySelector = (selection: string, scroll: boolean, fallback: string) => {
	setTimeout(() => {
		const el = document.querySelector(selection) || document.querySelector(fallback);
		if (el) {
			if (el.getAttribute("tabindex") === null) {
				el.setAttribute("tabindex", "-1");
			}
			el.focus();
			el.classList.add("no-outline");
			scroll && el.scrollIntoView();
		}
	}, 100);
};
