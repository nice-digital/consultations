// @flow

import React, { Component, Fragment } from "react";
import ReactDOM from "react-dom";
import { chapterSelector, sectionSelector, subsectionSelector, internalLinkSelector } from "../../document-processing/transforms/types";

type PropsType = {
	html: string,
	newCommentClickFunc: func,
	matchUrl: string,
	allowComment: boolean
};

type StateType = {
	content: any
};

export class Chapter extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);
		this.state = {
			originalContent: this.props.html,
			content: null,
		};
	}

	// byTag (element: any, tag: string) { return element ? element.getElementsByTagName(tag) : []; }
	// hasClass (element: any, className: string) { 		
	// 	return ((" " + element.className + " ").replace(/[\t\r\n\f]/g, " ").indexOf(` ${className} `) > -1 );
	// }
	// elementsWithClass(elements: Array<any>, className: string){
	// 	let elementsToReturn = [];
	// 	for (let index = 0; index < elements.length; index++){
	// 		elementsToReturn = elementsToReturn.concat(elements[index]);
	// 	}
	// 	return elementsToReturn;
	// }
	// hasAttributeWithValue(element: any, attribute: string, value: string) {
	// 	return (typeof(element[attribute=value]) !== "undefined");
	// }
	// elementsWithAttributeValue(elements: Array<any>, className: string){
	// 	let elementsToReturn = [];
	// 	for (let index = 0; index < elements.length; index++){
	// 		elementsToReturn = elementsToReturn.concat(elements[index]);
	// 	}
	// 	return elementsToReturn;
	// }
	

	// byAttrValue (elementArray: Array<any>, attr: string, value: string) {
	// 	let values = [];
	// 	for (let element of elementArray){
	// 		let elementsForThisElement = element.querySelectorAll(`[${attr}="${value}"]`);
	// 		values = values.concat(...elementsForThisElement);
	// 	}
	// 	return values;
	// } 

	componentDidUpdate(prevProps, prevState){
		console.log("CDU");

		let doc = new DOMParser().parseFromString(this.props.html, "text/html");

		const htmlSections = doc.childNodes[0].childNodes;
		const sections = Object.keys(htmlSections).map((key, i) => {
			let el = htmlSections[key];
			let contents = [<p>{el.innerHTML}</p>];
			if (el.hasAttribute("but")) contents.push(<button>Comment</button>);
			return <div key={i}>{contents}</div>;
		});

		if (prevState.sections != sections){
			this.setState({content: sections});
		}		

		//const domNode = ReactDOM.findDOMNode(this);

		//let chapters = domNode.querySelectorAll(chapterSelector);

		// for (var chapter of chapters){
			
		// 	if ()


		// }



	}

	render() {
		return <div>{this.state.sections}</div>;
		//return (<div dangerouslySetInnerHTML={{__html: this.props.html}} />);
	}
}

export default Chapter;
