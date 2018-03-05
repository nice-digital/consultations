// @flow

import React, { Component } from "react";
import { Helmet } from "react-helmet";
import Breadcrumbs from "./../Breadcrumbs/Breadcrumbs";
import StackedNav from "./../StackedNav/StackedNav";
import { HashLink } from "react-router-hash-link";

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
			root: { label: "Chapters in this document", url: "#" },
			links: [
				{ label: "Key priorities for implementation", url: null },
				{ label: "Recommendations", url: null },
				{ label: "Intravenous fluid therapy in children and young people in hospital", url: null },
				{ label: "Context", url: null },
				{ label: "Recommendations for research", url: null }
			]
		};

		const additionalDocuments = {
			root: { label: "Additional documents to comment on", url: "#" },
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
				<HashLink
					smooth
					to={"#section"}
				>Scroll to section</HashLink>
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
							<h2>H2 Heading</h2>
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
							<p>
								Lorem, ipsum dolor sit amet consectetur adipisicing elit. Neque officia magnam eligendi quae sunt
								numquam veniam impedit. Dolore voluptas minus velit, molestiae quas sed corporis laboriosam dolor,
								maiores, voluptate natus!
							</p>
							<p>
								Minima odit iusto eos. Cum, odit id nostrum maxime repellendus dolore! Ipsa aliquam commodi nihil,
								cupiditate dolore expedita. Laudantium aut, nostrum rem commodi perferendis quisquam at quas est ut
								facere.
							</p>
							<h3>H3 Heading</h3>
							<p>
								Rerum et esse, est reiciendis cupiditate recusandae iusto cum, veritatis quod quos perferendis nam,
								necessitatibus fuga eveniet quam id sed aperiam. Error veniam iure eveniet officiis! Ab optio ducimus
								laboriosam?
							</p>
							<p>
								Consectetur assumenda quidem tenetur, possimus sit ipsa repellendus vel atque accusamus odio nulla
								similique nobis id itaque nihil earum officia cupiditate, ab, sed ullam est impedit nisi adipisci omnis?
								Dolorum!
							</p>
							<p>
								Voluptatem temporibus inventore numquam possimus cupiditate, libero ad magnam, aperiam praesentium
								itaque consequuntur maiores dolorem vitae et, eius expedita iusto dicta quasi alias eveniet voluptatibus
								deserunt eum. Ipsam, accusamus nemo!
							</p>
							<p>
								Architecto itaque quidem dignissimos consectetur illum natus nostrum iusto amet sequi, possimus atque!
								Nisi, numquam. Maiores, cum optio tempora incidunt aperiam quos voluptate itaque. Ullam mollitia ad
								aperiam id dolore!
							</p>
							<p>
								Rerum, velit tempora? Quas nesciunt, repellat facilis magnam reprehenderit eius delectus, ducimus labore
								quasi sed et deserunt beatae nihil itaque similique eum. Soluta iusto atque voluptatibus, perferendis
								porro dolores itaque?
							</p>
							<p>
								Fuga ad dolorem optio facilis ea voluptatibus deleniti. Dolorum commodi excepturi sit, ab maxime cum
								voluptatum, voluptas distinctio velit omnis quam ratione laboriosam quos provident, optio voluptatibus
								suscipit magni eveniet.
							</p>
							<a id="section">Section</a>
							<p>
								Beatae quo iste nobis, ducimus eum consequuntur, eius harum voluptatem numquam maxime odit reiciendis
								nisi labore hic? Autem excepturi atque omnis quod fuga nam laboriosam, perspiciatis ipsam odio quam
								temporibus!
							</p>
							<h4>H4 Heading</h4>
							<p>
								Porro quibusdam sapiente maiores sed eum voluptatum, qui neque vitae! Cum velit molestias magnam
								nesciunt ducimus minima quisquam ab autem asperiores officiis laboriosam, error consequatur placeat
								eligendi laborum accusamus similique.
							</p>
						</div>
					</div>
					<div data-g="12 lg:3">
						<HashLink smooth to="#section"></HashLink>
					</div>
				</div>
			</div>
		);
	}
}

export default FetchData;
