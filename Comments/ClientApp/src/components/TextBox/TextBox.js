// import React, { Component, Fragment } from "react";
// import Moment from "react-moment";




// type TextBoxType = {
// 	commentId: number,
// 	lastModifiedDate: Date,
// 	lastModifiedByUserId: string,
// 	text: string,
// 	locationId: number,
// 	sourceURI: string,
// 	htmlElementID: string,
// 	rangeStart: string,
// 	rangeStartOffset: string,
// 	rangeEnd: string,
// 	rangeEndOffset: string,
// 	quote: string,
// 	commentOn: string,
// 	show: boolean
	
// };


// type PropsType = {
// 	staticContext?: any,
// 	isVisible: boolean,
// 	textualResponse: TextBoxType,
// 	readOnly: boolean,
// 	saveHandler: Function,
// 	deleteHandler: Function,
// 	unique: string
// };

// type StateType = {
// 	commentId: number,
// 	commentText: string
// };

// export class TextBox extends Component<PropsType, StateType> {
// 	constructor() {
// 		super();
// 		this.state = {
// 			comment: {
// 				commentText: ""
// 			},
// 			unsavedChanges: false
// 		};
// 	}

// 	componentDidMount() {
// 		this.setState({
// 			comment: this.props.comment
// 		});
// 	}

// 	textareaChangeHandler = e => {
// 		const comment = this.state.comment;
// 		comment.commentText = e.target.value;
// 		this.setState({
// 			comment,
// 			unsavedChanges: true
// 		});
// 	};

// 	static getDerivedStateFromProps(nextProps, prevState) {
// 		const prevTimestamp = prevState.comment.lastModifiedDate;
// 		const nextTimestamp = nextProps.comment.lastModifiedDate;
// 		const hasCommentBeenUpdated = () => prevTimestamp !== nextTimestamp;
// 		if (hasCommentBeenUpdated()) {
// 			return {
// 				comment: nextProps.comment,
// 				unsavedChanges: false
// 			};
// 		}
// 		return null;
// 	}

// 	isTextSelection = (comment) => comment.commentOn && comment.commentOn.toLowerCase() === "selection" && comment.quote;

// 	render() {
// 		if (!this.state.comment) return null;
// 		const {
// 			text,
// 			lastModifiedDate,
// 			quote,
// 			commentId
// 		} = this.state.comment;
// 		const unsavedChanges = this.state.unsavedChanges;
// 		const comment = this.state.comment;
// 		const readOnly = this.props.readOnly;
// 		const moment = require("moment");

// 		return (

// 			<Fragment>
// 				{lastModifiedDate ? (
// 					<div className="CommentBox__datestamp mb--d font-weight-bold">
// 						Last Modified:{" "}
// 						<Moment format="D/M/YYYY - h:mma" date={moment.utc(lastModifiedDate).toDate()}/>
// 					</div>
// 				) : null}
// 				<form onSubmit={e => this.props.saveHandler(e, comment)}>
// 					<div className="form__group form__group--textarea mb--0">
// 						<label
// 							className="form__label visually-hidden"
// 							htmlFor={this.props.unique}>
// 							Comment on {commentOn}, {quote}
// 						</label>
// 						<textarea
// 							data-qa-sel="Comment-text-area"
// 							disabled={readOnly}
// 							id={this.props.unique}
// 							className="form__input form__input--textarea"
// 							onChange={this.textareaChangeHandler}
// 							placeholder="Enter your comment here"
// 							value={text}/>
// 					</div>
// 					{!readOnly && text && text.length > 0 && (
// 						<input
// 							data-qa-sel="submit-button"
// 							className="btn ml--0"
// 							type="submit"
// 							value={unsavedChanges ? "Save comment" : "Saved"}
// 							disabled={!unsavedChanges}/>

// 					)}
// 					{!readOnly &&
// 					<button
// 						data-qa-sel="delete-comment-button"
// 						className="btn mr--0 right"
// 						onClick={e => this.props.deleteHandler(e, commentId)}>
// 						<span className="visually-hidden">Delete this comment</span>
// 						<span className="icon icon--trash" aria-hidden="true"/>
// 					</button>
// 					}
// 				</form>
// 			</Fragment>
// 		);
// 	}
// }

// export default TextBox;