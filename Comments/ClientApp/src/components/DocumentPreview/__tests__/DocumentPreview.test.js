/* global jest */

import React from "react";
import { shallow, mount } from "enzyme";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { MemoryRouter } from "react-router";

import { DocumentPreview } from "../DocumentPreview";
import PreviewChapterData from "./PreviewChapter";
import PreviewConsultationData from "./PreviewConsultation";
import PreviewDocumentsData from "./PreviewDocuments";
import { nextTick } from "../../../helpers/utils";
import toJson from "enzyme-to-json";

jest.mock("../../../context/UserContext", () => {
	return {
		UserContext: {
			Consumer: (props) => {
				return props.children({
					isAuthorised: true,
				});
			},
		},
	};
});

describe("[ClientApp] ", () => {
	describe("DocumentPreview Component", () => {
		const fakeProps = {
			location: {},
			match: {
				params: {
					consultationId: 113,
					documentId: 1,
					chapterSlug: "draftappendicesa-tcleandocx",
					reference: "GID-NG10186",
				},
			},
		};

		it("should render the loading message", () => {
			const wrapper = shallow(<DocumentPreview {...fakeProps} />, {
				disableLifecycleMethods: true,
			});
			expect(wrapper.find("h1").text()).toEqual("Loading...");
		});

		it("should match snapshot with supplied data", () => {
			const mock = new MockAdapter(axios);

			const wrapper = mount(
				<MemoryRouter>
					<DocumentPreview {...fakeProps} />
				</MemoryRouter>
			);

			let documentsPromise = new Promise(resolve => {
				mock
					.onGet("consultations/api/PreviewDraftDocuments?consultationId=113&documentId=1&reference=GID-NG10186")
					.reply(() => {
						resolve();
						return [200, PreviewDocumentsData];
					});
			});

			let consulatationPromise = new Promise(resolve => {
				mock
					.onGet("/consultations/api/Consultation?consultationId=113&isReview=false")
					.reply(() => {
						resolve();
						return [200, PreviewConsultationData];
					});
			});

			let chapterPromise = new Promise(resolve => {
				mock
					.onGet(
						"/consultations/api/PreviewChapter?consultationId=113&documentId=1&chapterSlug=draftappendicesa-tcleandocx&reference=GID-NG10186"
					)
					.reply(() => {
						resolve();
						return [200, PreviewChapterData];
					});
			});

			return Promise.all([
				documentsPromise,
				consulatationPromise,
				chapterPromise,
			]).then(async () => {
				await nextTick();
				wrapper.update();
				console.log(wrapper.html());
				expect(
					toJson(wrapper, {
						noKey: true,
						mode: "deep",
					})
				).toMatchSnapshot();
			});
		});

		it("getPreviewDocumentChapterLinks method", () => {
			const documentPreview = new DocumentPreview();
			expect(
				documentPreview.getPreviewDocumentChapterLinks(1, "draftappendicesa-tcleandocx", PreviewDocumentsData, "this is my title", "GID-1234", 1)).toHaveProperty("links", [
				{
					"current": true,
					"isReactRoute": true,
					"label": "DRAFT_Appendices_A-T_CLEAN.docx",
					"url": "/preview/GID-1234/consultation/1/document/1/chapter/draftappendicesa-tcleandocx",
				},
				{
					"current": false,
					"isReactRoute": true,
					"label": "Appendices",
					"url": "/preview/GID-1234/consultation/1/document/1/chapter/appendices",
				},
				{
					"current": false,
					"isReactRoute": true,
					"label": "1 Scope",
					"url": "/preview/GID-1234/consultation/1/document/1/chapter/scope",
				}]);
		});

	});
});
