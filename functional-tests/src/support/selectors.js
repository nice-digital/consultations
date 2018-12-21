const toDataQASelAttr = (attrValue) => `[data-qa-sel='${attrValue}']`;
const toNthChildAttr = (attrValue) => `.CommentBox:nth-child(${attrValue})`;
const steve = (childIndex, attrValue) => toNthChildAttr(childIndex) + ' ' + toDataQASelAttr(attrValue);

export default {
	documentPage: {
		commentBoxTitle: toDataQASelAttr("comment-box-title"),
		firstCommentTextArea: steve(1, "Comment-text-area"),
		secondCommentTextArea: toNthChildAttr(2) + ' ' + toDataQASelAttr("Comment-text-area"),
		thirdCommentTextArea: toNthChildAttr(3) + ' ' + toDataQASelAttr("Comment-text-area"),
		commentTextArea:toDataQASelAttr("Comment-text-area"),
		firstCommentTextArea:"#Comment-1",
		submitButton:toDataQASelAttr("submit-button"),
		saveIndicator: toNthChildAttr(1) + ' ' + ".CommentBox__savedIndicator",
		deletebutton: toDataQASelAttr("delete-comment-button"),
		reviewAllButton: toDataQASelAttr("review-all-comments")
	},
	reviewPage: {
		commentTextArea: toDataQASelAttr("Comment-text-area"),
		firstCommentTextArea: toNthChildAttr(1) + ' ' + toDataQASelAttr("Comment-text-area"),
		secondCommentTextArea: toNthChildAttr(2) + ' ' + toDataQASelAttr("Comment-text-area"),
		thirdCommentTextArea: toNthChildAttr(3) + ' ' + toDataQASelAttr("Comment-text-area"),
		answerNoRepresentOrg: toDataQASelAttr("respond-no-responding-as-org"),
		answerNoTobacLink: toDataQASelAttr("respond-no-has-tobac-links"),
		submitResponseButton: toDataQASelAttr("submit-comment-button"),
		reviewSubmittedCommentsButton: toDataQASelAttr("review-submitted-comments"),
		responseSubmittedHeader: toDataQASelAttr("changeable-page-header")
	}
};
