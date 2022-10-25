// @flow

import React, { Component } from "react";
import { getElementPositionWithinDocument, getSectionTitle } from "../../helpers/utils";
import { tagManager } from "../../helpers/tag-manager";
import { pullFocusByQuerySelector } from "../../helpers/accessibility-helpers";

type PropsType = {
	newCommentFunc: Function,
	sourceURI: string,
	allowComments: boolean
};

type StateType = {
	toolTipVisible: boolean,
	comment: any,
	position: any
};

export class Selection extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);
		this.state = {
			toolTipVisible: false,
			comment: {},
			position: {},
		};
		this.selectionContainer = React.createRef();
	}

	getXPathForElement(element) {
		const idx = (sib, name) => sib
			? idx(sib.previousElementSibling, name||sib.localName) + (sib.localName == name) // eslint-disable-line
			: 1;
		const segs = elm => !elm || elm.nodeType !== 1
			? [""]
			: elm.id && document.querySelector(`#${elm.id}`) === elm
				? [`id("${elm.id}")`]
				: [...segs(elm.parentNode), `${elm.localName.toLowerCase()}[${idx(elm)}]`];
		return segs(element).join("/");
	}

	findClosest(element, selector){
		while (element.parentElement){
			const result = element.querySelector(selector);
			if (result != null){
				return result;
			}
			while (element.previousSibling){
				if (element.previousSibling.matches(selector)){
					return element.previousSibling;
				}
				element = element.previousSibling;
			}
			element = element.parentElement;
		}
		return null;
	}

	getCommentForRange = (limitingElement: any, selection: any) =>{
		let selectionRange = selection.getRangeAt(0);
		let comment = null;
		let sectionNumber = "";
		try {
			const closestCommentButton = this.findClosest(selectionRange.startContainer.parentElement, "button.document-comment-container__commentButton,button.document-comment-container__commentButton");
			if (closestCommentButton != null){
				sectionNumber = closestCommentButton.getAttribute("data-sectionnumber");
			}
			comment = {
				quote: selectionRange.toString(),
				rangeStart: this.getXPathForElement(selectionRange.startContainer.parentElement),
				rangeStartOffset: selectionRange.startOffset,
				rangeEnd: this.getXPathForElement(selectionRange.endContainer.parentElement),
				rangeEndOffset: selectionRange.endOffset,
				sourceURI: this.props.sourceURI,
				placeholder: "Comment on this selected text",
				commentText: "",
				commentOn: "Selection",
				order: getElementPositionWithinDocument(selectionRange.commonAncestorContainer.parentNode) + "." + selectionRange.startOffset.toString(),
				sectionHeader: getSectionTitle(selectionRange.commonAncestorContainer.parentNode),
				sectionNumber: sectionNumber,
			};
		} catch (error) {
			console.error("getCommentForRange", error);
		}
		return(comment);
	};

	onMouseUp = (event: Event) => {
		if (window && window.getSelection) {
			const arrowSize = 10; //this must match the size in $arrow-size in Selection.scss
			const selection = window.getSelection();
			if (selection.isCollapsed || selection.rangeCount < 1){ //isCollapsed is true when there's no text selected.
				this.setState({ toolTipVisible: false });
				return;
			}
			const comment = this.getCommentForRange(event.currentTarget, selection);
			if (comment === null) {
				this.setState({ toolTipVisible: false });
			}
			const scrollTop = "pageYOffset" in window ? window.pageYOffset : document.documentElement.scrollTop;
			const scrollLeft = "pageXOffset" in window ? window.pageXOffset : document.documentElement.scrollLeft;
			const boundingRectOfContainer = this.selectionContainer.current.getBoundingClientRect();
			const position =
			{
				x: event.pageX - (boundingRectOfContainer.left + scrollLeft) - arrowSize,
				y: event.pageY - (boundingRectOfContainer.top + scrollTop) + arrowSize,
			};
			this.setState({ comment, position, toolTipVisible: true });
			setTimeout(() => { pullFocusByQuerySelector(".selection-container button"); }, 0);
		} else{
			this.setState({ toolTipVisible: false });
		}
	};

	onButtonClick = () => {
		this.props.newCommentFunc(null, this.state.comment); //can't pass the event here, as it's the button click event, not the start of the text selection.
		this.setState({ toolTipVisible: false });
		tagManager({
			event: "generic",
			category: "Consultation comments page",
			action: "Clicked",
			label: "Comment on text selection",
		});
	};

	// trim strips whitespace from either end of a string.
	//
	// This usually exists in native code, but not in IE8.
	trim = (s: string) => {
		if (typeof String.prototype.trim === "function") {
			return String.prototype.trim.call(s);
		} else {
			return s.replace(/^[\s\xA0]+|[\s\xA0]+$/g, "");
		}
	};

	componentDidUpdate(prevProps: PropsType){
		// if we're on a different page from when the selection was made, reinitialise the selection
		if (this.props.sourceURI !== prevProps.sourceURI) {
			this.setState({
				toolTipVisible: false,
				comment: {},
				position: {},
			});
		}
	}

	render() {

		if (!this.props.allowComments)
			return (
				<div>{this.props.children}</div>
			);

		return (
			<div onMouseUp={this.onMouseUp} ref={this.selectionContainer} role="presentation">
				<MyToolTip visible={this.state.toolTipVisible} onButtonClick={this.onButtonClick} position={this.state.position}/>
				{this.props.children}
			</div>
		);
	}
}

type ToolTipPropsType = {
	position: any,
	visible: boolean,
	onButtonClick: any
}
export const MyToolTip = (props = ToolTipPropsType) => {
	const { position, visible, onButtonClick } = props;
	var contentMenuStyle = {
		display: visible ? "block" : "none",
		left: position.x || "auto",
		top: position.y || "auto",
	};
	return (
		<div className="selection-container unselectable" style={contentMenuStyle}>
			<button onClick={onButtonClick} className="btn"><span className="icon icon--comment unselectable" aria-hidden="true"></span>&nbsp;&nbsp;Comment</button>
		</div>
	);
};

export default Selection;
