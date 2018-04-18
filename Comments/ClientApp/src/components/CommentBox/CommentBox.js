// @flow

import React, {Component, Fragment} from "react";
import { withRouter } from "react-router-dom";
import {load} from "../../data/loader";
import preload from "../../data/pre-loader";
import stringifyObject from "stringify-object";

type PropsType = {
	staticContext?: any,
	type: string,
	commentId: number
}

type CommentType = null | {
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

type StateType = {
	comment: CommentType,
	loading: boolean,
	saving: boolean
};

export class CommentBox extends Component<PropsType, StateType> {

	constructor(props: PropsType) {
		super(props);
		this.state = {
			loading: true,
			saving: false,
			comment: {}
		}
		;

		// const preloaded = preload(this.props.staticContext, "comment", [1], {});
		// const preloaded = preload(this.props.staticContext, "comments", [], { sourceURI: this.props.match.url });

		// if (preloaded) {
		//
		// 	// console.log(`preloaded: ${stringifyObject(preloaded)}`);
		// 	// console.log('setting loading to false');
		//
		// 	this.state = {
		// 		comment: preloaded,
		// 		loading: false,
		// 		saving: false
		// 	};
		// }
	}

	// todo: create a new comment

	componentDidMount() {
		// if (!this.state.comment) {
			load("comment", undefined, [this.props.commentId])
				.then(response => {
					this.setState({
						comment: response.data,
						loading: false
					});
				});
		// }
	}

	commentChangeHandler = e => {
		const comment = this.state.comment;
		comment.commentText = e.target.value;
		this.setState({
			comment
		});
	};

	formSubmitHandler = (e, comment) => {
		e.preventDefault();
		load("comment", undefined, [comment.commentId], {}, "PUT", comment);
		//.then(
		//	res => console.log({res})
		//);
	};

	render() {
		if (this.state.loading) return <p>Loading</p>;
		return (
			<Fragment>
				<li>
					<form onSubmit={(e) => this.formSubmitHandler(e, this.state.comment)}>
						<textarea name="1" id={this.state.comment.commentId} cols="10" rows="10"
								  value={this.state.comment.commentText}
								  onChange={this.commentChangeHandler}/>
						<input type="submit" value="Save"/>
					</form>
				</li>
			</Fragment>
		);
	}

}

export default withRouter(CommentBox);
