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
			tabIndex: "-1",
			comment: {
				commentText: ""
			},
			ui: {
				unsavedChanges: false
			}
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
			ui: {
				unsavedChanges: true
			}
		});
	};

	static getDerivedStateFromProps(nextProps, prevState) {
		const prevTimestamp = prevState.comment.lastModifiedDate;
		const nextTimestamp = nextProps.comment.lastModifiedDate;
		const hasCommentBeenUpdated = () => prevTimestamp !== nextTimestamp;
		if (hasCommentBeenUpdated()) {
			return {
				comment: nextProps.comment,
				ui: {
					unsavedChanges: false
				}
			};
		}
		return null;
	}

	render() {
		if (!this.state.comment) return null;
		const {
			commentText,
			commentOn,
			lastModifiedDate,
			quote
		} = this.state.comment;
		const tabIndex = this.props.drawerOpen ? "0" : "-1";
		return (
			<Fragment>
				<li className="CommentBox">
					<section>
						<h1 className="CommentBox__title mt--0">
							Comment on: <span className="text-capitalize">{commentOn}</span>
							<br />
							{quote}
						</h1>
						{lastModifiedDate ? (
							<div className="CommentBox__datestamp pv--d font-weight-bold">
								Last Modified Date:{" "}
								<Moment format="D/M/YYYY - h:mma" date={lastModifiedDate} />
							</div>
						) : null}

						<form onSubmit={e => this.props.saveHandler(e, this.state.comment)}>
							<div className="form__group form__group--textarea mb--0">
								<label
									className="form__label visually-hidden"
									htmlFor={this.props.unique}
								>
									Comment
								</label>
								<textarea
									id={this.props.unique}
									tabIndex={tabIndex}
									className="form__input form__input--textarea"
									onChange={this.textareaChangeHandler}
									placeholder="Enter your comment here"
									value={commentText}
								/>
							</div>
							{this.state.comment.commentText &&
								this.state.comment.commentText.length > 0 && (
									<input
										tabIndex={tabIndex}
										className="btn ml--0"
										type="submit"
										value={
											this.state.ui.unsavedChanges ? "Save draft" : "Saved"
										}
										disabled={!this.state.ui.unsavedChanges}
										//
									/>
								)}
							<button
								tabIndex={tabIndex}
								className="btn mr--0 right"
								onClick={e =>
									this.props.deleteHandler(e, this.state.comment.commentId)
								}
							>
								<span className="visually-hidden">Delete this comment</span>
								<span className="icon icon--trash" aria-hidden="true" />
							</button>
						</form>
					</section>
				</li>
			</Fragment>
		);
	}
}

export default CommentBox;
