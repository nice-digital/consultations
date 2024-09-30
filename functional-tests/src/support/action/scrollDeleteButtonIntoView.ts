import scroll from "../action/scroll.js";
import selectors from "../selectors.js";

export async function scrollDeleteButtonIntoView(): Promise<void> {
	await scroll(selectors.reviewPage.deletebutton);
};

export default scrollDeleteButtonIntoView;
