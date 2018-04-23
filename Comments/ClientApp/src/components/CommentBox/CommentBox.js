import React, { Component, Fragment } from "react";

type PropsType = {
	staticContext?: any
};

type StateType = {
	commentId: number,
	commentText: string
};

export class CommentBox extends Component<PropsType, StateType> {

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

	static getDerivedStateFromProps(nextProps) {
		return {
			ui: {
				unsavedChanges: false
			},
			comment: nextProps.comment
		};
	}

	render() {
		if (!this.state.comment) return null;
		const { commentText } = this.state.comment;
		const placeholder = this.state.comment.placeholder ? this.state.comment.placeholder : "Enter your comment here";
		return (
			<Fragment>
				<li>
					<small>{this.state.ui.unsavedChanges ? "unsaved" : ""}</small>
					<form onSubmit={e => this.props.saveHandler(e, this.state.comment)}>
						<textarea
							rows="2"
							value={commentText}
							onChange={this.textareaChangeHandler}
							placeholder={placeholder}
						/>
						<input type="submit" value="Save" />
					</form>
				</li>
			</Fragment>
		);
	}
}

export default CommentBox;
