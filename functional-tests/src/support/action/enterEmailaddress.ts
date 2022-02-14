import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import {setInputField} from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField"

export async function enterEmailaddress(emailaddress): Promise<void> {
	await pause("2000");
	await clickElement("click", "selector", "input#emailAddress");
	await setInputField("set", process.env[emailaddress], "input#emailAddress");
	await pause("2000");
};
