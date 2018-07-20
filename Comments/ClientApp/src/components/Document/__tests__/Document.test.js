/* global jest */

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

jest.mock("../../../context/UserContext", () => {
	return {
		UserContext: {
			Consumer: (props) => {
				return props.children({
					isAuthorised: true
				});
			}
		}
	};
});

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
					.onGet("/consultations/api/Consultation?consultationId=1&isReview=false")
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

		describe("getDocumentLinks method", () => {
			const documents = [
				{
					title: "Document One",
					documentId: 1,
					convertedDocument: false,
					href: "/guidance/one/123",
					chapters: null
				},
				{
					title: "",
					documentId: 2,
					chapters: [{ slug: "chapter-two-slug" }],
					convertedDocument: true,
					href: "/guidance/two/123"
				},
				{
					title: "Document Three",
					documentId: 3,
					convertedDocument: false,
					href: "/guidance/three/123",
					chapters: null
				},
				{
					title: "Document Four",
					documentId: 4,
					convertedDocument: true,
					href: "/guidance/four/123",
					chapters: [{ slug: "chapter-two-slug" }]
				}
			];

			it("getDocumentLinks for Commentable documents", () => {
				const document = new Document();
				expect(
					document.getDocumentLinks(true, "This is the title", documents, 1, 1)
				).toHaveProperty("links", [
					{
						current: false,
						label: "Download Document",
						url: "/1/2/chapter-two-slug",
						isReactRoute: true
					},
					{
						current: false,
						label: "Document Four",
						url: "/1/4/chapter-two-slug",
						isReactRoute: true
					}
				]);
			});

			it("getDocumentLinks for Supporting documents", () => {
				const document = new Document();
				expect(
					document.getDocumentLinks(false, "This is the title", documents, 1, 1)
				).toHaveProperty("links", [
					{
						current: true,
						label: "Document One",
						url: "/guidance/one/123",
						isReactRoute: false
					},
					{
						current: false,
						label: "Document Three",
						url: "/guidance/three/123",
						isReactRoute: false
					}
				]);
			});
		});
	});
});
