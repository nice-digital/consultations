import React, { Component } from "react";

export class CommentPanel extends Component {
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
			<div
				style={this.props.style}
				className={
					"CommentPanel js-CommentPanel " +
					(this.state.panelActive ? "active" : "inactive")
				}
			>
				<button
					onClick={this.toggleCommentBox}
					className="CommentPanel__toggle js-CommentPanel__toggle">
					{this.state.panelActive ? <span>&raquo;</span> : <span>&laquo;</span>}
				</button>
				<p>
					<b>Comment Panel</b>
				</p>
				<p>Comment on this document</p>
				<textarea name="comment" id="comment" cols="30" rows="10" />
				<br />
				<button className="btn btn-primary">Add comment</button>
			</div>
		);
	}
}
