// @flow

import React, { Component } from "react";
import { Helmet } from "react-helmet";
import Breadcrumbs from "./../Breadcrumbs/Breadcrumbs";
import StackedNav from "./../StackedNav/StackedNav";

type PropsType = {};

class FetchData extends Component<PropsType> {
	render() {
		const breadcrumbs = [
			{ label: "Home", url: "/document" },
			{ label: "NICE Guidance", url: null },
			{ label: "In Consulation", url: null },
			{ label: "Document title", url: null }
		];

		const chapterLinks = {
			root: { label: "Chapters in this document", url: null },
			links: [
				{ label: "Key priorities for implementation", url: null },
				{ label: "Recommendations", url: null },
				{ label: "Intravenous fluid therapy in children and young people in hospital", url: null },
				{ label: "Context", url: null },
				{ label: "Recommendations for research", url: null }
			]
		};

		const additionalDocuments = {
			root: { label: "Additional documents to comment on", url: null },
			links: [
				{ label: "Intravenous fluid therapy in children and young people in hospital - Short Guideline", url: null }
			]
		};

		return (
			<div>
				<Helmet>
					<title>Comment on Document</title>
				</Helmet>
				<Breadcrumbs segments={breadcrumbs} />
				<div className="page-header">
					<h1 className="page-header__heading">
						Intravenous fluid therapy in children and young people in hospital : Consultation
					</h1>
					<p className="page-header__lead">In development [NG29] Expected publication date TBC</p>
				</div>
				<div className="grid">
					<div data-g="12 lg:3">
						<StackedNav links={chapterLinks} />
						<StackedNav links={additionalDocuments} />
					</div>
					<div data-g="12 lg:6">
						<div className="document-comment-container">
							<h1>Key priorities for implementation</h1>
							<p>
								The following recommendations have been identified as priorities for implementation. The full list of
								recommendations is in the recommendations section.
							</p>
							<hr />
							<h2>Assessment and monitoring</h2>
							<ul>
								<li>
									Lorem ipsum dolor sit amet, consectetur adipisicing elit. Fugit amet inventore quaerat voluptas culpa
									cumque!
									<ul>
										<li>Lorem ipsum dolor sit amet consectetur adipisicing elit. Nostrum, hic.</li>
										<li>Illo, iure! Eum beatae porro ipsam consequatur doloribus. Dolores, suscipit.</li>
										<li>Soluta enim id vero beatae eos qui ut nesciunt accusamus?</li>
									</ul>
								</li>
								<li>
									Soluta, molestias labore. Eos, maiores quis! Veritatis fuga, animi dolores molestiae tempora numquam
									omnis atque.
								</li>
								<li>
									Quam debitis accusantium ea esse cumque rem dolore ullam veritatis doloribus, tempore natus?
									Consequatur, consequuntur.
								</li>
								<li>
									Eligendi, pariatur inventore fuga amet nostrum velit. Adipisci necessitatibus ullam dolorem velit,
									iste consequatur veritatis.
								</li>
								<li>
									Voluptate quaerat, nulla iusto distinctio iure quibusdam in illo accusamus nemo sint. Recusandae,
									eveniet quod!
								</li>
							</ul>
							<p>
								Lorem ipsum dolor sit amet consectetur adipisicing elit. Sequi enim delectus, ducimus repellendus
								laudantium, sed architecto omnis praesentium expedita officia corporis fuga sit sint ea?
							</p>
							<p>
								Cum dolores porro soluta aliquam vel quidem. Minus eligendi esse repudiandae tempora quisquam quia rerum
								consequatur delectus. Saepe beatae excepturi minus dolorem blanditiis, omnis nulla.
							</p>
							<p>
								Repellendus vel quos inventore accusamus nesciunt praesentium nam nisi. Obcaecati quaerat quasi facere
								veniam atque, vel magnam unde tenetur dolor reprehenderit quam ullam suscipit eveniet.
							</p>
						</div>
					</div>
					<div data-g="12 lg:3">
						<div data-inpagenav data-inpagenav-headings-container=".document-comment-container" />
					</div>
				</div>
			</div>
		);
	}
}

export default FetchData;
