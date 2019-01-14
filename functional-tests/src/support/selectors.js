const toDataQASelAttr = (attrValue) => `[data-qa-sel='${attrValue}']`;
const toNthChildAttr = (attrValue) => `.CommentBox:nth-child(${attrValue})`;
const toChildAndQASel = (childIndex, attrValue) => toNthChildAttr(childIndex) + ' ' + toDataQASelAttr(attrValue);

export default {
	documentPage: {
		commentBoxTitle: toDataQASelAttr("comment-box-title"),
		firstCommentTextArea: toChildAndQASel(1, "Comment-text-area"),
		secondCommentTextArea: toChildAndQASel(2, "Comment-text-area"),
		thirdCommentTextArea: toChildAndQASel(3, "Comment-text-area"),
		commentTextArea:toDataQASelAttr("Comment-text-area"),
		firstCommentTextArea:"#Comment-1",
		submitButton:toDataQASelAttr("submit-button"),
		saveIndicator: toChildAndQASel(1, ".CommentBox__savedIndicator"),
		deletebutton: toDataQASelAttr("delete-comment-button"),
		reviewAllButton: toDataQASelAttr("review-all-comments")
	},
	reviewPage: {
		commentTextArea: toDataQASelAttr("Comment-text-area"),
		firstCommentTextArea: toChildAndQASel(1, "Comment-text-area"),
		secondCommentTextArea: toChildAndQASel(2, "Comment-text-area"),
		thirdCommentTextArea: toChildAndQASel(3, "Comment-text-area"),
		answerNoRepresentOrg: toDataQASelAttr("respond-no-responding-as-org"),
		answerNoTobacLink: toDataQASelAttr("respond-no-has-tobac-links"),
		submitResponseButton: toDataQASelAttr("submit-comment-button"),
		reviewSubmittedCommentsButton: toDataQASelAttr("review-submitted-comments"),
		responseSubmittedHeader: toDataQASelAttr("changeable-page-header")
	}
};
