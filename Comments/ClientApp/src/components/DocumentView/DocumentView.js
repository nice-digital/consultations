// @flow

import React, { Component, Fragment } from "react";
import { withRouter } from "react-router";

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
				message: null
			}
		};
	}

	newCommentHandler = (e: Event, incomingComment: CommentType) => {
		// this method is passed down to <DocumentWithRouter /> by props.
		// The function is the one we're using from <Drawer />
		this.commentList.newComment(e, incomingComment);
	};


	render() {
		if (this.state.error.hasError) { throw new Error(this.state.error.message); }

		return (
			<Fragment>
				
				<CommentListWithRouter  wrappedComponentRef={component => (this.commentList = component)} />
				
				{/* Passing the function we're using from <CommentListWithRouter /> to DocWithRouter via props*/}
				<DocumentWithRouter onNewCommentClick={this.newCommentHandler} />
			</Fragment>
		);
	}
}

export default withRouter(DocumentView);