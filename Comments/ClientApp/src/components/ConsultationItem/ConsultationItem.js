// @flow

import React, { Component, Fragment } from "react";
import Moment from "react-moment";
import { Link } from "react-router-dom";

import { GenerateCode } from "../GenerateCode/GenerateCode";

type StateType = {}

type PropsType = {
	title: string,
	startDate: Date,
	endDate: Date,
	submissionCount: number,
	consultationId: number,
	documentId: number,
	chapterSlug: string,
	gidReference: string,
	productTypeName: string,
	hasCurrentUserEnteredCommentsOrAnsweredQuestions: boolean,
	hasCurrentUserSubmittedCommentsOrAnswers: boolean,
	isOpen: boolean,
	isClosed: boolean,
	isUpcoming: boolean,
	organisationCodes: Array,
	showShareWithOrganisationButton: boolean,
	show: boolean,
	basename: string,
	submissionToLeadCount: number,
	hidden: boolean
}

export class ConsultationItem extends Component<PropsType, StateType> {

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
			productTypeName,
			hasCurrentUserEnteredCommentsOrAnsweredQuestions,
			hasCurrentUserSubmittedCommentsOrAnswers,
			isOpen,
			isClosed,
			isUpcoming,
			organisationCodes,
			showShareWithOrganisationButton,
			submissionToLeadCount,
			hidden,
		} = this.props;

		const status = (isOpen, isClosed, isUpcoming) => {
			if (isOpen) return "Open";
			if (isClosed) return "Closed";
			if (isUpcoming) return "Upcoming";
			return "?";
		};

		const userHasRespondedButNotSubmitted = hasCurrentUserEnteredCommentsOrAnsweredQuestions & !hasCurrentUserSubmittedCommentsOrAnswers;
		const consultationStatus = status(isOpen, isClosed, isUpcoming);

		return (
			<li className="ConsultationItem">
				<article className="card">
					<header className="card__header">
						<h3 className="card__heading">
							{
								documentId != null && chapterSlug != null ?
									<Link to={`/${consultationId}/${documentId}/${chapterSlug}`}>{title}</Link>
									: title
							}
						</h3>
					</header>
					<dl className="card__metadata">
						<div className="card__metadatum">
							<dt className="visually-hidden">Consultation state</dt>
							<dd>
								<span className={`tag tag--${consultationStatus.toLowerCase()}`}>{consultationStatus}</span>
							</dd>
						</div>
						{Boolean(hidden) && (
							<div className="card__metadatum">
								<dt className="visually-hidden">Consultation state</dt>
								<dd>
									<span className={"tag tag--hidden" }>Hidden</span>
								</dd>
							</div>
						)}
						{Boolean(userHasRespondedButNotSubmitted) && (
							<div className="card__metadatum">
								<dt className="visually-hidden">Unsubmitted questions or answers</dt>
								<dd>
									<span className="tag tag--unsubmitted">Unsubmitted</span>
								</dd>
							</div>
						)}
						{showShareWithOrganisationButton && submissionToLeadCount !== null &&
							<div className="card__metadatum">
								{submissionToLeadCount} {submissionToLeadCount === 1 ? "response" : "responses"} from your organisation
							</div>
						}
						<div>
							<div className="card__metadatum">
								<dt className="visually-hidden">Project ID</dt>
								<dd title={`Consultation ID ${consultationId}`}>
									{gidReference}
								</dd>
							</div>
							<div className="card__metadatum">
								<dt className="visually-hidden">Product type name</dt>
								<dd>
									{productTypeName}
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
							{((consultationStatus !== "Upcoming") && (submissionCount !== null)) &&
								<div className="card__metadatum">
									<dd>
										{submissionCount > 0 ?
											<a href={`${this.props.basename}/api/Export/${this.props.consultationId}`} target="_blank" rel="noopener noreferrer">
												Download <strong>{submissionCount}</strong> response{submissionCount > 1 ? "s" : ""}
											</a>
											:
											<span>No responses</span>
										}
									</dd>
								</div>
							}
						</div>
					</dl>
					{showShareWithOrganisationButton &&
						<GenerateCode organisationCodes={organisationCodes} consultationId={consultationId} />
					}
				</article>
			</li>
		);
	}
}

export default ConsultationItem;
