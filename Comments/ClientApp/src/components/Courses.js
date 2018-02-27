import React, { Component } from "react";
import { connect } from "react-redux";
import { Helmet } from "react-helmet";

import { createCourse } from "./../actions/courseActions";

class Course extends Component {
	displayName = Course.name;

	constructor(props, context) {
		super(props, context);

		this.state = {
			course: {
				title: ""
			}
		};
	}

	onTitleChange = ev => {
		const course = this.state.course;
		course.title = ev.target.value;
		this.setState({ course });
	};

	onClickSave = () => {
		this.props.dispatch(createCourse(this.state.course));
	};

	courseRow = (course, index) => {
		return <div key={index}>{course.title}</div>;
	};

	render() {
		return (
			<div>
				<Helmet>
					<title>Homepage title rendered with helmet</title>
					<meta name="description" content="This is a meta description render" />
				</Helmet>
				<p>Title: {this.state.course.title}</p>
				<h1>Course page</h1>

				<hr />
				{/* {this.props.courses.map(this.courseRow)} */}
				<hr />

				<input type="text" onChange={this.onTitleChange} defaultValue={this.state.course.title} />
				<input type="button" onClick={this.onClickSave} value="Save" />
			</div>
		);
	}
}

function mapStateToProps(state, ownProps) {
	// this will return the properties we'd like to see exposed as props on our component
	return {
		courses: state.courses
	};
}

// if there's no mapDispatchToProps, you get this.props.dispatch for firing actions
export default connect(mapStateToProps)(Course);
