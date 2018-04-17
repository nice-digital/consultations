// @flow

import React, {Component, Fragment} from "react";
import {load} from "../../data/loader";

export class CommentBox extends Component {

	state = {
		comment: null,
		loading: true,
		saving: false
	};

	// constructor() {
	// 	super();
	// 	// if (typeof(this.props.comment) === undefined)
	// 	 //	console.log("error");
	// 	 	//throw new Error();
	// }
	// // todo: create a new comment

	componentDidMount() {
		if (!this.state.comment) {
			load("comment", undefined, [this.props.comment.commentId], {})
				.then(response => {
					this.setState({
						comment: response.data,
						loading: false
					});
				});
		}
	}

	commentChangeHandler = e => {
		const comment = this.state.comment;
		comment.commentText = e.target.value;
		this.setState({
			comment
		});
	};

	formSubmitHandler = (comment) => {
		load("comment", undefined, [comment.commentId], {}, "PUT", comment);
			//.then(
			//	res => console.log({res})
			//);
	};

	render() {
		const {comment} = this.state;

		if (!comment) {
			return <li>loading comment</li>;
		}

		return (
			<Fragment>
				<li>
					<form onSubmit={()=>this.formSubmitHandler(this.state.comment)}>
						<textarea name="1" id={comment.commentId} cols="10" rows="10"
								  value={comment.commentText}
								  onChange={() => this.commentChangeHandler(this.state.comment)}/>

						<input type="submit" value="Save"/>
					</form>
				</li>
			</Fragment>
		);
	}

}

export default CommentBox;
