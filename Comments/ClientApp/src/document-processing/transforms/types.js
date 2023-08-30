export function nodeIsParagraphNumber(node) {
	return node.name === "span" && node.attribs && node.attribs["class"] === "paragraph-number";
}

export function nodeIsNewArticleHeading(node) {
	return /h[1-6]/g.test(node.name);
}

export function nodeIsChapter(node){
	const isHeading = nodeIsNewArticleHeading(node);
	return (node.name === "a" || isHeading) && node.attribs && node.attribs["data-heading-type"] === "chapter";
}

export function nodeIsSection(node) {
	const isHeading = nodeIsNewArticleHeading(node);
	return (node.name === "a" || isHeading) && node.attribs && node.attribs["data-heading-type"] === "section";
}

export function nodeIsInternalLink(node) {
	return node.name === "a" && node.attribs && node.attribs["rel"] === "internal";
}

export function nodeIsSubsection(node) {
	return (node.name === "p" || node.name === "article") && node.attribs && node.attribs["data-heading-type"] === "numbered-paragraph";
}

export function nodeIsTypeText(node) {
	return node.type === "text";
}

export function nodeIsSpanTag(node) {
	return node.name === "span";
}

export function nodeIsComment(node) {
	return node.type === "comment";
}
