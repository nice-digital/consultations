import React, { Component, Fragment } from "react";
import Moment from "react-moment";

type PropsType = {
	staticContext?: any,
	isVisible: boolean
};

type StateType = {
	questionId: number
};

export class Question extends Component<PropsType, StateType> {
	constructor() {
		super();
		this.state = {
			question: {
				questionText: ""
			},
			unsavedChanges: false
		};
	}

	componentDidMount() {
		this.setState({
			question: this.props.question
		});
	}

	// static getDerivedStateFromProps(nextProps, prevState) {
	// 	const prevTimestamp = prevState.comment.lastModifiedDate;
	// 	const nextTimestamp = nextProps.comment.lastModifiedDate;
	// 	const hasCommentBeenUpdated = () => prevTimestamp !== nextTimestamp;
	// 	if (hasCommentBeenUpdated()) {
	// 		return {
	// 			comment: nextProps.comment,
	// 			unsavedChanges: false
	// 		};
	// 	}
	// 	return null;
	// }

	// isTextSelection = (comment) => comment.commentOn && comment.commentOn.toLowerCase() === "selection" && comment.quote;

	render() {
		if (!this.state.question) return null;
		

		return (

			<li className="CommentBox">
				this is a question
			</li>
		);
	}
}

export default Question;
