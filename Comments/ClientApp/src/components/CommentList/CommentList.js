// @flow

import React, { Component, Fragment } from "react";
import { withRouter } from "react-router-dom";
import { load } from "./../../data/loader";
import preload from "../../data/pre-loader";
import { CommentBox } from "../CommentBox/CommentBox";

type PropsType = {
	staticContext?: any,
	match: {
		url: string,
		params: any
	},
	location: {
		pathname: string
	}
};
type NewCommentType = {
		type: string,
		//location: "",
		placeholderText: string
}

type CommentType = {
	commentId: number,
	lastModifiedDate: Date,
	lastModifiedByUserId: string, //really a guid
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
	constructor(props: PropsType) {
		super(props);
		this.state = {
			comments: [],
			loading: true
		};
		const preloaded = preload(this.props.staticContext, "comments", [], {
			sourceURI: this.props.match.url
		});

		if (preloaded) {
			this.state = {
				comments: preloaded.comments,
				loading: false
			};
		}
	}

	loadComments() {
		load("comments", undefined, [], { sourceURI: this.props.match.url }).then(
			res => {
				this.setState({
					comments: res.data.comments,
					loading: false
				});
			}
		);
	}

	componentDidMount() {
		if (this.state.comments.length === 0) {
			this.loadComments();
		}
		this.props.setClick(this.newComment.bind(this));
	}

	componentDidUpdate(prevProps: PropsType) {
		const oldRoute = prevProps.location.pathname;
		const newRoute = this.props.location.pathname;
		if (oldRoute !== newRoute) {
			this.setState({
				loading: true
			});
			this.loadComments();
		}
	}

	newComment(newComment: NewCommentType){
		console.log('in new comment');
		let comments = this.state.comments;

		//negative ids are unsaved / new comments
		let idToUseForNewBox = -1;
		if (comments && comments.length) {
			const existingIds = comments.map(c => c.commentId);
			const lowestExistingId = Math.min.apply(Math, existingIds);
			idToUseForNewBox = (lowestExistingId >= 0) ? -1 : lowestExistingId - 1;
		} else{
			comments = [];
		}
		const generatedComment = Object.assign({}, newComment, {commentId: idToUseForNewBox});
		comments.unshift(generatedComment);
		this.setState({ comments });
	}

	saveComment = (e: Event, comment: CommentType) => {
		e.preventDefault();
		//if commentId < 0 - POST

		load("comment", undefined, [comment.commentId], {}, "PUT", comment, true)
			.then((res)=>{
				const index = this.state.comments.map(function(c) {return c.commentId; }).indexOf(comment.commentId);
				const comments = this.state.comments;
				comments[index] = res.data;
				this.setState({
					comments
				});
			});
	}

	render() {
		if (this.state.loading) return <p>Loading</p>;
		if (!this.state.loading && this.state.comments.length === 0)
			return <p>No comments</p>;

		return (
			<Fragment>
				<button onClick={() => this.newComment({})} >Add comment</button>
				<ul>
					{this.state.comments.map((comment) => {
						return (
							<CommentBox
								key={comment.commentId}
								comment={comment}
								saveHandler={this.saveComment}
							/>
						);
					})}
				</ul>
			</Fragment>
		);
	}
}

export default withRouter(CommentList);
