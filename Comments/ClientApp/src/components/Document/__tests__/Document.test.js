import React from "react";
import { shallow, mount } from "enzyme";
import { MemoryRouter } from "react-router-dom";

import axios from "axios";
import MockAdapter from "axios-mock-adapter";

import DocumentWithRouter, { Document } from "../Document";
// import sampleData from "./sample";
// import { nextTick } from "../../../helpers/utils";

const mock = new MockAdapter(axios);

describe("[ClientApp] ", () => {

	describe("Document Component", () => {

		it("Needs tests rewriting", () => {

		});

		// it("should render with the loading message", () => {
		// 	const wrapper = mount(
		// 		<MemoryRouter>
		// 			<DocumentWithRouter />
		// 		</MemoryRouter>
		// 	);
		// 	expect(wrapper.find("h1").text()).toEqual("Loading...");
		// });

		// it("should render the component with the supplied data", async () => {
		// 	const wrapper = mount(
		// 		<MemoryRouter>
		// 			<DocumentWithRouter />
		// 		</MemoryRouter>
		// 	);
		// 	mock.onGet().reply(200, sampleData);
		// 	await nextTick();
		// 	wrapper.update();
		// 	expect(wrapper.find("h1").text()).toEqual("For consultation comments");
		// });
		//
		// it("renderDocumentHtml method", () => {
		// 	const html = `	<h1>Testing</h1>
		// 					<span> &amp; </span>
		// 					<p>...the HTML</p>`;
		// 	const wrapper = mount(
		// 		<MemoryRouter>
		// 			<DocumentWithRouter />
		// 		</MemoryRouter>
		// 	);
		// 	expect(wrapper.instance().renderDocumentHtml(html))
		// 		.toHaveProperty("__html",
		// 			"\t<h1>Testing</h1>\n\t\t\t\t\t\t\t<span> &amp; </span>\n\t\t\t\t\t\t\t<p>...the HTML</p>");
		// });
		//
		// describe("getSupportingDocumentLinks method", () => {
		// 	it("should only return links for documents with a title & Id", () => {
		// 		const documents = [{
		// 			title: "Document One",
		// 			documentId: 1,
		// 			chapters: [ { slug: "chapter-one-slug"	} ]
		// 		},
		// 		{
		// 			title: "",
		// 			documentId: 2,
		// 			chapters: [ { slug: "chapter-two-slug"	} ]
		// 		},
		// 		{
		// 			title: "Document Three",
		// 			documentId: 3,
		// 			chapters: [ { slug: "chapter-three-slug"} ]
		// 		}];
		// 		const wrapper = mount(
		// 			<MemoryRouter>
		// 				<DocumentWithRouter />
		// 			</MemoryRouter>
		// 		);
		// 		expect(wrapper
		// 			.instance()
		// 			.getSupportingDocumentLinks(documents))
		// 			.toHaveProperty("links",
		// 				[
		// 					{ label: "Document One", url: "/1/1/chapter-one-slug" },
		// 					{ label: "Document Three", url: "/1/3/chapter-three-slug" }
		// 				]);
		// 	});
		// });

	});
});
