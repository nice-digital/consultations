// @flow

import React, {PureComponent, Fragment} from "react";
import ReactHtmlParser from "react-html-parser";
import {nodeIsChapter, nodeIsInternalLink, nodeIsSection, nodeIsSubsection} from "./transforms/types";
import {processChapterSectionSubsection, getSectionNumberFromAnchor, getSectionNumberFromParagraph} from "./transforms/chapter-section-subsection";
import processInternalLink from "./transforms/internal-link";

type PropsType = {
	content: any,
	url: string,
	onNewCommentClick: Function,
	allowComments: boolean,
}

type StateType = {
	content: any,
}

export class ProcessDocumentHtml extends PureComponent<PropsType, StateType> {
	constructor(props) {
		super(props);

		if (this.props) {
			const content = ReactHtmlParser(this.props.content, {
				transform: this.transformHtml,
			});
			this.state = {content};
		}
		this.mostRecentSectionNumber = null;
	}
	mostRecentSectionNumber;

	transformHtml = (node) => {
		const isChapter = nodeIsChapter(node);
		const isSection = nodeIsSection(node);
		if (isChapter || isSection || nodeIsSubsection(node)) {
			if (isChapter){
				this.mostRecentSectionNumber = getSectionNumberFromAnchor(node);
			} else if (isSection){ 
				const sectionNumberFromAnchor = getSectionNumberFromAnchor(node);
				if (sectionNumberFromAnchor !== null){
					this.mostRecentSectionNumber = sectionNumberFromAnchor;
				}
			} else{ //node subsection
				const sectionNumberFromParagraph = getSectionNumberFromParagraph(node);
				if (sectionNumberFromParagraph !== null){
					this.mostRecentSectionNumber = sectionNumberFromParagraph;
				}
			}
			return processChapterSectionSubsection(node, this.props.onNewCommentClick, this.props.url, this.props.allowComments, this.mostRecentSectionNumber);
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

	componentDidMount() {
		if (!this.state.content) {
			this.renderHtml();
		}
	}

	componentDidUpdate(prevProps) {
		if (prevProps.slug !== this.props.slug) {
			this.renderHtml();
		}
	}

	render() {
		if (!this.state.content) return null;
		return (
			<Fragment>
				{this.state.content}
			</Fragment>
		);
	}
}
