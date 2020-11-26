import React from "react";
import { withRouter } from "react-router-dom";
import { withHistory } from "../HistoryContext/HistoryContext";
import { Breadcrumbs } from "../Breadcrumbs/Breadcrumbs";

export const LeadInfo = () => {
	const emailAddress = "DITApplicationSupport@nice.org.uk"
	const emailSubject = encodeURIComponent("New commenting lead request");
	const emailBody = encodeURIComponent("Dear Sir/Madam\r\rMy name:\rI want to be commenting lead for\r\rKind regards");

	return (
		<div className="container">
			<div className="grid">
				<div data-g="12">
					<Breadcrumbs links={[{label:"Home",url:"/"},{label:"All consultations", url:"/guidance/inconsulation"}]} />

					<h2>What is a commenting lead?</h2>
					<p>Commenting leads are representatives of an organisation who collate feedback on NICE consultations from colleagues within their organisation. They are also responsible for submitting their organisation's consultation response to NICE.</p>
					<p>Leads are now able to do this online using our commenting system, but will require a set of permissions which can be assigned on request.</p>
				
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
								<p>
									<a href={`mailto:${emailAddress}?subject=${emailSubject}&body=${emailBody}`}>Email us</a> at DITApplicationSupport@nice.org.uk with:
									<ul>
										<li>your name</li>
										<li>your organisation's name</li>
									</ul>
								</p>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
	);
}

export default withRouter(withHistory(LeadInfo));