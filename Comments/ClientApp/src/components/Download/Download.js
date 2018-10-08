// @flow

import React, { Component, Fragment } from "react";
import { withRouter } from "react-router-dom";
import Helmet from "react-helmet";
import { UserContext } from "../../context/UserContext";
import { LoginBanner } from "../LoginBanner/LoginBanner";
import { Header } from "../Header/Header";
import { Breadcrumbs } from "../Breadcrumbs/Breadcrumbs";
import ConsultationSampleData from "./ConsultationSampleData";
import { ConsultationItem } from "./ConsultationItem/ConsultationItem";

class Download extends Component {

	render() {

		const fakeLinks = [
			{
				label: "Not",
				url: "/",
				localRoute: true,
			},
			{
				label: "Real",
				url: "/",
				localRoute: true,
			},
			{
				label: "Breadcrumbs",
				url: "/",
				localRoute: true,
			},
		];

		return (
			<UserContext.Consumer>
				{(contextValue: any) => !contextValue.isAuthorised ?
					<LoginBanner
						signInButton={false}
						currentURL={this.props.match.url}
						signInURL={contextValue.signInURL}
						registerURL={contextValue.registerURL}
						signInText="to administer a consultation"
					/>
					:
					<Fragment>
						<Helmet>
							<title>Download Responses</title>
						</Helmet>
						<div className="container">
							<div className="grid">
								<div data-g="12">
									<Breadcrumbs links={fakeLinks}/>
									<Header title="Download Responses"/>
									<div className="grid">
										<div data-g="12 md:3">
											<h2>Filter</h2>
										</div>
										<div data-g="12 md:9">
											<h2>All consultations</h2>
											<ul className="list--unstyled">
												{ConsultationSampleData.map((item, idx) => <ConsultationItem key={idx} {...item} />)}
											</ul>
										</div>
									</div>
								</div>
							</div>
						</div>
					</Fragment>
				}
			</UserContext.Consumer>
		);
	}

}

export default withRouter(Download);
