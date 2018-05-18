import React, { Component, Fragment } from "react";

import CommentListWithRouter from "../CommentList/CommentList";

export class ReviewPage extends Component {
	constructor() {
		super();
	}


	render() {
		return (
			<Fragment>
				<CommentListWithRouter
					wrappedComponentRef={component => (this.commentList = component)}
				/>
			</Fragment>
		);
	}
}

export default ReviewPage;
