// @flow
import React, { Component, Fragment } from "react";
import { withRouter } from "react-router-dom";
import sampleData from "./sample";
import { load } from "./../../data/loader";
import preload from "../../data/pre-loader";

import stringifyObject from "stringify-object";

type PropsType = {
	staticContext?: any,
	match: {
		url: string,
		params: any
	}
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
type StateType = {
	comments: Array<CommentType>,
	loading: boolean
};

export class CommentList extends Component<PropsType, StateType> {
	constructor(props) {
		super(props);
		this.state = {
			comments: [],
			loading: true
		};
		const preloaded = preload(this.props.staticContext, "comments", { sourceURI: this.props.match.url });

		if (preloaded) {
			// console.log(`setting comments to: ${preloaded}`);
			console.log(`data is: ${stringifyObject(preloaded)}`);
			this.state = { comments: preloaded.comments, loading: false };
		}
	}

	componentDidMount() {
		if (this.state.comments.length === 0){
			load("comments", undefined, { sourceURI: this.props.match.url })
				.then(res=>{
					this.setState({
						comments: res.data.comments,
						loading: false
					});
				});
		}
	}
	
	render() {
		if (this.state.loading) return <p>loading!!!</p>;
		if (this.state.comments.length === 0) return <p>No comments in array</p>;
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

export default withRouter(CommentList);
