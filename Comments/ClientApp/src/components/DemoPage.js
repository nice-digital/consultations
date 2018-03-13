// @flow

import React from "react";
import NavMenu from "./NavMenu/NavMenu";

type PropsType = Object;

const Layout = (props: PropsType) => {
	return (
		<div className="container">
			<div className="grid">
				<div data-g="12 sm:3">
					<NavMenu />
				</div>
				<div data-g="12 sm:9">{props.children}</div>
			</div>
		</div>
	);
};

export default Layout;
