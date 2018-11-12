const toDataQASelAttr = (attrValue: string) => `[data-qa-sel='${attrValue}']`};

export default {
	documentPage: {
		commentTextArea: toDataQASelAttr("Comment-text-area"),
		submitButton: toDataQASelAttr("submit-button"),
		saveIndicator: ".CommentBox:first-child .CommentBox__savedIndicator"
	}
};
