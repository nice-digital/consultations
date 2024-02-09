import clickElement from "../action/clickElement.js";
import pause from "../action/pause.js";
import setInputField from "../action/setInputField.js";

export async function enterEmailaddress(emailaddress: string): Promise<void> {
	await pause("2000");
	await clickElement("click", "selector", "input#emailAddress");
	await setInputField("set", process.env[emailaddress], "input#emailAddress");
	await pause("2000");
};
