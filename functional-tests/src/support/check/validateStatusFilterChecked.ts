import pause from "../action/pause";

export async function validateStatusFilterChecked(): Promise<void> {
	await $('#filter_Status_Open').click();
	await pause("2000");
	$('#filter_Status_Closed').click();
	pause("2000");
};
export default validateStatusFilterChecked;
