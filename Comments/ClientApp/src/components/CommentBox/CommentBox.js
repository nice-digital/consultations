// @flow

import React, {Component, Fragment} from "react";

type PropsType = {
	staticContext?: any,
	comment: CommentType,
	readOnly: boolean,
	saveHandler: Function,
	deleteHandler: Function,
	unique: string,
	updateUnsavedCounter: Function,
};

type StateType = {
	comment: CommentType,
	unsavedChanges: boolean,
};

export class CommentBox extends Component<PropsType, StateType> {
	constructor() {
		super();
		this.state = {
			comment: {},
			unsavedChanges: false,
		};
	}

	componentDidMount() {
		this.setState({
			comment: this.props.comment,
		});
	}

	componentDidUpdate(prevProps: PropsType){
		const nextTimestamp = this.props.comment.lastModifiedDate;
		const prevTimestamp = prevProps.comment.lastModifiedDate;
		const hasCommentBeenUpdated = () => prevTimestamp !== nextTimestamp;
		if (hasCommentBeenUpdated()){
			this.props.updateUnsavedCommentIds(this.props.comment.commentId, false);
		}
	}

	textareaChangeHandler = (e: any) => {
		const comment = this.state.comment;
		comment.commentText = e.target.value;
		this.setState({
			comment,
			unsavedChanges: true,
		});
		this.props.updateUnsavedCommentIds(comment.commentId, true);
	};

	static getDerivedStateFromProps(nextProps: any, prevState: any) {
		const prevTimestamp = prevState.comment.lastModifiedDate;
		const nextTimestamp = nextProps.comment.lastModifiedDate;
		const hasCommentBeenUpdated = () => prevTimestamp !== nextTimestamp;
		if (hasCommentBeenUpdated()) {
			return {
				comment: nextProps.comment,
				unsavedChanges: false,
			};
		}
		return null;
	}

	isTextSelection = (comment: CommentType) => comment.commentOn && comment.commentOn.toLowerCase() === "selection" && comment.quote;

	render() {
		if (!this.state.comment) return null;
		const {
			commentText,
			commentOn,
			quote,
			commentId,
		} = this.state.comment;
		const unsavedChanges = this.state.unsavedChanges;
		const comment = this.state.comment;
		const readOnly = this.props.readOnly;
		return (
				<li className={unsavedChanges ? "CommentBox CommentBox--unsavedChanges" : "CommentBox"}>
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

						<form onSubmit={e => this.props.saveHandler(e, comment)} className="mb--0">
							<div className="form__group form__group--textarea mb--b">
								<label
									className="form__label visually-hidden"
									htmlFor={this.props.unique}>
									Comment on {commentOn}, {quote}
								</label>
								{unsavedChanges &&
								<p className="CommentBox__validationMessage">You have unsaved changes</p>
								}
								<textarea
									data-qa-sel="Comment-text-area"
									disabled={readOnly}
									id={this.props.unique}
									className="form__input form__input--textarea"
									onChange={this.textareaChangeHandler}
									placeholder="Enter your comment here"
									tabIndex={0}
									value={commentText}/>
							</div>
							{!readOnly && commentText && commentText.length > 0 ?
								unsavedChanges ?
									<input
										data-qa-sel="submit-button"
										className="btn ml--0 mb--0"
										type="submit"
										value="Save comment"
									/>
									:
									<span className="ml--0 mb--0 CommentBox__savedIndicator">Saved</span>
								:
								null
							}
							{!readOnly &&
							<button
								data-qa-sel="delete-comment-button"
								className="btn mr--0 mb--0 right"
								onClick={e => this.props.deleteHandler(e, commentId)}>
								Delete
							</button>
							}
						</form>
					</section>
				</li>
		);
	}
}

export default CommentBox;
