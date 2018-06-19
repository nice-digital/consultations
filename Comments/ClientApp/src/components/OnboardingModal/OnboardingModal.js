import React, { Component } from "react";
import { UserContext } from "../../context/UserContext";

class OnboardingModal extends Component {
	constructor(){
		super();
		this.state = {
			onboarded: false
		};
	}

	render(){
		return (
			<UserContext.Consumer>
				{contextValue => contextValue.isAuthorised ?
					<div style={{ "display": this.state.onboarded ? "none" : "block" }}
						 className="onboarding"
					>
						<div className="onboarding__modal">
							<button
								data-qa-sel="close-onboarding-modal"
								className="onboarding__closeButton"
								onClick={() => this.setState({ onboarded: true })}>
								&times;
							</button>
							<h1>How to make comments</h1>
							<div className="grid">
								<div data-g="6">
									<p><b>Click on the icon next to a heading to comment on the chapter or section</b></p>
									<img src="images/comment-icon-helper.png" alt="Diagram showing how to click the comment icon to comment on a section"/>
								</div>
								<div data-g="6">
									<p><b>Highlight some text to make a comment on it</b></p>
									<img src="images/selection-helper.png" alt="Diagram showing how to select some text and make a comment on it"/>
								</div>
							</div>
						</div>
					</div>
					:
					null}
			</UserContext.Consumer>
		);
	}
}

export default OnboardingModal;
