import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import selectors from "../selectors";

export async function deleteOneComment(): Promise<void> {
	await clickElement("click", "selector", selectors.documentPage.deletebutton);
};

export default deleteOneComment;
