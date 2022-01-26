import {scroll} from "@nice-digital/wdio-cucumber-steps/lib/support/action/scroll";
import selectors from "../selectors";

export async function scrollDeleteButtonIntoView(): Promise<void> {
	await scroll(selectors.reviewPage.deletebutton);
};

export default scrollDeleteButtonIntoView;
