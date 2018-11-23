// @flow

import React, {Component, Fragment} from "react";
import { withRouter } from "react-router-dom";
import { LoginBanner } from "../LoginBanner/LoginBanner";
import Helmet from "react-helmet";
import { Breadcrumbs } from "../Breadcrumbs/Breadcrumbs";
import { Header } from "../Header/Header";
import { UserContext } from "../../context/UserContext";

export class Questions extends Component {
	render(){

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

		return  (
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
							<title>Set Questions</title>
						</Helmet>
						<div className="container">
							<div className="grid">
								<div data-g="12">
									<Breadcrumbs links={fakeLinks}/>
									<Header title={`Set Questions for consultation ${this.props.match.params.consultationId}`}/>
									<div className="grid">
										<div data-g="12 md:3">
											<p>Left</p>
										</div>
										<div data-g="12 md:9">
											<p>Right</p>
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

export default withRouter(Questions);
