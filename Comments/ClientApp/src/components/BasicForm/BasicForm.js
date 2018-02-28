import React, { Component } from "react";
import { connect } from "react-redux";
import { Helmet } from "react-helmet";

import { createComment } from "./formActions";

class BasicForm extends Component {
	constructor(props, context) {
		super(props, context);

		this.state = {
			comment: {
				text: ""
			}
		};
	}

	onTitleChange = ev => {
		const comment = this.state.comment;
		comment.text = ev.target.value;
		this.setState({ comment: comment });
	};

	onClickSave = () => {
		this.props.dispatch(createComment(this.state.comment));
	};

	commentRow = (comment, index) => {
		return <div key={index}>{comment.text}</div>;
	};

	render() {
		return (
			<div>
				<Helmet>
					<title>Basic Form</title>
				</Helmet>

				<h1>Comment page</h1>
				<p>Text: {this.state.comment.text}</p>
				<hr />
				{this.props.comments.map(this.commentRow)}
				<hr />

				<input type="text" onChange={this.onTitleChange} defaultValue={this.state.comment.text} />
				<input type="button" onClick={this.onClickSave} value="Save" />
			</div>
		);
	}
}

function mapStateToProps(state) {
	// this will return the properties we'd like to see exposed as props on our component
	return {
		comments: state.comments
	};
}

// if there's no mapDispatchToProps, you get this.props.dispatch for firing actions
export default connect(mapStateToProps)(BasicForm);
