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
	internalLinkHrefs: Array<string>,
};

export class Chapter extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);
		const convertedHTMLAndinternalLinkHrefs = this.parseHtml();

		this.state = {
			originalHTML: this.props.html,
			convertedHTML: convertedHTMLAndinternalLinkHrefs.html,
			internalLinkHrefs: convertedHTMLAndinternalLinkHrefs.internalLinkHrefs,
		};		
	}

	componentDidMount(){
		this.attachOrDetachEvents(true);
	}

	componentDidUpdate(prevProps, prevState){
		if (prevProps.html !== this.state.originalHTML){
		   	this.attachOrDetachEvents(false);
			this.setState({originalHTML: prevProps.html});
		   	const convertedHTMLAndinternalLinkHrefs = this.parseHtml();
			   this.setState({convertedHTML: convertedHTMLAndinternalLinkHrefs.html,
					internalLinkHrefs: convertedHTMLAndinternalLinkHrefs.internalLinkHrefs,
			}, () => this.attachOrDetachEvents(true));
		}		
   }

	parseHtml = () => {
		let handler = new htmlparser.DomHandler();
		let parser = new htmlparser.Parser(handler);
		parser.write(this.props.html);
		parser.end();
		let dom = handler.dom;

		let chapters = domutils.find(nodeIsChapter, dom, true);
		let sections = domutils.find(nodeIsSection, dom, true);
		let subsections = domutils.find(nodeIsSubsection, dom, true);
		const foundElements = [].concat(chapters).concat(sections).concat(subsections);
		this.addButtons(handler, foundElements);

		const internalLinks = domutils.find(nodeIsInternalLink, dom, true);
		const internalLinkHrefs = this.processInternalLinks(handler, internalLinks);
		
		return { html: domutils.getOuterHTML(handler.dom), 
			internalLinkHrefs: internalLinkHrefs };
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

		for (let buttonElement of elementsWithCommentEventClass){
			if (attach){
				buttonElement.addEventListener("click", this.commentButtonClickEventHandler);
			} else{
				buttonElement.removeEventListener("click", this.commentButtonClickEventHandler);
			}			
		}			

		const internalLinkHrefs = this.state.internalLinkHrefs;
		for (let internalLinkHref of internalLinkHrefs){
			let internalLinkElement = node.querySelector(internalLinkHref);
			if (internalLinkElement != null){ //hmm
				internalLinkElement.addEventListener("click", this.internalLinkClickEventHandler)
			} else{
				console.log("something's wrong. no match for: " + internalLinkHref);
			}
		}
	};

	commentButtonClickEventHandler = (e: Event) => {
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

	internalLinkClickEventHandler = (e: Event) => {
		e.preventDefault();
		let hash = e.srcElement.hash;
		if (document){ //the click event should only ever be called client-side. calling it on the server-side render would be weird, and break without this.
			const target = document.querySelector(this.escapeTheDotsInId(hash));
			if (target){
				target.scrollIntoView();
			} else{
				console.log("something went wrong finding element:" + this.escapeTheDotsInId(hash));
			}			
		}
		//console.log("internal link click. should scroll to:" + hash);
		//console.log(e);
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

	processInternalLinks = (handler: DomHandler, internalLinks: Array<any>) => {
		let linkIds = [];
		for (let internalLink of internalLinks){
			let id = internalLink.attribs["href"];
			let escapedId = this.escapeTheDotsInId(id);
			if (!linkIds.includes(escapedId)){
				linkIds.push(escapedId);
			}
		}
		return linkIds;
	}

	escapeTheDotsInId = (id: string) => {
		//annoyingly some internal link id's have dots in them, and they break querySelector as that interprets dots as a class name. so escaping here.
		return id.replace(/\./g, "\\."); 
	};

	render() {
		return (<div dangerouslySetInnerHTML={{__html: this.state.convertedHTML}} />);
	}
}

export default Chapter;