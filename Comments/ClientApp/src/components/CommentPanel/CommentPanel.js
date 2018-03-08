import React, { Component } from "react";
// import stickybits from "stickybits";

class CommentPanel extends Component {
	state = {
		panelActive: true
	};

	toggleCommentBox = () => {
		const panelActive = this.state.panelActive;

		this.setState({
			panelActive: !panelActive
		});
	};

	componentDidMount() {
		// stickybits(".js-CommentPanel");
	}

	render() {
		return (
			<div
				className={
					"CommentPanel js-CommentPanel " +
					(this.state.panelActive ? "active" : "inactive")
				}
			>
				<button
					onClick={this.toggleCommentBox}
					className="CommentPanel__toggle js-CommentPanel__toggle"
				>
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

export default CommentPanel;
