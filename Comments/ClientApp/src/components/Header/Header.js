// @flow

import React, { Fragment, PureComponent } from "react";
import Moment from "react-moment";

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
		const title = this.props.title;
		const subtitle1 = this.props.subtitle1;
		const subtitle2 = this.props.subtitle2;

		let endDate, isOpen, notStartedYet;

		if (this.props.consultationState) {
			endDate = this.props.consultationState.endDate || "";
			isOpen = this.props.consultationState.consultationIsOpen || "";
			notStartedYet = this.props.consultationState.consultationHasNotStartedYet || "";
		}

		let startOrEnd = "ended";

		if (notStartedYet) {
			startOrEnd = "starts";
		}

		return (
			<Fragment>
				<h1 data-qa-sel="changeable-page-header" className="page-header__heading mt--0">{title}</h1>
				{this.props.consultationState &&
				<div className="mb--d">
					{isOpen ?
						<p><span className="tag tag--open">Open for comments</span> Open until{" "}
							<Moment format="D MMMM YYYY" date={endDate}/>
						</p>
						:
						<p>
							{startOrEnd === "starts" && <span className="tag">Not yet open for comments</span>}
							{startOrEnd === "ended" && <span className="tag">Closed for comments</span>}{" "}
							This consultation {startOrEnd} on <Moment format="D MMMM YYYY" date={endDate}/> at{" "}
							<Moment format="HH:mm" date={endDate}/>
						</p>
					}
				</div>
				}
				{subtitle1 && <p>{subtitle1}</p>}
				{subtitle2 && <p>{subtitle2}</p>}
			</ Fragment>
		);
	}

}
