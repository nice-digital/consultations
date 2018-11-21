import React, { Component, Fragment } from "react";
import Moment from "react-moment";
import { Link } from "react-router-dom";


type StateType = {}

type PropsType = {
	title:string,
	startDate : Date,
	endDate : Date,
	submissionCount:number,
	consultationId :number,
	documentId:number,
	chapterSlug :string,
	gidReference :string,
	productTypeName :string,
	isOpen: boolean,
	isClosed: boolean,
	isUpcoming: boolean,
	show: boolean,
	basename: string,
}

export class ConsultationItem extends Component<PropsType, StateType> {

	constructor(props: PropsType) {
		super(props);
	}

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
						{consultationStatus !== "Upcoming" &&
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
					</dl>
				</article>
			</li>
		);
	}
}

export default ConsultationItem;