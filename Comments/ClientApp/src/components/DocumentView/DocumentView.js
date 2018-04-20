// @flow
import React, {Component, Fragment} from "react";

import DocumentWithRouter from "../Document/Document";
import {Drawer} from "../Drawer/Drawer";

export class DocumentView extends Component {

	constructor(){
		super();
		// this creates a reference to <Drawer />
		this.drawer = React.createRef();
	}

	newCommentHandler = (incomingComment) => {
		// this method is passed down to <DocumentWithRouter /> by props.
		// The function is the one we're using from <Drawer />
		this.drawer.current.newComment(incomingComment);
	};

	render() {
		return (
			<Fragment>
				{/* "ref" ties the <Drawer /> component to React.createRef() above*/}
				<Drawer ref={this.drawer}/>
				{/* Passing the function we're using from <Drawer /> to DocWithRouter via props*/}
				<DocumentWithRouter onNewCommentClick={this.newCommentHandler}/>
			</Fragment>
		);
	}
}

export default DocumentView;
