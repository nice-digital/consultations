import React, { Component, Fragment } from "react";
import { Route } from "react-router-dom";

export default class NotFound extends Component {
	render() {
		return (
			<Fragment>
				<Route render={({ staticContext }) => {
					if (staticContext)
						staticContext.status = 404;
					return null;
				}} />
				<h1>Not found</h1>

				<p>Sorry that page could not be found.</p>
			</Fragment>
		);
	}
}
