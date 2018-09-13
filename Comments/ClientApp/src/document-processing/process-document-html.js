import React, {PureComponent, Fragment} from "react";
import ReactHtmlParser from "react-html-parser";
import {nodeIsChapter, nodeIsInternalLink, nodeIsSection, nodeIsSubsection} from "./transforms/types";
import {processChapterSectionSubsection} from "./transforms/chapter-section-subsection";
import processInternalLink from "./transforms/internal-link";

export class ProcessDocumentHtmlComponent extends PureComponent {
	constructor(props) {
		super(props);

		if (this.props) {
			const content = ReactHtmlParser(this.props.content, {
				transform: this.transformHtml,
			});
			this.state = {content};
		}
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
		console.log("running renderHtml()");
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
