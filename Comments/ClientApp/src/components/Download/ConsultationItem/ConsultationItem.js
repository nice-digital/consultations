import React, { Component } from "react";
import Moment from "react-moment";
import { Link } from "react-router-dom";

export class ConsultationItem extends Component {

	randomStatus = (responses) => {
		if (responses < 30) {
			return <span className="tag tag--flush tag--consultation">Upcoming</span>;
		} else if (responses > 30 && responses < 60){
			return <span className="tag tag--flush tag--consultation">Open</span>;
		}
		return <span className="tag tag--flush tag--updated">Closed</span>;
	};
	
	render() {

		const {
			title,
			endDate,
			startDate,
			reference,
			consultationType,
			supportsQuestions,
			supportsComments,
		} = this.props;

		// fake data yet to come in
		const responses = Math.floor(Math.random() * 100);
		const consultationId = 22;
		const documentId = 1;
		
		return (
			<li className="ConsultationItem">
				<article className="card">
					<header className="card__header">
						<h3 className="card__heading">
							<Link to={`/${consultationId}/${documentId}`}>{title}</Link>	
						</h3>
					</header>
					<dl className="card__metadata">
						<div className="card__metadatum">
							<dt className="visually-hidden">Consultation state</dt>
							<dd>
								{this.randomStatus(responses)}
							</dd>
						</div>
						<div className="card__metadatum">
							<dt className="visually-hidden">Project ID</dt>
							<dd>
								{reference}
							</dd>
						</div>
						<div className="card__metadatum">
							<dd>
								Closes on {" "}
								<Moment format="D MMMM YYYY" date={endDate}/>
							</dd>
						</div>
						<div className="card__metadatum">
							<dd>
								<button
									className="buttonAsLink"
									onClick={() => {alert("downloading...")}}
									title="Download responses">
									Download <strong>{responses}</strong> responses
								</button>
							</dd>
						</div>
					</dl>
				</article>
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
