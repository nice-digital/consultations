import React, {Component} from "react";

export default class Layout extends Component {
	render() {
		return (
			<div className="container">
				<div className="grid">
					<div data-g="12">
						{this.props.children}
					</div>
				</div>
			</div>
		);
	}
}

