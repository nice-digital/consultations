import React from "react";
import { shallow, mount } from "enzyme";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
// import { MemoryRouter } from "react-router";
// import FlushPromises from "flush-promises";
// import renderer from "react-test-renderer";

import DocumentWithRouter, {Document} from "../Document";
// import ChapterData from "./Chapter";
// import ConsultationData from "./Consultation";
// import DocumentsData from "./Documents";
// import { generateUrl } from "./../../../data/loader";

describe("[ClientApp] ", () => {

	describe("Document Component", () => {

		const mock = new MockAdapter(axios);

		const fakeProps = {
			location: {},
			match: {
				params: {
					consultationId: 1,
					documentId: 1,
					chapterSlug: "introduction"
				}
			}
		};

		it("should render with the loading message when the data is loading", () => {
			mock.restore();
			mock.onAny().reply(200);
			const wrapper = mount(
				<Document {...fakeProps} />
			);
			expect(wrapper.find("h1").text()).toEqual("Loading...");
		});

		it("renderDocumentHtml method", () => {
			const html = `	<h1>Testing</h1>
							<span> &amp; </span>
							<p>...the HTML</p>`;
			const document = new Document;
			expect(document.renderDocumentHtml(html))
				.toHaveProperty("__html",
					"\t<h1>Testing</h1>\n\t\t\t\t\t\t\t<span> &amp; </span>\n\t\t\t\t\t\t\t<p>...the HTML</p>");
		});

		it("getSupportingDocumentLinks method", () => {
			const documents = [{
				title: "Document One",
				documentId: 1,
				chapters: [{slug: "chapter-one-slug"}]
			},
			{
				title: "",
				documentId: 2,
				chapters: [{slug: "chapter-two-slug"}]
			},
			{
				title: "Document Three",
				documentId: 3,
				chapters: [{slug: "chapter-three-slug"}]
			}];
			const document = new Document;
			expect(
				document.getSupportingDocumentLinks(documents, 1, 1))
				.toHaveProperty("links",
					[
						{
							current: true,
							label: "Document One",
							url: "/1/1/chapter-one-slug"
						},
						{
							current: false,
							label: "Document Three",
							url: "/1/3/chapter-three-slug"
						}
					]);
		});

	});
});
