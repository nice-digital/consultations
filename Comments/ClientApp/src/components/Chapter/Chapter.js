// @flow

import React, { Component, Fragment } from "react";
import ReactDOM from "react-dom";
import { chapterSelector, sectionSelector, subsectionSelector, internalLinkSelector } from "../../document-processing/transforms/types";
import htmlparser from "htmlparser2";
import domutils from "domutils"

type PropsType = {
	html: string,
	newCommentClickFunc: func,
	matchUrl: string,
	allowComment: boolean
};

type StateType = {
	originalHTML: string,
	convertedHTML: string,
};

export class Chapter extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);
		const convertedHTML = this.parseHtml(false);
		this.state = {
			originalHTML: this.props.html,
			convertedHTML: convertedHTML,
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
	
	parseHtml = (setState: boolean) => {

		 var handler=new htmlparser.DomHandler();
		 var parser = new htmlparser.Parser(handler);
		 parser.write(this.props.html);
		 parser.end();

		//let div = document.createElement('div')
		//div.innerHTML = str
		//return div

		const convertedHTML = domutils.getOuterHTML(handler.dom);
// 		let doc = HtmlParser(this.props.html); //new DOMParser().parseFromString(this.props.html, "text/html");

// console.log(doc);

// 		const htmlSections = doc.childNodes[0].childNodes;
// 		const convertedHTML = Object.keys(htmlSections).map((key, i) => {
// 			let el = htmlSections[key];
// 			let contents = [<p>{el.innerHTML}</p>];
// 			if (el.hasAttribute("but")) contents.push(<button>Comment</button>);
// 			return <div key={i}>{contents}</div>;
// 		});

		//let convertedHTML = this.props.html;

		if (setState){
			this.setState({convertedHTML})
		}
		return convertedHTML;
	}


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

		 if (prevProps.html !== this.state.originalHTML){
		 	this.setState({originalHTML: prevProps.html});
		 	this.parseHtml(true);
		 }		

		//const domNode = ReactDOM.findDOMNode(this);

		//let chapters = domNode.querySelectorAll(chapterSelector);

		// for (var chapter of chapters){
			
		// 	if ()


		// }



	}

	render() {
		//return <div>{this.state.convertedHTML}</div>;
		return (<div dangerouslySetInnerHTML={{__html: this.state.convertedHTML}} />);
	}
}

export default Chapter;
