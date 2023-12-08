import clickElement from "../action/clickElement";
import selectors from "../selectors";

export async function deleteOneComment(): Promise<void> {
	await clickElement("click", "selector", selectors.documentPage.deletebutton);
};

export default deleteOneComment;
