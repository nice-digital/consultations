// @flow

import React, { Component } from "react";
import { Helmet } from "react-helmet";
import Breadcrumbs from "./../Breadcrumbs/Breadcrumbs";

type PropsType = {};

class FetchData extends Component<PropsType> {
	render() {
		return (
			<div>
				<Helmet>
					<title>Comment on Document</title>
				</Helmet>
				<Breadcrumbs
					segments={[
						{
							label: "Home",
							url: "/document"
						},
						{
							label: "NICE Guidance",
							url: "#"
						},
						{
							label: "In Consulation",
							url: "#"
						},
						{
							label: "Document title",
							url: "#"
						}
					]}
				/>
			</div>
		);
	}
}

export default FetchData;
