import scroll from "../action/scroll";
import selectors from "../selectors";

export async function scrollDeleteButtonIntoView(): Promise<void> {
	await scroll(selectors.reviewPage.deletebutton);
};

export default scrollDeleteButtonIntoView;
