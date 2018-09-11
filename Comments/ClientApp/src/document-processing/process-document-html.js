// @flow

import React, { PureComponent } from "react";
import ReactHtmlParser from "react-html-parser";
import { nodeIsChapter, nodeIsInternalLink, nodeIsSection, nodeIsSubsection } from "./transforms/types";
import { processChapterSectionSubsection } from "./transforms/chapter-section-subsection";
import processInternalLink from "./transforms/internal-link";

export class ProcessDocumentHtmlComponent extends PureComponent {
	constructor(){
		super();
		this.state = {
			content: "",
		};
	}

	transformHtml = (node) => {
		if (nodeIsChapter(node) || nodeIsSection(node) || nodeIsSubsection(node)) {
			return processChapterSectionSubsection(node, this.props.onNewCommentClick, this.props.sourceURI, this.props.allowComments);
		}
		if (nodeIsInternalLink(node)) {
			return processInternalLink(node);
		}
	};

	renderHtml = () => {
		const content = ReactHtmlParser(this.props.content, {
			transform: this.transformHtml,
		});
		this.setState({content});
	};

	componentDidMount(){
		this.renderHtml();
	}

	componentDidUpdate(prevProps){
		if (prevProps.content !== this.props.content) {
			this.renderHtml();
		}
	}

	render() {
		return (
			<div>
				{this.state.content}
			</div>
		);
	}
}
