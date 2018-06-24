/**
 * Pull focus to an element, principally for accessibility
 * @param {string} selection - The ***ID ONLY*** If it's not an interactive element, the target will need an attribute of "tabIndex={-1}"
 */
// todo: needs test
export const pullFocusByQuerySelector = selection => {
	const el = document.querySelector(selection);
	if (el.getAttribute("tabindex") === null) {
		el.setAttribute("tabindex", "-1");
	}
	el.focus();
	console.log(`Pulling focus by querySelector to ${selection}`);
};

export const pullFocusById = selection => {
	selection = selection.replace("#", ""); // make sure there's no # being passed into here
	const el = document.getElementById(selection);
	if (el.getAttribute("tabindex") === null) {
		el.setAttribute("tabindex", "-1");
	}
	el.focus();
	console.log(`Pulling focus by ID to ${selection}`);
};
