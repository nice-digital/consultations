// @flow

import React, { Component, Fragment } from "react";
import {load} from "../../data/loader";

export class CommentBox extends Component {

	state = {
		comment: {}
	};

	constructor() {
		super();
		// if (typeof(this.props.comment) === undefined)
		 //	console.log("error");
		 	//throw new Error();
	}

	// loadComments(){
	// 	load("comments", undefined, { sourceURI: this.props.match.url })
	// 		.then(res=>{
	// 			this.setState({
	// 				comments: res.data.comments,
	// 				loading: false
	// 			});
	// 		});
	// }

	componentDidMount() {
		// load("comment", undefined, [this.props.comment.commentId], {})
		// 	.then(response =>{
		// 		console.log(response);
		// 	});
	}

	render() {
		return (
			<Fragment>
				<li>
					authenticated comment insert
				</li>
			</Fragment>
			// <textarea name="1" id={this.props.comment.commentId} cols="10" rows="10"
			// 		  defaultValue={this.props.comment.commentText}/>

		);
	}

}

export default CommentBox;
