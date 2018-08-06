import React, { Component, Fragment } from "react";
import { withRouter } from "react-router";

import DocumentWithRouter from "../Document/Document";
//import { Drawer } from "../Drawer/Drawer";
import CommentListWithRouter from "../CommentList/CommentList";
//import { load } from "./../../data/loader";

type PropsType = {
	location: {
		pathname: string,
		search: string
	}
};

// type ConsultationStateType = {
// 	consultationIsOpen: boolean,
// 	hasQuestions: boolean,
// 	consultationHasEnded: boolean,
// 	hasUserSuppliedAnswers: boolean,
// 	hasUserSuppliedComments: boolean
// };

// type ConsultationDataType = {
// 	consultationState: ConsultationStateType,
// 	supportsComments: boolean,
// 	supportsQuestions: boolean
// };

type StateType = {
	// consultationData: ConsultationDataType,
	// shouldShowDrawer: boolean,
	// shouldShowCommentsTab: boolean,
	// shouldShowQuestionsTab: boolean,
	error: {
		hasError: boolean,
		message: string
	}
};

export class DocumentView extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);
		// this creates a reference to <Drawer />
		this.drawer = React.createRef();

		this.state = {
			// consultationData: null,
			// shouldShowDrawer: true,
			// shouldShowCommentsTab: false,
			// shouldShowQuestionsTab: false,
			error: {
				hasError: false,
				message: null
			}
		};
	}

	newCommentHandler = incomingComment => {
		// this method is passed down to <DocumentWithRouter /> by props.
		// The function is the one we're using from <Drawer />
		this.commentList.current.newComment(incomingComment);
	};

	// gatherData = async () => {
	// 	const consultationId = this.props.match.params.consultationId;
	// 	const consultationData = load("consultation", undefined, [], {
	// 		consultationId
	// 	})
	// 		.then(response => response.data)
	// 		.catch(err => {
	// 			this.setState({
	// 				error: {
	// 					hasError: true,
	// 					message: "consultationData " + err
	// 				}
	// 			});
	// 		});

	// 	return {
	// 		consultationData: await consultationData
	// 	};
	// };

	// componentDidMount() {
	// 	this.gatherData()
	// 		.then(data => {
				
	// 			const shouldShowDrawer =  (	data.consultationData.supportsComments || 
	// 										data.consultationData.supportsQuestions ||
	// 										data.consultationData.consultationState.hasUserSuppliedComments || 
	// 										data.consultationData.consultationState.hasUserSuppliedAnswers); 
	// 			const shouldShowCommentsTab = (	data.consultationData.supportsComments || 
	// 											data.consultationData.consultationState.hasUserSuppliedComments);

	// 			const shouldShowQuestionsTab = (	data.consultationData.consultationState.hasQuestions || 
	// 												data.consultationData.consultationState.hasUserSuppliedAnswers);

	// 			this.setState({
	// 				consultationData : data.consultationData,
	// 				shouldShowDrawer: shouldShowDrawer,
	// 				shouldShowCommentsTab: shouldShowCommentsTab,
	// 				shouldShowQuestionsTab: shouldShowQuestionsTab
	// 			});
	// 		})
	// 		.catch(err => {
	// 			this.setState({
	// 				error: {
	// 					hasError: true,
	// 					message: "gatherData in componentDidMount failed  " + err
	// 				}
	// 			});

	// 		});
	// }

	render() {
		if (this.state.error.hasError) { throw new Error(this.state.error.message) }
		return (
			<Fragment>
				
				{/* "ref" ties the <Drawer /> component to React.createRef() above*/}
				{/* {this.state.shouldShowDrawer && 					 */}
					{/* <Drawer ref={this.drawer} 
					shouldShowCommentsTab={this.state.shouldShowCommentsTab} 
					shouldShowQuestionsTab={this.state.shouldShowQuestionsTab}
					/> */}
					<CommentListWithRouter  ref={this.commentList} />
				{/* } */}
				{/* Passing the function we're using from <Drawer /> to DocWithRouter via props*/}
				<DocumentWithRouter onNewCommentClick={this.newCommentHandler} />
			</Fragment>
		);
	}
}

export default withRouter(DocumentView);
