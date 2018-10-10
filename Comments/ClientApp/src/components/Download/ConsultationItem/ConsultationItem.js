import React, { Component, Fragment } from "react";
import Moment from "react-moment";
import { Link } from "react-router-dom";

export class ConsultationItem extends Component {

	randomStatus = (responses) => {
		if (responses < 30) {
			return <span className="tag tag--flush tag--consultation">Upcoming</span>;
		} else if (responses > 30 && responses < 60) {
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
		} = this.props;

		function status(startDate, endDate) {
			const now = new Date();
			startDate = new Date(startDate);
			endDate = new Date(endDate);
			if (now < startDate) {
				return "Upcoming";
			} else if (now > startDate && now < endDate) {
				return "Open";
			}
			return "Closed";
		}

		const consultationStatus = status(startDate, endDate);

		// fake data yet to come in
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
								<span className={`tag tag--${consultationStatus.toLowerCase()}`}>{consultationStatus}</span>
							</dd>
						</div>
						<div className="card__metadatum">
							<dt className="visually-hidden">Project ID</dt>
							<dd title={`Consultation ID ${consultationId}`}>
								{reference}
							</dd>
						</div>
						<div className="card__metadatum">
							<dt className="visually-hidden">Consultation Type</dt>
							<dd>
								{consultationType}
							</dd>
						</div>
						<div className="card__metadatum">
							<dd>
								{consultationStatus === "Upcoming" ?
									<Fragment>
										Starts on{" "}
										<strong><Moment format="D MMMM YYYY" date={startDate}/></strong>
									</Fragment>
									:
									<Fragment>
										Closes on{" "}
										<strong><Moment format="D MMMM YYYY" date={endDate}/></strong>
									</Fragment>
								}
							</dd>
						</div>
						{consultationStatus !== "Upcoming" &&
						<div className="card__metadatum">
							<dd>
								<button
									className="buttonAsLink"
									onClick={() => {
										alert("Will download...");
									}}
									title="Download responses">
									Download <strong>{"{quantity}"}</strong> responses
								</button>
							</dd>
						</div>
						}
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
