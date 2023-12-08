import {clickAllDeleteButtons} from "./findAndClickDeleteButtons";

export async function deleteCommentsOnReviewPage(): Promise<void> {
	// let commentsNotDisplayed = !!$("[data-qa-sel='comment-box-title']").waitForDisplayed();
	// if (commentsNotDisplayed = true){
	// 	return;
	// } else {
	await $("[data-qa-sel='delete-comment-button']").scrollIntoView();
	// await $("[data-qa-sel='delete-comment-button']").click()
	await clickAllDeleteButtons();
	// async function clickAllDeleteButtons() {
	// 	let deleteButtons = await $$("[data-qa-sel='delete-comment-button']");
	// 	deleteButtons.forEach(element => {
	// 		element.click();
	// 	});
	// }
	};

	// let sidePanelvisible = await $("[data-qa-sel='comment-list-wrapper']").isDisplayedInViewport()
	// if (sidePanelvisible = true) {
	// 	await clickAllDeleteButtons();
	// } else if (
	// 	await !$("[data-qa-sel='delete-comment-button']").isDisplayedInViewport()
	// ) {
	// 	await $("[data-qa-sel='delete-comment-button']").scrollIntoView();
	// 	await clickAllDeleteButtons();
	// } else {

	// async function clickAllDeleteButtons() {
	// 	const deleteButtons = await $$("[data-qa-sel='delete-comment-button']");
	// 	deleteButtons.forEach(element => {
	// 		element.click();
	// 	});
	// };

	export default deleteCommentsOnReviewPage
