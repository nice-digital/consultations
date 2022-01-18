import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import {setInputField} from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField"

export const enterEmailaddress = (emailaddress) => {
	pause("2000");
	clickElement("click", "selector", "input#emailAddress");
	setInputField("set", process.env[emailaddress], "input#emailAddress");
	pause("2000");
};
