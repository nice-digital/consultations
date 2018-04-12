// @flow
import React, { Component, Fragment } from "react";
import sampleData from "./sample";
import { load } from "./../../data/loader";

type PropsType = {
	match: {
		url: string
	}
};

type StateType = {
	comments: Array<CommentType>
};
type CommentType = {
	commentId: number,
	lastModifiedDate: Date,
	lastModifiedByUserId: string, //guid..
	commentText: string,
	locationId: number,
	sourceURI: string,
	htmlElementID: string,
	rangeStart: string,
	rangeStartOffset: string,
	rangeEnd: string,
	rangeEndOffset: string,
	quote: string
};

export default class CommentList extends Component<PropsType, StateType> {
	constructor() {
		super();
		this.state = {
			comments: sampleData.comments
		};
	}

	componentDidMount() {
		load("comments", undefined, { monkey: true });
		// load("comments", undefined, { sourceURI: "monkey" });
		// axios.get('/comments')
		// .then(function(response) {
		// 	console.log(response);p
		// });
	}
	
	render() {
		return (
			<Fragment>
				<ul>
					{this.state.comments.map((comment) => {														
						return (
							<Comment key={comment.commentId} comment={comment} />
						);
					})}	
				</ul>
			</Fragment>
		);
	}
}

const Comment = (props) => {
	const comment = props.comment;
	return <li>{comment.commentId}</li>;
};