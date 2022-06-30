﻿import React, { Fragment } from "react";
import { withRouter } from "react-router-dom";
import Helmet from "react-helmet";
import { withHistory } from "../HistoryContext/HistoryContext";
import { Breadcrumbs } from "../Breadcrumbs/Breadcrumbs";
import { Header } from "../Header/Header";

export const LeadInfo = () => {
	const emailAddress = "DITApplicationSupport@nice.org.uk";
	const emailSubject = encodeURIComponent("New commenting lead request");
	const emailBody = encodeURIComponent("Dear Application Support\r\rMy name is:\rI want to be a commenting lead for the following organisation:\rCurrent consultation/topic of interest (choose 1 or more current topics; however, your lead role will also apply to other future topics):\r\rKind regards");

	return (
		<Fragment>
			<Helmet>
				<title>What is a commenting lead?</title>
			</Helmet>
			<main role="main">
				<div className="container">
					<div className="grid">
						<div data-g="12">
							<Breadcrumbs links={[{label:"Home",url:"/"},{label:"All consultations", url:"/guidance/inconsultation"}]} />

							<Header title="What is a commenting lead?" />
							<p>Commenting leads are representatives of an organisation who collate feedback on NICE consultations from colleagues within their organisation. They are also responsible for submitting their organisation's consultation response to NICE.</p>
							<p>Leads are now able to do this online using our commenting system but will require a set of permissions to be added to their NICE account, which can be assigned on request. </p>

							<div className="grid">
								<div data-g="8">
									<h2>How does it work?</h2>
									<p>You only need to request the commenting lead permissions once. Then you will be able to:</p>
									<ul>
										<li>share the online consultation with colleagues from your organisation</li>
										<li>review and edit their comments</li>
										<li>combine the collated comments into a single response</li>
										<li>submit your organisation's final response.</li>
									</ul>
									<p>You can only be the lead for one organisation at a time. Once you are lead you can manage multiple consultations simultaneously.</p>
									<p>Ideally each consultation will only have one commenting lead. If you need to add a second lead for a consultation then you can, but only one of you will be able to submit the final response to NICE.</p>
									<p>If you need to change the lead during a consultation you can email us. However, if the outgoing lead has already submitted a response then the new lead will not be able to edit it or submit another response for that consultation.</p>
								</div>
								<div data-g="4">
									<div className="panel">
										<h2>How do I become a lead?</h2>
										<p><a href={`mailto:${emailAddress}?subject=${emailSubject}&body=${emailBody}`}>Email us</a> at DITApplicationSupport@nice.org.uk with:</p>
										<ul>
											<li>your name</li>
											<li>your organisation's name</li>
											<li>area of interest, <a href="https://www.nice.org.uk/guidance/inconsultation" target="_blank" rel="noopener noreferrer">view in consultation guidance list</a></li>
										</ul>
									</div>
								</div>
							</div>
						</div>
					</div>
				</div>
			</main>
		</Fragment>
	);
};

export default withRouter(withHistory(LeadInfo));

