// @flow

import React, { Component, Fragment } from "react";
import { withRouter } from "react-router-dom";
import { Route } from "react-router";
import Helmet from "react-helmet";
import { UserContext } from "../../context/UserContext";
import { LoginBanner } from "../LoginBanner/LoginBanner";
import { ConsultationList } from "./ConsultationList/ConsultationList";
import { Questions } from "./Questions/Questions";
import { Header } from "../Header/Header";
import { Breadcrumbs } from "../Breadcrumbs/Breadcrumbs";

class Admin extends Component {

	render() {

		const fakeLinks = [
			{
				label: "Admin Home",
				url: "/admin",
				localRoute: true,
			},
			{
				label: "Questions",
				url: "/admin/questions",
				localRoute: true,
			}];

		return (
			<Fragment>
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
								<title>Admin home screen</title>
							</Helmet>
							<div className="container">
								<div className="grid">
									<div data-g="12">
										<Breadcrumbs links={fakeLinks}/>
										<Header title="Admin"/>
										<Route exact path="/admin" component={ConsultationList}/>
										<Route exact path="/admin/questions" component={Questions}/>
									</div>
								</div>
							</div>
						</Fragment>
					}
				</UserContext.Consumer>
			</Fragment>
		);
	}

}

export default withRouter(Admin);
