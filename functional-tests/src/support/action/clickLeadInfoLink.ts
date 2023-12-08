import clickElement from "./clickElement";
import pause from "./pause";


export async function clickLeadInfoLink(): Promise<void> {
	await clickElement("click", "selector", "a[href='/consultations/leadinformation']");
	await pause("2000");
}
