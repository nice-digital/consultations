import React, {Component, Fragment} from "react";

export class Questions extends Component {
	render(){
		return  (
			<Fragment>
				<h2>Questions</h2>
				<div className="grid">
					<div data-g="12 md:3">
						left
					</div>
					<div data-g="12 md:9">
						right
					</div>
				</div>
			</Fragment>
		);
	}
}
