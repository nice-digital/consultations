import { render } from "enzyme";
import React, { Component } from "react";
import { withRouter } from "react-router-dom";
import Cookies from "js-cookie";

import { withHistory } from "../HistoryContext/HistoryContext";
import { Breadcrumbs } from "../Breadcrumbs/Breadcrumbs";

type StateType = {
	indevReturnPath: string | null
}

export class LeadInfo extends Component<null, StateType> {
	constructor(props: PropsType) {
		super(props);
		this.state = {
			indevReturnPath: null
		}
	}

	setIndevReturnPath = () => {
		let indevReturnPath = null;

		if (typeof (document) !== "undefined") {
			const documentReferrer = document.referrer;
			if (documentReferrer.toLowerCase().indexOf("indev") !== -1) {
				indevReturnPath = documentReferrer;
				const inTenMinutes = new Date(new Date().getTime() + 10 * 60 * 1000);
				Cookies.set("documentReferrer", documentReferrer, {
					expires: inTenMinutes,
				});
			}
			else {
				const cookieReferrer = Cookies.get("documentReferrer");
				if (cookieReferrer != null) {
					indevReturnPath = cookieReferrer;
				}
			}
		}
		return indevReturnPath;
	};

	

	render() {
		let breadcrumbLinkParams = [
			{
				label: "Home",
				url: "/",
				localRoute: false,
			},
			{
				label: this.state.indevReturnPath ? "Back to InDev" : "All consultations",
				url: this.state.indevReturnPath ? this.state.indevReturnPath : "/guidance/inconsultation",
				localRoute: false,
			},
		];

		const emailAddress = "DITApplicationSupport@nice.org.uk"
		const emailSubject = "New commenting lead request";
		const emailBody = "Dear Sir/Madam";

		return (
			<div className="container">
				<Breadcrumbs links={breadcrumbLinkParams} />

				<div className="grid">
					<div data-g="12">
						<h2>What is a commenting lead?</h2>
						<p>Commenting leads are representatives of an organisation who collate feedback on NICE consultations from colleagues within their organisation. They are also responsible for submitting their organisation's consultation response to NICE.</p>
						<p>Leads are now able to do this online using our commenting system, but will require a set of permissions which can be assigned on request.</p>
					</div>
				</div>

				<div className="grid">
					<div data-g="8">
						
						<h3>How does it work?</h3>
						<p>
							Once you have signed up to be the lead for a consultation, you will be able to:
							<ul>
								<li>share the online consultation with colleagues from your organisation</li>
								<li>review and edit their comments</li>
								<li>combine the collated comments into a single response</li>
								<li>submit your organisation's final response.</li>
							</ul>
							You can be the lead for multiple consultations.
						</p>
						<p>Ideally each consultation will only have one commenting lead. If you need to add a second lead for a consultation then you can, but only one of you will be able to submit the final response to NICE.</p>
						<p>If you need to change the lead during a consultation you can email us. However, if the outgoing lead has already submitted a response then the new lead will not be able to edit it or submit another response.</p>
					</div>
					<div data-g="4">
						<div className="panel">
							<h3>How do I become a lead?</h3>
							<p>Email us with your name and the name of the organisation you will be a lead for.</p>
							<a href={`mailto:${emailAddress}?subject=${emailSubject}&body=${emailBody}`}>Request the commenting lead role</a>
						</div>
					</div>
				</div>
			</div>
		);
	}
}

export default withRouter(withHistory(LeadInfo));
