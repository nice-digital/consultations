// @flow

import React, { Component, Fragment } from "react";
import ReactDOM from "react-dom";
import { nodeIsChapter, nodeIsSection, nodeIsSubsection, nodeIsInternalLink, nodeIsTypeText, nodeIsSpanTag } from "../../document-processing/transforms/types";
import htmlparser from "htmlparser2";
import domutils from "domutils"
import ElementType from "domelementtype";

type PropsType = {
	html: string,
	newCommentClickFunc: func,
	sourceURI: string,
	allowComments: boolean
};

type StateType = {
	originalHTML: string,
	convertedHTML: string,
};

export class Chapter extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);
		const convertedHTML = this.parseHtml();
		this.state = {
			originalHTML: this.props.html,
			convertedHTML: convertedHTML,
		};		
	}

	componentDidMount(){
		this.attachOrDetachEvents(true);
	}

	parseHtml = () => {

		let handler=new htmlparser.DomHandler();
		let parser = new htmlparser.Parser(handler);
		parser.write(this.props.html);
		parser.end();
		let dom = handler.dom;

		let chapters = domutils.find(nodeIsChapter, dom, true);
		let sections = domutils.find(nodeIsSection, dom, true);
		let subsections = domutils.find(nodeIsSubsection, dom, true);

		const foundElements = [].concat(chapters).concat(sections).concat(subsections);
		this.addButtons(handler, foundElements);

		const convertedHTML = domutils.getOuterHTML(handler.dom);

		return convertedHTML;
	}

	getProperties = (tagName: string, attribs: any, children: any) => {
		return {
			type: ElementType.Tag,
			name: tagName,
			attribs: attribs,
			children: children,
		};		
	};

	attachOrDetachEvents = (attach: bool) => {
		const node = ReactDOM.findDOMNode(this);

		let elementsWithCommentEventClass = [...node.querySelectorAll(".comment-event")];

		for (let element of elementsWithCommentEventClass){
			if (attach){
				element.addEventListener('click', this.clickEventHandler);
			} else{
				element.removeEventListener('click', this.clickEventHandler);
			}			
		}			
	};


	clickEventHandler = (e: Event) => {
		console.log('clicked!');
		const buttonClicked = e.currentTarget;
		const commentOn = buttonClicked.getAttribute("data-comment-on");
		const htmlElementID = buttonClicked.getAttribute("data-html-element-id");
		const quote = buttonClicked.getAttribute("data-quote");

		this.props.newCommentClickFunc(e, {
			sourceURI: this.props.sourceURI,
			commentText: "",
			commentOn,
			htmlElementID,
			quote,
		});
	};

	addButtons = (handler: DomHandler, elementArray: Array<any>) => {		

		for (let elementToInsertBefore of elementArray){

			let commentOn = elementToInsertBefore.attribs["data-heading-type"].toLowerCase();
			let quote = elementToInsertBefore.children.filter(nodeIsTypeText)[0].data;

			if (nodeIsSubsection(elementToInsertBefore)) {
				quote = elementToInsertBefore.children.filter(nodeIsSpanTag)[0].children.filter(nodeIsTypeText)[0].data;
				commentOn = "subsection";
			}

			const htmlElementID = (commentOn === "section" || commentOn === "subsection") ? elementToInsertBefore.attribs.id : "";

			const spanCommentIconProperties = this.getProperties("span", {
				"class": "icon icon--comment",
				"aria-hidden": "true"
			}, []);
			const spanCommentIcon = handler._createDomElement(spanCommentIconProperties);

			const spanCommentLabelProperties = this.getProperties("span", {
				"class": "visually-hidden",
			}, [{
				"data": `Comment on ${commentOn}: ${quote}`,
				"type": "text"
				}]);
			const spanCommentLabel = handler._createDomElement(spanCommentLabelProperties);

			var buttonProperties = this.getProperties("button", {
				"data-qa-sel": "in-text-comment-button",
				"class": "document-comment-container__commentButton comment-event",
				"tabIndex": "0",
				"data-comment-on": commentOn,
				"data-quote": quote,
				"data-html-element-id": htmlElementID,
			}, [spanCommentIcon, spanCommentLabel]);
			var buttonElement = handler._createDomElement(buttonProperties);

			domutils.prepend(elementToInsertBefore, buttonElement);
		}
	};

	componentDidUpdate(prevProps, prevState){
		 if (prevProps.html !== this.state.originalHTML){
			this.attachOrDetachEvents(false);
		 	this.setState({originalHTML: prevProps.html});
			const convertedHTML = this.parseHtml();
			this.setState({convertedHTML}, () => this.attachOrDetachEvents(true));
		 }		
	}

	render() {
		return (<div dangerouslySetInnerHTML={{__html: this.state.convertedHTML}} />);
	}
}

export default Chapter;
