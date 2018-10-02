// @flow

import React, {Fragment, PureComponent} from "react";
import Moment from "react-moment";

type PropsType = {
	title: string,
	endDate?: any,
	match?: any,
	subtitle1?: string,
	subtitle2?: string,
	consultationState: {
		endDate: string,
		consultationIsOpen: boolean,
		consultationHasNotStartedYet: ?boolean,
	},
}

export class Header extends PureComponent<PropsType> {

	render() {
		const title = this.props.title;
		const subtitle1 = this.props.subtitle1;
		const subtitle2 = this.props.subtitle2;
		const endDate = this.props.consultationState.endDate;
		const isOpen = this.props.consultationState.consultationIsOpen;
		const notStartedYet = this.props.consultationState.consultationHasNotStartedYet;

		let startOrEnd = "ended";

		if (notStartedYet) {
			startOrEnd = "starts";
		}

		return (
			<Fragment>
				<h1 data-qa-sel="changeable-page-header" className="page-header__heading mt--0">{title}</h1>
				<p className="page-header__lead mb--d">
					{isOpen ?
						<Fragment>
							Open until{" "}<Moment format="D MMMM YYYY" date={endDate}/>
						</Fragment>
						:
						<Fragment>
							This consultation {startOrEnd} on <Moment format="D MMMM YYYY" date={endDate}/> at {" "}
							<Moment format="HH:mm" date={endDate}/>
						</Fragment>
					}
				</p>
				{subtitle1 && <p>{subtitle1}</p>}
				{subtitle2 &&	<p>{subtitle2}</p>}
			</ Fragment>
		);
	}

}
