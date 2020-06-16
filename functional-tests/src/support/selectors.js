const toDataQASelAttr = (attrValue) => `[data-qa-sel='${attrValue}']`;
const toNthChildAttr = (attrValue) => `.CommentBox:nth-child(${attrValue})`;
const onlyToNthChildAttr = (attrValue) => `:nth-child(${attrValue})`;
const toChildAndQASel = (childIndex, attrValue) =>
	toNthChildAttr(childIndex) + " " + toDataQASelAttr(attrValue);
const toQAselAndChild = (attrValue, childIndex) =>
	toDataQASelAttr(attrValue) + " > li" + onlyToNthChildAttr(childIndex) + " a";
export default {
	documentPage: {
		pageHeader: ".page-header",
		openQuestionPanel: toDataQASelAttr("open-questions-panel"),
		commentPanel: toDataQASelAttr("comment-panel"),
		commentBoxTitle: toDataQASelAttr("comment-box-title"),
		firstCommentTextArea: toChildAndQASel(1, "Comment-text-area"),
		secondCommentTextArea: toChildAndQASel(2, "Comment-text-area"),
		thirdCommentTextArea: toChildAndQASel(3, "Comment-text-area"),
		commentTextArea: toDataQASelAttr("Comment-text-area"),
		firstCommentTextAreapart2: "#Comment-1",
		submitButton: toDataQASelAttr("submit-button"),
		secondSubmitButton: toChildAndQASel(2, "submit-button"),
		saveIndicator: toNthChildAttr(1) + " " + ".CommentBox__savedIndicator",
		deletebutton: toDataQASelAttr("delete-comment-button"),
		reviewAllButton: toDataQASelAttr("review-all-comments"),
	},
	reviewPage: {
		commentTextArea: toDataQASelAttr("Comment-text-area"),
		firstCommentTextArea: toChildAndQASel(1, "Comment-text-area"),
		secondCommentTextArea: toChildAndQASel(2, "Comment-text-area"),
		thirdCommentTextArea: toChildAndQASel(3, "Comment-text-area"),
		answerYesRepresentOrg: toDataQASelAttr("respond-yes-responding-as-org"),
		answerNoRepresentOrg: toDataQASelAttr("respond-no-responding-as-org"),
		enterOrg: toDataQASelAttr("organisation-name"),
		expressInterestQsYes: toDataQASelAttr("express-interest-yes"),
		expressInterestQsNo: toDataQASelAttr("express-interest-no"),
		answerYesTobacLink: toDataQASelAttr("respond-yes-has-tobac-links"),
		answerNoTobacLink: toDataQASelAttr("respond-no-has-tobac-links"),
		submitResponseButton: toDataQASelAttr("submit-comment-button"),
		reviewSubmittedCommentsButton: toDataQASelAttr("review-submitted-comments"),
		deletebutton: toDataQASelAttr("delete-comment-button"),
		responseSubmittedHeader: toDataQASelAttr("changeable-page-header"),
		submitResponseFeedback: toDataQASelAttr("Submit-response-feedback"),
	},
	adminDownloadPage: {
		filterByGID: toDataQASelAttr("FilerByTitleOrGID"),
		pageResultCount: toDataQASelAttr("admin-download-page-count"),
		cancelFilter: toDataQASelAttr("filter-keyword"),
		numberResultsOnPage: toDataQASelAttr("result-on-the-page-index"),
		paginationSection: toDataQASelAttr("pagination-section"),
		firstPager: toQAselAndChild("pagination-section", 1),
		secondPager: toQAselAndChild("pagination-section", 2),
		nextPager: "body .pagination__pager.next",
		yourResponsesFilter: "body .gtm-topic-list-filter-deselect",
	},
};
