// @flow

import React, { Component, Fragment } from "react";
import ReactDOM from "react-dom";
import { nodeIsChapter, nodeIsSection, nodeIsSubsection, nodeIsInternalLink } from "../../document-processing/transforms/types";
import htmlparser from "htmlparser2";
import domutils from "domutils"
import ElementType from "domelementtype";

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

	componentDidMount(){
		this.attachEvents();
	}

	parseHtml = (setState: boolean) => {

		let handler=new htmlparser.DomHandler();
		let parser = new htmlparser.Parser(handler);
		parser.write(this.props.html);
		parser.end();
		let dom = handler.dom;

		let chapters = domutils.find(nodeIsChapter, dom, true);

		this.addButton(handler, chapters);

		const convertedHTML = domutils.getOuterHTML(handler.dom);

		if (setState){
			this.setState({convertedHTML})
		}
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

	attachEvents = () => {
		const node = ReactDOM.findDOMNode(this);

		let button = node.querySelector("#uniqueButtonIdGoesHere");

		button.addEventListener('click', this.clickEventHandler);
	};

	clickEventHandler = () => {
		console.log('clicked!');
	};

	addButton = (handler: DomHandler, elementArray: Array<any>) => {

		//todo: refactor
		const spanCommentIconProperties = this.getProperties("span", {
			"class": "icon icon--comment",
			"aria-hidden": "true"
		}, []);
		const spanCommentIcon = handler._createDomElement(spanCommentIconProperties);

		const spanCommentLabelProperties = this.getProperties("span", {
			"class": "visually-hidden",
		}, [{
			  "data": "Comment on {commentOn}: {quote}", //TODO - replacements
			  "type": "text"
			}]);
		const spanCommentLabel = handler._createDomElement(spanCommentLabelProperties);

		var buttonProperties = this.getProperties("button", {
			"data-qa-sel": "in-text-comment-button",
			"class": "document-comment-container__commentButton",
			"tabIndex": "0", //todo: tabindex
			"id": "uniqueButtonIdGoesHere",
			//todo: onclick
		}, [spanCommentIcon, spanCommentLabel]);
		var buttonElement = handler._createDomElement(buttonProperties);

		for (let elementToInsertBefore of elementArray){

			domutils.prepend(elementToInsertBefore, buttonElement);

		}

		// <button
		// 			data-qa-sel="in-text-comment-button"
		// 			className="document-comment-container__commentButton"
		// 			tabIndex={0}
		// 			onClick={e => {
		// 				e.preventDefault();
		// 				onNewCommentClick(e, {
		// 					sourceURI,
		// 					commentText: "",
		// 					commentOn,
		// 					htmlElementID,
		// 					quote,
		// 				});
		// 			}}
		// 		>
		// 			<span className="icon icon--comment" aria-hidden="true" />
		// 			<span className="visually-hidden">Comment on {commentOn}: {quote}</span>
		// 		</button>




	};

	componentDidUpdate(prevProps, prevState){
		 if (prevProps.html !== this.state.originalHTML){
		 	this.setState({originalHTML: prevProps.html});
			this.parseHtml(true);
			this.attachEvents();
		 }		
	}

	render() {
		return (<div dangerouslySetInnerHTML={{__html: this.state.convertedHTML}} />);
	}
}

export default Chapter;
