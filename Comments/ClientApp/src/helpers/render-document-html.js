import React from "react";
import ReactHtmlParser from "react-html-parser";

const sectionCommmentButton = () => {
	return <button>Comment on this section</button>;
};

export const renderDocumentHtml = (incomingHtml) => {

	function myTransformFunction(incomingTree){

		console.log({incomingTree});
		const descendants = incomingTree[0].children;
		const outgoingTree = descendants.map(node => {
			console.log(node);
			if (node && node.attribs && node.attribs.class === "section") {
				const firstChild = node.children[1]; // this is the h3.title
				const sectionCommmentButton = {
					type: "tag",
					name: "button",
					attribs: {},
					children: [
						{
							type: "text",
							data: "Whooooooooooooooo"
						}
					]
				};

				firstChild.children.unshift(sectionCommmentButton);
			}
			return node;
		});
		console.log({outgoingTree});
		return outgoingTree;
	}

	return ReactHtmlParser(incomingHtml, {
		preprocessNodes: myTransformFunction
	});

};
