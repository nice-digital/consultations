import scroll from "@nice-digital/wdio-cucumber-steps/lib/support/action/scroll";
import selectors from "../selectors";

export const scrollDeleteButtonIntoView = () => {
	scroll(selectors.reviewPage.deletebutton);
};

export default scrollDeleteButtonIntoView;
