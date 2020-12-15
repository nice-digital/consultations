// @flow

import React, { Component } from "react";
import { withRouter } from "react-router";
import { LiveMessenger } from "react-aria-live";

import DocumentWithRouter from "../Document/Document";
import CommentListWithRouter from "../CommentList/CommentList";

type PropsType = {
	location: {
		pathname: string,
		search: string
	}
};

type StateType = {
	error: {
		hasError: boolean,
		message: string
	}
};

export class DocumentView extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);

		this.state = {
			error: {
				hasError: false,
				message: null,
			},
		};
	}

	newCommentHandler = (e: Event, incomingComment: CommentType) => {
		// this method is passed down to <DocumentWithRouter /> by props.
		// The function is the one we're using from <Drawer />
		this.commentList.newComment(e, incomingComment);
	};

	getDocumentTitle = () => {
		return this.documentComponent.getDocumentTitle();
	}

	render() {
		if (this.state.error.hasError) { throw new Error(this.state.error.message); }

		return (
			<div>
				<LiveMessenger>
					{({announceAssertive, announcePolite}) =>
						<CommentListWithRouter
							announceAssertive={announceAssertive}
							announcePolite={announcePolite}
							wrappedComponentRef={component => (this.commentList = component)}
							getTitleFunction={this.getDocumentTitle} />}
				</LiveMessenger>
				
				{/* Passing the function we're using from <CommentListWithRouter /> to DocWithRouter via props*/}
				<DocumentWithRouter onNewCommentClick={this.newCommentHandler} 
					wrappedComponentRef={component => (this.documentComponent = component)}/>
			</div>
		);
	}
}

export default withRouter(DocumentView);
