import clickElement from "../action/clickElement.js";
import pause from	"../action/pause.js";

export async function logout(): Promise<void> {
	await pause("2000");
	await clickElement("click", "selector", "#my-account-button");
	await pause("2000")
	// await clickElement("click", "selector", ".gn_2nlnx");
	await clickElement("click", "selector", "a[href='/consultations/account/logout']");
	await pause("2000");
};
