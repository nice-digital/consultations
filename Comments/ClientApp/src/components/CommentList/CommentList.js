// @flow
import React, { Component, Fragment } from "react";
import { withRouter } from "react-router-dom";
import { load } from "./../../data/loader";
import preload from "../../data/pre-loader";

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

	componentDidUpdate(prevProps) {
		const oldRoute = prevProps.location.pathname;
		const newRoute = this.props.location.pathname;
		if (oldRoute !== newRoute) {
			this.setState({
				loading: true
			});
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
		if (this.state.loading) return <p>Loading</p>;
		if (!this.state.loading && (this.state.comments.length === 0)) return <p>No comments</p>;
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
	return <li>{comment.commentText}</li>;
};

export default withRouter(CommentList);
