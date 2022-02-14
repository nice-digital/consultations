export async function clickAllDeleteButtons(): Promise<void> {
		const deleteButtons = await $$("[data-qa-sel='delete-comment-button']");
		for (const button of deleteButtons) {
			await button.click();
		}
	}

	export default clickAllDeleteButtons
