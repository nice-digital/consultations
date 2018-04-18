// @flow
import React, { Component, Fragment } from "react";
import { withRouter } from "react-router-dom";
import { load } from "./../../data/loader";
import preload from "../../data/pre-loader";
import CommentBoxWithRouter from "../CommentBox/CommentBox";

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

type CommentType = {
	commentId: number,
	type: string
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
		const preloaded = preload(this.props.staticContext, "comments", [], { sourceURI: this.props.match.url });

		if (preloaded) {
			this.state = {
				comments: preloaded.comments,
				loading: false
			};
		}
	}

	loadComments(){
		load("comments", undefined, [], { sourceURI: this.props.match.url })
			.then(res=>{
				this.setState({
					comments: res.data.comments,
					loading: false
				});
			});
	}

	componentDidMount() {
		if (this.state.comments.length === 0){
			this.loadComments();
		}
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

	render() {
		if (this.state.loading) return <p>Loading</p>;
		if (!this.state.loading && (this.state.comments.length === 0)) return <p>No comments</p>;

		return (
			<Fragment>
				<button onClick={this.addCommentHandler} >Add comment</button>
				<ul>
					{this.state.comments.map((comment) => {
						const props = {
							commentType: "comment",
							commentId: comment.commentId
						};
						return (
							<CommentBoxWithRouter key={comment.commentId} {...props} />
						);
					})}
				</ul>
			</Fragment>
		);
	}
}

// const Comment = (props) => {
// 	const comment = props.comment;
// 	return <li>{comment.commentText}</li>;
// };

export default withRouter(CommentList);
