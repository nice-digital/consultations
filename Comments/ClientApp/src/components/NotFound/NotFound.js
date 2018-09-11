import React, { Component, Fragment } from "react";
import { Route } from "react-router-dom";

export default class NotFound extends Component {
	render() {
		return (
			<Fragment>
				<Route
					render={({ staticContext }) => {
						if (staticContext) staticContext.status = 404;
						return null;
					}}
				/>
				<div className="container">
					<div className="panel page-header">
						<h1 className="heading mt--c">We can't find this page</h1>
						<p className="lead">It's probably been moved, updated or deleted.</p>
						<p><a href="/guidance/inconsultation">Back to consultations</a></p>
					</div>
				</div>
			</Fragment>
		);
	}
}
