import clickElement from "./clickElement.js";
import pause from "./pause.js";


export async function clickLeadInfoLink(): Promise<void> {
	await clickElement("click", "selector", "a[href='/consultations/leadinformation']");
	await pause("2000");
}
