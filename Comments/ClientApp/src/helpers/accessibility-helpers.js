/**
 * Pull focus to an element, principally for accessibility
 * @param {string} selection - The ***ID ONLY*** If it's not an interactive element, the target will need an attribute of "tabIndex={-1}"
 */
export const pullFocusByQuerySelector = selection => {
	setTimeout(() => {
		const el = document.querySelector(selection);
		if (el) {
			if (el.getAttribute("tabindex") === null) {
				el.setAttribute("tabindex", "-1");
			}
			el.focus();
		}
	}, 100);
};

export const pullFocusById = selection => {
	setTimeout(() => {
		selection = selection.replace("#", ""); // make sure there's no # being passed into here
		const el = document.getElementById(selection);
		if (el) {
			if (el.getAttribute("tabindex") === null) {
				el.setAttribute("tabindex", "-1");
			}
			el.focus();
		}
	}, 100);
};
