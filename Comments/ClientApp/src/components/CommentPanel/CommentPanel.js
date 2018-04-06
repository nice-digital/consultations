// @flow

import React, { Component } from "react";

type PropsType = {

}

type StateType = {

}

export class CommentPanel extends Component<PropsType, StateType> {

	state = {
		panelActive: true
	};

	toggleCommentBox = () => {
		const panelActive = this.state.panelActive;

		this.setState({
			panelActive: !panelActive
		});
	};

	render() {
		return (
			<div className={
				"CommentPanel js-CommentPanel " +
					(this.state.panelActive ? "active" : "inactive")
			}>
				<button
					onClick={this.toggleCommentBox}
					className="CommentPanel__toggle js-CommentPanel__toggle">
					{this.state.panelActive ? <span>&raquo;</span> : <span>&laquo;</span>}
				</button>
				<p>Comment on this document</p>
				<textarea name="comment" id="comment" rows="3" />
				<br />
				<button className="btn btn-primary">Add comment</button>
			</div>
		);
	}
}
