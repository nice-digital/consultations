// @flow

import React, {Component} from "react";
import {mobileWidth} from "../../constants";
import CommentListWithRouter from "../CommentList/CommentList";
import {pullFocusById} from "../../helpers/accessibility-helpers";

type PropsType = {
	commentList: Function,
	match: {
		params: {
			consultationId: number
		}
	}
};

type StateType = {
	drawerExpandedWidth: boolean,
	drawerOpen: boolean,
	drawerMobile: boolean,
	commentsTabVisible: boolean
};

export class Drawer extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);
		this.state = {
			drawerExpandedWidth: false,
			drawerOpen: false,
			drawerMobile: false,
			commentsTabVisible: true
		};
	}

	componentDidMount(){
		// We can't prerender whether we're on mobile cos SSR doesn't have a window
		this.setState({
			drawerMobile: this.isMobile()
		});
	}

	// This isn't called in this file - this is the method called from DocumentView
	newComment(comment: Object) {
		this.setState({
			drawerOpen: true
		});
		// and what we're calling is newComment from inside <CommentList />
		// $FlowIgnore | this is bound by wrappedComponentRef in CommentListWithRouter
		this.commentList.newComment(comment);
	}

	isMobile = () => {
		if (typeof document !== "undefined") {
			return (
				document.getElementsByTagName("body")[0].offsetWidth <= mobileWidth
			);
		}
		return false;
	};

	drawerClassnames = () => {
		const width = this.state.drawerExpandedWidth ? "Drawer--wide" : "";
		const open = this.state.drawerOpen ? "Drawer--open" : "";
		const mobile = this.state.drawerMobile ? "Drawer--mobile" : "";
		return `Drawer ${width} ${open} ${mobile}`;
	};

	handleClick = (event: string) => {
		switch (event) {
			case "toggleWidth--narrow":
				this.setState({drawerExpandedWidth: false});
				break;
			case "toggleWidth--wide":
				this.setState({drawerExpandedWidth: true});
				break;
			case "toggleOpenComments":
				this.setState(prevState => ({
					drawerOpen: (prevState.drawerOpen && prevState.commentsTabVisible ? !prevState.drawerOpen : true),
					commentsTabVisible: true
				}));
				pullFocusById("#js-drawer-toggleopen-comments");
				break;
			case "toggleOpenQuestions":
				this.setState(prevState => ({
					drawerOpen: (prevState.drawerOpen && !prevState.commentsTabVisible ? !prevState.drawerOpen : true),
					commentsTabVisible: false
				}));
				pullFocusById("#js-drawer-toggleopen-questions");
				break;
			default:
				return;
		}
	};

	render() {
		return (
			<section aria-label="Commenting panel"
					 className={this.drawerClassnames()}>
				<div className="Drawer__controls">
					<button
						data-qa-sel="open-commenting-panel"
						id="js-drawer-toggleopen-comments"
						className={`Drawer__control Drawer__control--comments ${(this.state.commentsTabVisible ? "active" : "active")}`}
						onClick={() => this.handleClick("toggleOpenComments")}
						aria-controls="comments-panel"
						aria-haspopup="true"
						aria-label={this.state.drawerOpen ? "Close the commenting panel" : "Open the commenting panel"}
						tabIndex="0">
						{!this.state.drawerMobile ?
							<span>{(this.state.drawerOpen && this.state.commentsTabVisible ? "Close comments" : "Open comments")}</span>
							:
							<span
								className={`icon ${
									this.state.drawerOpen
										? "icon--chevron-right"
										: "icon--chevron-left"}`}
								aria-hidden="true"
								data-qa-sel="close-commenting-panel"
							/>
						}
					</button>
					<button
						data-qa-sel="open-questions-panel"
						id="js-drawer-toggleopen-questions"
						className={`Drawer__control Drawer__control--questions ${(this.state.commentsTabVisible ? "active" : "active")}`}
						onClick={() => this.handleClick("toggleOpenQuestions")}
						aria-controls="questions-panel"
						aria-haspopup="true"
						aria-label={this.state.drawerOpen ? "Close the questions panel" : "Open the questions panel"}
						tabIndex="0">
						{!this.state.drawerMobile ?
							<span>{(this.state.drawerOpen && !this.state.commentsTabVisible ? "Close questions" : "Open questions")}</span>
							:
							<span
								className={`icon ${
									this.state.drawerOpen
										? "icon--chevron-right"
										: "icon--chevron-left"}`}
								aria-hidden="true"
								data-qa-sel="close-questions-panel"
							/>
						}
					</button>
				</div>
				<div aria-hidden={!this.state.drawerOpen && !this.state.commentsTabVisible}
					 data-qa-sel="comment-panel"
					 id="comments-panel"
					 className={`Drawer__main ${(this.state.commentsTabVisible ? "" : "visuallyhidden")}`}>
					
					<CommentListWithRouter
						isReviewPage={false}
						isVisible={this.state.drawerOpen}
						// $FlowIgnore | this.commentList is bound to this below
						wrappedComponentRef={component => (this.commentList = component)}/>
				</div>
				<div aria-hidden={!this.state.drawerOpen && this.state.commentsTabVisible}
					 data-qa-sel="questions-panel"
					 id="question-panel"
					 className={`Drawer__main ${(this.state.commentsTabVisible ? "visuallyhidden" : "")}`}>
					<h3>questions panel component goes here</h3>
				</div>
			</section>
		);
	}
}

export default Drawer;
