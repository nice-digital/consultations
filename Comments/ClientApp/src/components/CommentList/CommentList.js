// @flow

import React, { Component } from "react";
import { withRouter, Link } from "react-router-dom";
import { load } from "./../../data/loader";
import preload from "../../data/pre-loader";
import { CommentBox } from "../CommentBox/CommentBox";
import { LoginBanner } from "./../LoginBanner/LoginBanner";
import { UserContext } from "../../context/UserContext";
import { queryStringToObject, replaceFormat } from "../../helpers/utils";
import {pullFocusById} from "../../helpers/accessibility-helpers";
//import stringifyObject from "stringify-object";

type PropsType = {
	staticContext?: any,
	match: {
		url: string,
		params: any
	},
	location: {
		pathname: string,
		search: string
	},
	isVisible: boolean,
	isReviewPage: boolean,
	filterByDocument: number,
	isSubmmitted: boolean,
	submittedHandler: any //func?
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

type ContextType = any;

export class CommentList extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);
		this.state = {
			comments: [],
			questions: [],
			loading: true,
		};
		let preloadedData = {};
		if (this.props.staticContext && this.props.staticContext.preload) {
			preloadedData = this.props.staticContext.preload.data;
		}

		// if (this.props.isReviewPage){
		// 	const preloaded = preload(
		// 		this.props.staticContext,
		// 		"review",
		// 		[],
		// 		{ sourceURI: this.props.match.url },
		// 		preloadedData
		// 	);
		// } else{
		const preloaded = preload(
			this.props.staticContext,
			"comments",
			[],
			{ sourceURI: this.props.match.url },
			preloadedData
		);
		// }

		if (preloaded) {
			this.state = {
				loading: false,
				comments: preloaded.comments,
				filteredComments: [],
				questions: preloaded.questions
			};
		}
	}

	loadComments() {
		// if (this.props.isReviewPage){
		// 	load("review", undefined, [this.props.match.params.consultationId], {}) // todo: maybe this should us source URI instead of id... need to change feed to do this
		//  	.then(
		//  	 	res => {
		//  	 		this.setCommentListState(res);
		// 			});
		// } else{
		//  	load("comments", undefined, [], { sourceURI: this.props.match.url }).then(
		//  		res => {
		//  			this.setCommentListState(res);
		//  		});
		// }

		let sourceURI = this.props.match.url;
		if (this.props.isReviewPage)
		{
			sourceURI = replaceFormat("/{0}/{1}/{2}", [this.props.match.params.consultationId, 0, "Review"]);

		}
		// console.log(sourceURI);

		load("comments", undefined, [], { sourceURI: sourceURI, isReview: this.props.isReviewPage }).then(
			res => {
				this.setCommentListState(res);
			})
			.catch(err => console.log("load comments in commentlist " + err));
	}

	setCommentListState = (response: any) => {

		const comments = this.filterComments(this.props.location.search, response.data.comments );
		this.setState({
			comments,
			questions: response.data.questions,
			loading: false
		});
	};

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

	submitComments = () => {

		console.log('submit comments in commentlist component called');

		let commentsAndAnswers = {comments: this.state.comments, answers: null}; //todo: answers


		// //const submitResponse = 
		load("submit", undefined, [], {}, "POST", commentsAndAnswers, true)
		.then(res => {
			this.props.submittedHandler();
		})
		.catch(err => {
			console.log(err);
			if (err.response) alert(err.response.statusText);
		});		
	}


	filterComments = (newSourceURIToFilterBy: string, comments: Array<CommentType>) => {
		let filterBy = queryStringToObject(newSourceURIToFilterBy);
		if (filterBy.sourceURI == null) filterBy = { sourceURI: "" };
		const idsOfFilteredComments = comments.filter(comment => comment.sourceURI.indexOf(filterBy.sourceURI) !== -1).map(comment => comment.commentId);

		const commentsWithFilteredAttr = comments.map(comment => {
			comment.show = !idsOfFilteredComments.includes(comment.commentId);
			return comment;
		});

		return commentsWithFilteredAttr;
	};

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
			commentId: idToUseForNewBox,
			show: false
		});
		comments.unshift(generatedComment);
		this.setState({ comments });
		setTimeout(() => {
			pullFocusById(`Comment${idToUseForNewBox}`);
		}, 0);
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
		const commentsToShow = this.state.comments.filter(comment => !comment.show);
		return (
			<UserContext.Consumer>
				{ (contextValue: ContextType) => {
					return (
						<div data-qa-sel="comment-list-wrapper">

							{!this.props.isReviewPage ?
								<div className="grid">
									<h1 data-g="6" id="commenting-panel" className="p">
										Comments panel
									</h1>
									{contextValue.isAuthorised ?
										<p data-g="6">
											<Link
												to={`/${this.props.match.params.consultationId}/review`}
												data-qa-sel="review-all-comments"
												className="right">Review all comments</Link>
										</p> : null
									}
								</div> : null
							}

							{this.state.loading ? <p>Loading...</p> :

								contextValue.isAuthorised ?

									commentsToShow.length === 0 ? <p>No comments yet</p> :

										<ul className="CommentList list--unstyled">
											{commentsToShow.map((comment) => {
												return (
													<CommentBox
														readOnly={this.props.isSubmitted}
														isVisible={this.props.isVisible}
														key={comment.commentId}
														unique={`Comment${comment.commentId}`}
														comment={comment}
														saveHandler={this.saveCommentHandler}
														deleteHandler={this.deleteCommentHandler}
													/>
												);
											})}
										</ul> :

									<LoginBanner
										signInButton={true}
										currentURL={this.props.match.url}
										signInURL={contextValue.signInURL}
										registerURL={contextValue.registerURL}
									/>
							}

						</div>
					);
				}}
			</UserContext.Consumer>
		);
	}
}

export default withRouter(CommentList);
