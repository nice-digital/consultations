import React, {Component, Fragment} from "react";
import { ConsultationItem } from "../ConsultationItem/ConsultationItem";
import ConsultationSampleData from "./ConsultationSampleData.json";

export class ConsultationList extends Component {



	render(){
		return  (
			<Fragment>
				<div className="grid">
					<div data-g="12 md:3">
						<h2>Filter</h2>
					</div>
					<div data-g="12 md:9">
						<h2>All consultations</h2>
						<ul className="list--unstyled">
							{ConsultationSampleData.map((item, idx) => <ConsultationItem key={idx} {...item} />)}
						</ul>
					</div>
				</div>
			</Fragment>
		);
	}

}
