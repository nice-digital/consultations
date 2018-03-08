import React, { Component } from "react";
import stickybits from "stickybits";

class CommentPanel extends Component {
	componentDidMount() {
		// stickybits(".js-CommentPanel");
	}

	render() {
		return (
			<div className="CommentPanel js-CommentPanel">
				<p>
					<b>Comment Panel</b>
				</p>
				<p>
					To comment on any content within this chapter, highlight some text
					then click the icon that appears next to your text selection
				</p>
				<textarea name="comment" id="comment" cols="30" rows="10" />
			</div>
		);
	}
}

export default CommentPanel;
