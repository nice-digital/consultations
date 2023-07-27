export function nodeIsChapter(node){
	const isHeading = ["h1", "h2", "h3", "h4", "h5", "h6"].includes(node.name);
	return (node.name === "a" || isHeading) && node.attribs && node.attribs["data-heading-type"] === "chapter";
}

export function nodeIsSection(node) {
	const isHeading = ["h1", "h2", "h3", "h4", "h5", "h6"].includes(node.name);
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
