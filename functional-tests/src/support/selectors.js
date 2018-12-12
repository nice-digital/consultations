const toDataQASelAttr = (attrValue) => `[data-qa-sel='${attrValue}']`;

export default {
	documentPage: {
		commentBoxTitle: toDataQASelAttr("comment-box-title"),
		commentTextArea:toDataQASelAttr("Comment-text-area"),
		submitButton:toDataQASelAttr("submit-button"),
		saveIndicator:".CommentBox:first-child .CommentBox__savedIndicator",
		deletebutton: toDataQASelAttr("delete-comment-button")
	}
};
