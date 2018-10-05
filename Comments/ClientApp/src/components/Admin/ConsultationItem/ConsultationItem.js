import React, { Component, Fragment } from "react";
import Moment from "react-moment";

export class ConsultationItem extends Component {

	render() {

		const {
			title,
			endDate,
			startDate,
			reference,
			consultationType,
		} = this.props;

		// fake data for design's sake
		const responsesReceived = Math.round(Math.random() * 100);

		return (
			<li className="ConsultationItem">
				<h3 className="h5">{title}</h3>
				<p>
					<span className="tag tag--flush tag--consultation">Open</span>
				</p>
				<div className="grid">
					<div data-g="9">
						<div>
							<strong><Moment format="D MMMM YYYY" date={startDate}/></strong>
							{" "}to{" "}
							<strong><Moment format="D MMMM YYYY" date={endDate}/></strong>
						</div>
						<div>Received <strong>{responsesReceived}</strong> responses</div>
						<p className="text-muted">{reference} | {consultationType}</p>
					</div>


					<div data-g="3">
						<div>
							<button className="btn btn--cta">Download responses</button>
						</div>
						<div>
							<button className="btn btn--secondary">Set questions</button>
						</div>
					</div>
				</div>
			</li>
		);
	}
}

//  {
//     "reference": "GID-NG10109",
//     "title": "Consultation Comments Test project (Numbered Paragraphs )",
//     "consultationName": "Draft guidance consultation",
//     "startDate": "2018-07-02T00:00:00",
//     "endDate": "2056-12-24T17:00:00",
//     "consultationType": "Draft guidance consultation",
//     "resourceTitleId": null,
//     "projectType": "NG",
//     "productTypeName": "NICE guideline",
//     "developedAs": null,
//     "relevantTo": null,
//     "consultationId": 22,
//     "process": "NG",
//     "hasDocumentsWhichAllowConsultationComments": false,
//     "hasDocumentsWhichAllowConsultationQuestions": true,
//     "supportsQuestions": true,
//     "supportsComments": true,
//     "partiallyUpdatedProjectReference": null,
//     "origProjectReference": null,
//     "user": {
//       "isAuthorised": false,
//       "displayName": null,
//       "userId": null,
//       "organisationName": null
//     },
//     "consultationState": null,
//     "breadcrumbs": null,
//     "filters": null
//   }
