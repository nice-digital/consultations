import React, { Component } from "react";
import xpathRange from "xpath-range";
//import Tooltip from "rc-tooltip";
//import stringifyObject from "stringify-object";

type PropsType = {
	newCommentFunc: Function,
	sourceURI: string
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
			position: {}
		};
		this.selectionContainer = React.createRef();
	}

	getCommentForRange = (limitingElement: any, selection: any) =>{
		let selectionRange = selection.getRangeAt(0);
		let browserRange = new xpathRange.Range.BrowserRange(selectionRange);
		let normedRange = browserRange.normalize().limit(limitingElement); //restrict the range to the current limiting area.

		let quote = this.trim(normedRange.text());
		let serialisedRange = normedRange.serialize(limitingElement, "");

		let comment = { quote: quote,
			rangeStart: serialisedRange.start,
			rangeStartOffset: serialisedRange.startOffset,
			rangeEnd: serialisedRange.end,
			rangeEndOffset: serialisedRange.endOffset,
			sourceURI: this.props.sourceURI,
			placeholder: "Comment on this selected text",
			commentText: "",
			commentOn: "Selection" };

		return(comment);
	}

	onMouseUp = (event: Event) => {

		if (window && window.getSelection){
			const selection = window.getSelection();
			if (selection.isCollapsed || selection.rangeCount < 1){ //isCollapsed is true when there's no text selected.
				this.setState({ toolTipVisible: false });
				return;
			}
			const comment = this.getCommentForRange(event.currentTarget, selection);

			const boundingRectOfContainer = this.selectionContainer.current.getBoundingClientRect();
			const position =
			{
				x: event.pageX - (boundingRectOfContainer.left + document.documentElement.scrollLeft),
			  	y: event.pageY - (boundingRectOfContainer.top + document.documentElement.scrollTop)
			};

			this.setState({ comment, position, toolTipVisible: true });
		} else{
			this.setState({ toolTipVisible: false });
		}
	}

	getElementOffset = (element:any) => {

		var de = document.documentElement;
		var box = element.getBoundingClientRect();
		var top = box.top + window.pageYOffset - de.clientTop;
		var left = box.left + window.pageXOffset - de.clientLeft;
		return { top: top, left: left };
	}


	onButtonClick = (event: Event ) => {
		this.props.newCommentFunc(this.state.comment);
		this.setState({ toolTipVisible: false });
	}


	onVisibleChange = (toolTipVisible) => {
		this.setState({
			toolTipVisible
		});
	}

	// trim strips whitespace from either end of a string.
	//
	// This usually exists in native code, but not in IE8.
	trim = (s: string) => {
		if (typeof String.prototype.trim === "function") {
			return String.prototype.trim.call(s);
		} else {
			return s.replace(/^[\s\xA0]+|[\s\xA0]+$/g, '');
		}
	}

	render() {
		return (
			<div onMouseUp={this.onMouseUp} ref={this.selectionContainer}>
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
		left: position.x,
		top: position.y
	};
	return (
		<div className="selection-container unselectable" style={contentMenuStyle}>
			<button onClick={onButtonClick} className="btn"><span className="icon icon--comment unselectable" aria-hidden="true"></span>&nbsp;&nbsp;Comment</button>
		</div>
	);
};

export default Selection;
