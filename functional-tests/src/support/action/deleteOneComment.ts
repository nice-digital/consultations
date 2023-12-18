import clickElement from "../action/clickElement.js";
import selectors from "../selectors.js";

export async function deleteOneComment(): Promise<void> {
	await clickElement("click", "selector", selectors.documentPage.deletebutton);
};

export default deleteOneComment;
