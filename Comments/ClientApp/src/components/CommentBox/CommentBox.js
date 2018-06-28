import React, { Component, Fragment } from "react";
import Moment from "react-moment";

type PropsType = {
	staticContext?: any,
	drawerOpen: boolean
};

type StateType = {
	commentId: number,
	commentText: string
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
			commentId
		} = this.state.comment;
		const unsavedChanges = this.state.unsavedChanges;
		const comment = this.state.comment;
		const readOnly = this.props.readOnly;
		const tabIndex = this.props.drawerOpen ? "0" : "-1";

		return (

			<li className="CommentBox">
				<section>

					{!this.isTextSelection(comment) &&
					<Fragment>
						<h1 className="CommentBox__title mt--0 mb--d">
							Comment on: <span className="text-lowercase">
								{commentOn === "numbered-paragraph" ? "Numbered paragraph" : commentOn}
							</span>
							<br/>
							{quote}
						</h1>
					</Fragment>
					}

					{this.isTextSelection(comment) &&
					<Fragment>
						<h1 className="CommentBox__title mt--0 mb--d">
							Comment on: <span className="text-lowercase">{commentOn}</span>
						</h1>
						<div className="CommentBox__quote mb--d">{quote}</div>
					</Fragment>
					}

					{lastModifiedDate ? (
						<div className="CommentBox__datestamp mb--d font-weight-bold">
							Last Modified Date:{" "}
							<Moment format="D/M/YYYY - h:mma" date={lastModifiedDate}/>
						</div>
					) : null}
					<form onSubmit={e => this.props.saveHandler(e, comment)}>
						<div className="form__group form__group--textarea mb--0">
							<label
								className="form__label visually-hidden"
								htmlFor={this.props.unique}
							>
								Comment
							</label>
							<textarea
								disabled={readOnly}
								id={this.props.unique}
								tabIndex={tabIndex}
								className="form__input form__input--textarea"
								onChange={this.textareaChangeHandler}
								onKeyUp={this.textareaChangeHandler}
								placeholder="Enter your comment"
								value={commentText}
							/>
						</div>
						{!readOnly && commentText && commentText.length > 0 && (
							<input
								tabIndex={tabIndex}
								className="btn ml--0"
								type="submit"
								value={unsavedChanges ? "Save comment" : "Saved"}
								disabled={!unsavedChanges}
							/>
						)}
						{!readOnly &&
						<button
							tabIndex={tabIndex}
							className="btn mr--0 right"
							onClick={e => this.props.deleteHandler(e, commentId)}
						>
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
