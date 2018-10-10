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
			startDate,
			endDate,
			submissionCount,
			consultationId,
			documentId,
			chapterSlug,
			gidReference,
			consultationType,
			isOpen,
			isClosed,
			isUpcoming,
		} = this.props;

		const status = (isOpen, isClosed, isUpcoming) => {
			if (isOpen) return "Open";
			if (isClosed) return "Closed";
			if (isUpcoming) return "Upcoming";
			return "?";
		};

		const constultationStatus = status(isOpen, isClosed, isUpcoming);

		return (
			<li className="ConsultationItem">
				<article className="card">
					<header className="card__header">
						<h3 className="card__heading">
							<Link to={`/${consultationId}/${documentId}/${chapterSlug}`}>{title}</Link>
						</h3>
					</header>
					<dl className="card__metadata">
						<div className="card__metadatum">
							<dt className="visually-hidden">Consultation state</dt>
							<dd>
								<span className={`tag tag--${constultationStatus.toLowerCase()}`}>{constultationStatus}</span>
							</dd>
						</div>
						<div className="card__metadatum">
							<dt className="visually-hidden">Project ID</dt>
							<dd title={`Consultation ID ${consultationId}`}>
								{gidReference}
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
								{constultationStatus === "Upcoming" ?
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
						{constultationStatus !== "Upcoming" &&
						<div className="card__metadatum">
							<dd>
								<button
									className="buttonAsLink"
									onClick={() => {
										alert("Will download...");
									}}
									title="Download responses">
									Download <strong>{submissionCount}</strong> responses
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
