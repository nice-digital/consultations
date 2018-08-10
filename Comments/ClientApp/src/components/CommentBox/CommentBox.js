import React, { Component, Fragment } from "react";
import Moment from "react-moment";

type PropsType = {
	staticContext?: any,
	comment: CommentType,
	readOnly: boolean,
	saveHandler: Function,
	deleteHandler: Function,
	unique: string
};

type StateType = {
	commentId: number,
	commentText: string,
	comment: CommentType
};

export class CommentBox extends Component<PropsType, StateType> {
	constructor() {
		super();
		this.state = {
			comment: {
				commentText: ""
			},
			unsavedChanges: false
		};
	}

	componentDidMount() {
		this.setState({
			comment: this.props.comment
		});
	}

	textareaChangeHandler = e => {
		const comment = this.state.comment;
		comment.commentText = e.target.value;
		this.setState({
			comment,
			unsavedChanges: true
		});
	};

	static getDerivedStateFromProps(nextProps, prevState) {
		const prevTimestamp = prevState.comment.lastModifiedDate;
		const nextTimestamp = nextProps.comment.lastModifiedDate;
		const hasCommentBeenUpdated = () => prevTimestamp !== nextTimestamp;
		if (hasCommentBeenUpdated()) {
			return {
				comment: nextProps.comment,
				unsavedChanges: false
			};
		}
		return null;
	}

	isTextSelection = (comment) => comment.commentOn && comment.commentOn.toLowerCase() === "selection" && comment.quote;

	render() {
		if (!this.state.comment) return null;
		const {
			commentText,
			commentOn,
			lastModifiedDate,
			quote,
			commentId,
			order,
		} = this.state.comment;
		const unsavedChanges = this.state.unsavedChanges;
		const comment = this.state.comment;
		const readOnly = this.props.readOnly;
		const moment = require("moment");

		return (

			<li className="CommentBox">
				<section role="form">

					{!this.isTextSelection(comment) &&
					<Fragment>
						<h1 data-qa-sel="comment-box-title" className="CommentBox__title mt--0 mb--d">
							Comment on: <span className="text-lowercase">{commentOn}</span>
							<br/>
							{quote}
						</h1>
					</Fragment>
					}

					{this.isTextSelection(comment) &&
					<Fragment>
						<h1 data-qa-sel="comment-box-title" className="CommentBox__title mt--0 mb--d">
							Comment on: <span className="text-lowercase">{commentOn}</span>
						</h1>
						<div className="CommentBox__quote mb--d">{quote}</div>
					</Fragment>
					}

					{lastModifiedDate ? (
						<div className="CommentBox__datestamp mb--d font-weight-bold">
							Last Modified:{" "}
							<Moment format="D/M/YYYY - h:mma" date={moment.utc(lastModifiedDate).toDate()}/>
						</div>
					) : null}

					{order ? (
						<div className="mb--d font-weight-bold">
							Order: {order}
						</div>
					) : null}
					
					<form onSubmit={e => this.props.saveHandler(e, comment)}>
						<div className="form__group form__group--textarea mb--0">
							<label
								className="form__label visually-hidden"
								htmlFor={this.props.unique}>
								Comment on {commentOn}, {quote}
							</label>
							<textarea
								data-qa-sel="Comment-text-area"
								disabled={readOnly}
								id={this.props.unique}
								className="form__input form__input--textarea"
								onChange={this.textareaChangeHandler}
								placeholder="Enter your comment here"
								value={commentText}/>
						</div>
						{!readOnly && commentText && commentText.length > 0 && (
							<input
								data-qa-sel="submit-button"
								className="btn ml--0"
								type="submit"
								value={unsavedChanges ? "Save comment" : "Saved"}
								disabled={!unsavedChanges}/>

						)}
						{!readOnly &&
						<button
							data-qa-sel="delete-comment-button"
							className="btn mr--0 right"
							onClick={e => this.props.deleteHandler(e, commentId)}>
							<span className="visually-hidden">Delete this comment</span>
							<span className="icon icon--trash" aria-hidden="true"/>
						</button>
						}
					</form>
				</section>
			</li>
		);
	}
}

export default CommentBox;
