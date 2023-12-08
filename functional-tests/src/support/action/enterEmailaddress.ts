import clickElement from "../action/clickElement";
import pause from "../action/pause";
import setInputField from "../action/setInputField";

export async function enterEmailaddress(emailaddress): Promise<void> {
	await pause("2000");
	await clickElement("click", "selector", "input#emailAddress");
	await setInputField("set", process.env[emailaddress], "input#emailAddress");
	await pause("2000");
};
