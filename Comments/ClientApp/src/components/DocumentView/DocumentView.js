// @flow
import React, {Component, Fragment} from "react";

import DocumentWithRouter from "../Document/Document";
import {Drawer} from "../Drawer/Drawer";

export class DocumentView extends Component {

	constructor(){
		super();
		this.drawer = React.createRef();
	}

	newCommentHandler = (incomingComment) => {
		this.drawer.current.newComment(incomingComment);
	};

	render() {
		return (
			<Fragment>
				<Drawer ref={this.drawer}/>
				<DocumentWithRouter onNewCommentClick={this.newCommentHandler}/>
			</Fragment>
		);
	}
}

export default DocumentView;
