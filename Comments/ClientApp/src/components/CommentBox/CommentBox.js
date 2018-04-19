
import React, { Component, Fragment } from "react";
// import { load } from "../../data/loader";

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

type PropsType = {
	staticContext?: any,
	comment: CommentType
};

type StateType = {
	commentId: number,
	commentText: string
};

export class CommentBox extends Component<PropsType, StateType> {
	// commentChangeHandler = e => {
	// 	const comment = this.state.comment;
	// 	comment.commentText = e.target.value;
	// 	this.setState({
	// 		comment
	// 	});
	// };

	// formSubmitHandler = (e, comment) => {
	// 	e.preventDefault();
	// 	load("comment", undefined, [comment.commentId], {}, "PUT", comment);
	// };

	render() {
		const { commentId, commentText } = this.props.comment;
		const { index } = this.props;
		return (
			<Fragment>
				<li>
					<form>
						<textarea
							id={index}
							rows="2"
							value={commentText}
							onChange={e=>this.props.commentChangeHandler(index, this.props.comment)}
						/>
						<input type="submit" value="Save" />
					</form>
				</li>
			</Fragment>
		);
	}
}

export default CommentBox;
