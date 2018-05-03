import React, { Component } from "react";
import xpathRange from "xpath-range";

type PropsType = {
	
};

type StateType = {
	
};

export class Selection extends Component<PropsType, StateType> {
	constructor() {
		super();
		this.state = {
		};
	}
    
	serialiseRange = (ranges: any, limitingElement: any) => {
		let text = [],
			serializedRanges = [];

		for (var i = 0, len = ranges.length; i < len; i++) {
			var r = ranges[i];
			text.push(this.trim(r.text()));
			serializedRanges.push(r.serialize(limitingElement, ""));
		}

		return {
			quote: text.join(" / "),
			ranges: serializedRanges
		};
	}

	onMouseUp = (event: Event) => {
		if (window && window.getSelection){
			let ranges = []; 
			let selection = window.getSelection();
			
			if (selection.isCollapsed){ //isCollapsed is true when there's no text selected.
				return;
			}

			for (let i = 0; i < selection.rangeCount; i++) {
				let r = selection.getRangeAt(i);
				let browserRange = new xpathRange.Range.BrowserRange(r);
				let normedRange = browserRange.normalize().limit(event.currentTarget); //restrict the range to the current limiting area.
				
				if (normedRange !== null) {
					ranges.push(normedRange);
				}
			}

			if (ranges.length === 0){
				return;
			}

			let serialisedRange = this.serialiseRange(ranges, event.currentTarget);
			
			const firstRange = serialisedRange.ranges.pop();
			
			const comment = { quote: serialisedRange.quote,
				rangeStart: firstRange.start,
				rangeStartOffset: firstRange.startOffset,
				rangeEnd: firstRange.end,
				rangeEndOffset: firstRange.endOffset };

			console.log(comment);
		}		
	}	

	// trim strips whitespace from either end of a string.
	//
	// This usually exists in native code, but not in IE8.
	trim(s: string) {
		if (typeof String.prototype.trim === "function") {
			return String.prototype.trim.call(s);
		} else {
			return s.replace(/^[\s\xA0]+|[\s\xA0]+$/g, '');
		}
	}

	

	render() {
		
		return (
			<span onMouseUp={this.onMouseUp}>
				{this.props.children}
			</span>
		);
	}
}

export default Selection;
