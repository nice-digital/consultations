// @flow

import React, { Component } from "react";
import { withRouter } from "react-router-dom";
import { load } from "./../../data/loader";
import preload from "../../data/pre-loader";
import { CommentBox } from "../CommentBox/CommentBox";
import { LoginBanner } from "./../LoginBanner/LoginBanner";
import { UserContext } from "../../context/UserContext";
import { queryStringToObject } from "../../helpers/utils";
//import stringifyObject from "stringify-object";

type PropsType = {
	staticContext?: any,
	match: {
		url: string,
		params: any
	},
	location: {
		pathname: string
	},
	drawerOpen: boolean,
	isReviewPage: boolean,
	filterByDocument: number
};

type CommentType = {
	commentId: number,
	lastModifiedDate: Date,
	lastModifiedByUserId: string,
	commentText: string,
	locationId: number,
	sourceURI: string,
	htmlElementID: string,
	rangeStart: string,
	rangeStartOffset: string,
	rangeEnd: string,
	rangeEndOffset: string,
	quote: string,
	commentOn: string,
	show: boolean
};

type StateType = {
	comments: Array<CommentType>,
	questions: any,
	loading: boolean
};

export class CommentList extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);
		this.state = {
			comments: [],
			questions: [],
			loading: true
		};
		let preloadedData = {};
		if (this.props.staticContext && this.props.staticContext.preload) {
			preloadedData = this.props.staticContext.preload.data;

			//console.log('setting is authorised to:' + preloadedData.isAuthorised);
			// this.state.isAuthorised = preloadedData.isAuthorised;
		}

		//console.log(`preloadedData: ${stringifyObject(preloadedData)}`);
		const preloaded = preload(
			this.props.staticContext,
			"comments",
			[],
			{ sourceURI: this.props.match.url },
			preloadedData
		);

		if (preloaded) {
			this.state = {
				loading: false,
				comments: preloaded.comments,
				filteredComments: [],
				questions: preloaded.questions
			};
			//console.log(`preloaded.comments: ${preloaded.comments}`);
		}

	}

	loadComments() {
		if (this.props.isReviewPage){
			load("review", undefined, [1], {}) // todo: get current consulation id
		 	.then(				 
		 	 	res => {
		 	 		this.setCommentListState(res);
					});	
		} else{
		 	load("comments", undefined, [], { sourceURI: this.props.match.url }).then(
		 		res => {
		 			this.setCommentListState(res);
		 		});
		}
	}

	setCommentListState = (response: any) => {
		
		const comments = this.filterComments(this.props.location.search, response.data.comments );
		this.setState({
			comments,
			questions: response.data.questions,
			loading: false
			// isAuthorised: res.data.isAuthorised,
			// signInURL: res.data.signInURL
		});
	}

	componentDidMount() {
		this.loadComments();
	}

	componentDidUpdate(prevProps: PropsType) {
		const oldRoute = prevProps.location.pathname + prevProps.location.search;
		const newRoute = this.props.location.pathname + this.props.location.search;
		if (oldRoute !== newRoute) {
			this.setState({
				loading: true
			});
			this.loadComments();
		}
	}

	filterComments = (newSourceURIToFilterBy: string, comments: Array<CommentType>) => {
		let filterBy = queryStringToObject(newSourceURIToFilterBy);
		if (filterBy.sourceURI == null) filterBy = { sourceURI: "" };
		const idsOfFilteredComments = comments.filter(comment => comment.sourceURI.indexOf(filterBy.sourceURI) !== -1).map(comment => comment.commentId);

		const commentsWithFilteredAttr = comments.map(comment => {
			comment.show = idsOfFilteredComments.includes(comment.commentId);
			return comment;
		});

		return commentsWithFilteredAttr;
	}

	newComment(newComment: CommentType) {

		let comments = this.state.comments;
		//negative ids are unsaved / new comments
		let idToUseForNewBox = -1;
		if (comments && comments.length) {
			const existingIds = comments.map(c => c.commentId);
			const lowestExistingId = Math.min.apply(Math, existingIds);
			idToUseForNewBox = lowestExistingId >= 0 ? -1 : lowestExistingId - 1;
		} else {
			comments = [];
		}
		const generatedComment = Object.assign({}, newComment, {
			commentId: idToUseForNewBox
		});
		comments.unshift(generatedComment);
		this.setState({ comments });
	}

	saveCommentHandler = (e: Event, comment: CommentType) => {
		e.preventDefault();

		const isANewComment = comment.commentId < 0;
		const method = isANewComment ? "POST" : "PUT";
		const urlParameters = isANewComment ? [] : [comment.commentId];
		const endpointName = isANewComment ? "newcomment" : "editcomment";

		load(endpointName, undefined, urlParameters, {}, method, comment, true)
			.then(res => {
				if (res.status === 201 || res.status === 200) {
					const index = this.state.comments
						.map(function(comment) {
							return comment.commentId;
						})
						.indexOf(comment.commentId);
					const comments = this.state.comments;
					comments[index] = res.data;
					this.setState({
						comments
					});
				}
			})
			.catch(err => {
				console.log(err);
				if (err.response) alert(err.response.statusText);
			});
	};

	deleteCommentHandler = (e: Event, commentId: number) => {
		e.preventDefault();
		if (commentId < 0) {
			this.removeCommentFromState(commentId);
		} else {
			load("editcomment", undefined, [commentId], {}, "DELETE")
				.then(res => {
					if (res.status === 200) {
						this.removeCommentFromState(commentId);
					}
				})
				.catch(err => {
					console.log(err);
					if (err.response) alert(err.response.statusText);
				});
		}
	};

	removeCommentFromState = (commentId: number) => {
		let comments = this.state.comments;
		comments = comments.filter(comment => comment.commentId !== commentId);
		this.setState({ comments });
	};

	render() {
		const commentsToShow = this.state.comments;
		return (
			<UserContext.Consumer>   
				{ contextValue => {
					if (this.state.loading) return <p>Loading</p>;
					if (contextValue.isAuthorised) {
						if (commentsToShow.length === 0) return <p>No comments yet</p>;
						return (
							<ul className="CommentList list--unstyled">
								{commentsToShow.filter(comment => comment.show).map((comment, idx) => {
									return (
										<CommentBox
											drawerOpen={this.props.drawerOpen}
											key={comment.commentId}
											unique={`Comment${idx}`}
											comment={comment}
											saveHandler={this.saveCommentHandler}
											deleteHandler={this.deleteCommentHandler}
										/>
									);
								})}
							</ul>
						);
					} else {
						return <LoginBanner signInButton={true} currentURL={this.props.match.url} signInURL={contextValue.signInURL} registerURL={contextValue.registerURL}/>;
					}
				}}
			</UserContext.Consumer>
		);
	}
}

export default withRouter(CommentList);
