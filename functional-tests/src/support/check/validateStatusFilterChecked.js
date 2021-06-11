import pause from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";

export const validateStatusFilterChecked = () => {
	browser.click('#filter_Status_Open');
	pause(2000);
	browser.click('#filter_Status_Closed');
	pause(2000);
};
export default validateStatusFilterChecked;
