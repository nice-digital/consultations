import React, {Component, Fragment} from "react";
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

	constructor() {
		super();
		this.state = {
			comment: {},
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
		return {
			ui: {
				unsavedChanges: false
			},
			comment: nextProps.comment
		};
	}

	render() {
		if (!this.state.comment) return null;
		const {commentText} = this.state.comment;
		return (
			<Fragment>
				<li>
					<small>{this.state.ui.unsavedChanges ? "unsaved" : ""}</small>
					<form onSubmit={(e) => this.props.saveHandler(e, this.state.comment)}>
						<textarea
							rows="2"
							value={commentText}
							onChange={this.textareaChangeHandler}
						/>
						<input type="submit" value="Save"/>
					</form>
				</li>
			</Fragment>
		);
	}
}

export default CommentBox;
