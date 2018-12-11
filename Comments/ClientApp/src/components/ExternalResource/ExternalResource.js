// @flow

import React, {Component, Fragment} from "react";
import {LiveMessenger} from "react-aria-live";

import { Selection } from "../Selection/Selection";
import CommentListWithRouter from "../CommentList/CommentList";

type PropsType = {
};

type StateType = {
};

export class ExternalResource extends Component<PropsType, StateType> {
	constructor() {
		super();
		this.state = {
		};
	}

	newCommentHandler = (e: Event, incomingComment: CommentType) => {
		// this method is passed down to <DocumentWithRouter /> by props.
		// The function is the one we're using from <Drawer />
		this.commentList.newComment(e, incomingComment);
	};

	
	render() {
		
		return (
			<Fragment>
				<Selection newCommentFunc={this.newCommentHandler}
					sourceURI={"/CommentingOnOtherThings"}
					allowComments={true}/>
				<LiveMessenger>
					{({announceAssertive, announcePolite}) =>
						<CommentListWithRouter
							announceAssertive={announceAssertive}
							announcePolite={announcePolite}
							externalResource={true}
							wrappedComponentRef={component => (this.commentList = component)} />}
				</LiveMessenger>
			</Fragment>
		);
	}
}

export default ExternalResource;
