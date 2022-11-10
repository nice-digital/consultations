import React from "react";
import { render, screen, waitForElementToBeRemoved } from "@testing-library/react";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { MemoryRouter } from "react-router";
import { DocumentPreview } from "../DocumentPreview";
import PreviewChapterData from "./PreviewChapter.json";
import PreviewConsultationData from "./PreviewConsultation.json";
import PreviewDocumentsData from "./PreviewDocuments.json";

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
	staticContext: {
		preload: {
			data:{
				isAuthorised: true,
			},
		},
	},
};

test("should match snapshot with supplied data", () => {
	const mock = new MockAdapter(axios);
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
			.onGet("/consultations/api/DraftConsultation?consultationId=113&documentId=1&reference=GID-NG10186")
			.reply(() => {
				resolve();
				return [200, PreviewConsultationData];
			});
	});
	let chapterPromise = new Promise(resolve => {
		mock
			.onGet(
				"/consultations/api/PreviewChapter?consultationId=113&documentId=1&chapterSlug=draftappendicesa-tcleandocx&reference=GID-NG10186",
			)
			.reply(() => {
				resolve();
				return [200, PreviewChapterData];
			});
	});
	const {container} = render(
		<MemoryRouter>
			<DocumentPreview {...fakeProps} />
		</MemoryRouter>,
	);
	return Promise.all([
		documentsPromise,
		consulatationPromise,
		chapterPromise,
	]).then(async () => {
		await waitForElementToBeRemoved(() => screen.getByText("Loading...", { selector: "h1" }));
		expect(container).toMatchSnapshot();
	});
});

test("getPreviewDocumentChapterLinks method", () => {
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
