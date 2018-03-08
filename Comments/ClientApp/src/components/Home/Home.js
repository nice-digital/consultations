// @flow

import React from "react";
import { Helmet } from "react-helmet";

const Home = () => {
	return (
		<div>
			<Helmet>
				<title>Homepage title rendered with helmet</title>
				<meta name="description" content="This is a meta description render" />
			</Helmet>
			<h1>Static HTML</h1>
			<p>
				Lorem ipsum dolor sit amet, consectetur adipisicing elit. Voluptatum,
				quasi tenetur. Ut, officiis voluptatem maxime corporis vel explicabo
				cupiditate ducimus sit! Illo ipsa dolor doloremque tenetur inventore
				asperiores fugiat voluptas eaque recusandae rem voluptates a
				perspiciatis numquam quisquam repellendus, voluptatem impedit veniam
				nemo? Accusantium cupiditate tempore mollitia vero dicta porro.
			</p>
		</div>
	);
};

export default Home;
