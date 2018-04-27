import React from "react";
import { shallow, mount } from "enzyme";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { MemoryRouter } from "react-router";

import { Document } from "../Document";
import ChapterData from "./Chapter";
import ConsultationData from "./Consultation";
import DocumentsData from "./Documents";
import { nextTick } from "../../../helpers/utils";
import toJson from "enzyme-to-json";

// import { generateUrl } from "./../../../data/loader";

describe("[ClientApp] ", () => {
	describe("Document Component", () => {
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

		it("should render the loading message", () => {
			const wrapper = shallow(<Document {...fakeProps} />, {
				disableLifecycleMethods: true
			});
			expect(wrapper.find("h1").text()).toEqual("Loading...");
		});

		it("should match snapshot with supplied data", () => {
			const mock = new MockAdapter(axios);

			const wrapper = mount(
				<MemoryRouter>
					<Document {...fakeProps} />
				</MemoryRouter>
			);

			let documentsPromise = new Promise(resolve => {
				mock
					.onGet("/consultations/api/Documents?consultationId=1")
					.reply(() => {
						resolve();
						return [200, DocumentsData];
					});
			});

			let consulatationPromise = new Promise(resolve => {
				mock
					.onGet("/consultations/api/Consultation?consultationId=1")
					.reply(() => {
						resolve();
						return [200, ConsultationData];
					});
			});

			let chapterPromise = new Promise(resolve => {
				mock
					.onGet(
						"/consultations/api/Chapter?consultationId=1&documentId=1&chapterSlug=introduction"
					)
					.reply(() => {
						resolve();
						return [200, ChapterData];
					});
			});

			return Promise.all([
				documentsPromise,
				consulatationPromise,
				chapterPromise
			]).then(async () => {
				await nextTick();
				wrapper.update();
				expect(
					toJson(wrapper, {
						noKey: true,
						mode: "deep"
					})
				).toMatchSnapshot();
			});
		});

		it("renderDocumentHtml method", () => {
			const html = `	<h1>Testing</h1>
							<span> &amp; </span>
							<p>...the HTML</p>`;
			const document = new Document();
			expect(document.renderDocumentHtml(html)).toHaveProperty(
				"__html",
				"\t<h1>Testing</h1>\n\t\t\t\t\t\t\t<span> &amp; </span>\n\t\t\t\t\t\t\t<p>...the HTML</p>"
			);
		});

		it("getSupportingDocumentLinks method", () => {
			const documents = [
				{
					title: "Document One",
					documentId: 1,
					chapters: [{ slug: "chapter-one-slug" }]
				},
				{
					title: "",
					documentId: 2,
					chapters: [{ slug: "chapter-two-slug" }]
				},
				{
					title: "Document Three",
					documentId: 3,
					chapters: [{ slug: "chapter-three-slug" }]
				}
			];
			const document = new Document();
			expect(
				document.getSupportingDocumentLinks(documents, 1, 1)
			).toHaveProperty("links", [
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
