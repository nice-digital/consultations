/**
 * Pull focus to an element, principally for accessibility
 * @param {string} selection - The jQuery style selector for the thing you want to pull focus to. If it's not an interactive element, the target will need an attribute of "tabIndex={-1}"
 */
// todo: needs test
export const pullFocus = (selection) => {
	console.log(`Pulling focus to "${selection}"`);
	document.querySelector(selection).focus();
};
