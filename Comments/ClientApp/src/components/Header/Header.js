// @flow

import React, { Fragment, PureComponent } from "react";
import Moment from "react-moment";
import { Link } from "react-router-dom";
import { Alert } from "@nice-digital/nds-alert";

type PropsType = {
	title: string,
	endDate?: any,
	match?: any,
	subtitle1?: string,
	subtitle2?: string,
	consultationState?: {
		endDate: string,
		consultationIsOpen: boolean,
		consultationHasNotStartedYet?: boolean,
	},
}

export class Header extends PureComponent<PropsType> {

	render() {
		const {
			title,
			subtitle1,
			subtitle2,
		} = this.props;

		let startDate, endDate, isOpen, notStartedYet;

		if (this.props.consultationState) {
			startDate = this.props.consultationState.startDate || "";
			endDate = this.props.consultationState.endDate || "";
			isOpen = this.props.consultationState.consultationIsOpen || "";
			notStartedYet = this.props.consultationState.consultationHasNotStartedYet || "";
		}

		let startOrEnd = "ended";
		let showStartDate = false;

		if (notStartedYet) {
			startOrEnd = "starts";
			showStartDate = true;
		}

		return (
			<Fragment>
				<h1 data-qa-sel="changeable-page-header" className="page-header__heading mt--0">{title}</h1>
				{this.props.consultationState &&
					<div className="mb--d">
						<p className="container container-full ml--0">
							{isOpen ?
								<span>
									<span className="tag tag--open">Open for comments</span> Open until{" "}
									<Moment format="D MMMM YYYY" date={endDate}/>
								</span>
								:
								<span>
									{startOrEnd === "starts" && <span className="tag">Not yet open for comments</span>}
									{startOrEnd === "ended" && <span className="tag">Closed for comments</span>}{" "}
									This consultation {startOrEnd} on <Moment format="D MMMM YYYY" date={(showStartDate ? startDate : endDate)}/> at{" "}
									<Moment format="HH:mm" date={(showStartDate ? startDate : endDate)}/>
								</span>
							}
							&nbsp;&nbsp;
							<Link to={"/leadinformation"}>
								Request commenting lead permission
							</Link>
						</p>
					</div>
				}
				{subtitle1 && <p>{subtitle1}</p>}
				{subtitle2 &&
					<Alert type="caution" role="alert">
						<p>{subtitle2}</p>
					</Alert>
				}
			</ Fragment>
		);
	}

}
