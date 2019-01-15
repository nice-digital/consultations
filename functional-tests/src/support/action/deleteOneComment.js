import clickElement from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import selectors from "../selectors";

export const deleteOneComment = () => {
	clickElement("click", "button", selectors.documentPage.deletebutton);
};

export default deleteOneComment;
