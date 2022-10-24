import React from "react";
import { render, screen, waitForElementToBeRemoved } from "@testing-library/react";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { MemoryRouter } from "react-router";
import { Document } from "../Document";
import ChapterData from "./Chapter.json";
import ConsultationData from "./Consultation.json";
import ConsultationQuestionsOnlyData from "./ConsultationQuestionsOnly.json";
import DocumentsData from "./Documents.json";

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
			consultationId: 1,
			documentId: 1,
			chapterSlug: "introduction",
		},
	},
};

const documents = [
	{
		title: "Document One",
		documentId: 1,
		convertedDocument: false,
		href: "/guidance/one/123",
		chapters: null,
	},
	{
		title: "",
		documentId: 2,
		chapters: [{ slug: "chapter-two-slug" }],
		convertedDocument: true,
		href: "/guidance/two/123",
	},
	{
		title: "Document Three",
		documentId: 3,
		convertedDocument: false,
		href: "/guidance/three/123",
		chapters: null,
	},
	{
		title: "Document Four",
		documentId: 4,
		convertedDocument: true,
		href: "/guidance/four/123",
		chapters: [{ slug: "chapter-two-slug" }],
	},
];

it("should match snapshot with supplied data", () => {
	const mock = new MockAdapter(axios);
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
				"/consultations/api/Chapter?consultationId=1&documentId=1&chapterSlug=introduction",
			)
			.reply(() => {
				resolve();
				return [200, ChapterData];
			});
	});
	const {container} = render(
		<MemoryRouter>
			<Document {...fakeProps} />
		</MemoryRouter>,
	);

	return Promise.all([
		documentsPromise,
		consulatationPromise,
		chapterPromise,
	]).then(async () => {
		const tutorialButton = await screen.findByText("Hide how to comment", { selector: "button" });
		expect(tutorialButton).toBeInTheDocument();
		expect(container).toMatchSnapshot();
	});
});

it("should not contain tutorial if the consultation does not support comments", () => {
	const mock = new MockAdapter(axios);
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
				return [200, ConsultationQuestionsOnlyData];
			});
	});
	let chapterPromise = new Promise(resolve => {
		mock
			.onGet(
				"/consultations/api/Chapter?consultationId=1&documentId=1&chapterSlug=introduction",
			)
			.reply(() => {
				resolve();
				return [200, ChapterData];
			});
	});
	render(
		<MemoryRouter>
			<Document {...fakeProps} />
		</MemoryRouter>,
	);

	return Promise.all([
		documentsPromise,
		consulatationPromise,
		chapterPromise,
	]).then(async () => {
		await waitForElementToBeRemoved(() => screen.getByText("Loading...", { selector: "h1" }));
		expect(screen.queryAllByText("Hide how to comment", { selector: "button" }).length).toEqual(0);
	});
});

it("getDocumentChapterLinks method", () => {
	const document = new Document();
	expect(
		document.getDocumentChapterLinks(1, "introduction", 1, DocumentsData, "A title for the nav"),
	).toHaveProperty("links", [{"current": true, "isReactRoute": true, "label": "Introduction", "url": "/1/1/introduction"}, {"current": false, "isReactRoute": true, "label": "Patient-centred care", "url": "/1/1/patient-centred-care"}, {"current": false, "isReactRoute": true, "label": "Key priorities for implementation", "url": "/1/1/key-priorities-for-implementation"}, {"current": false, "isReactRoute": true, "label": "1 Guidance", "url": "/1/1/guidance"}, {"current": false, "isReactRoute": true, "label": "2 Notes on the scope of the guidance", "url": "/1/1/notes-on-the-scope-of-the-guidance"}, {"current": false, "isReactRoute": true, "label": "3 Implementation", "url": "/1/1/implementation"}, {"current": false, "isReactRoute": true, "label": "4 Research recommendations", "url": "/1/1/research-recommendations"}, {"current": false, "isReactRoute": true, "label": "5 Other versions of this guideline", "url": "/1/1/other-versions-of-this-guideline"}, {"current": false, "isReactRoute": true, "label": "6 Related NICE guidance", "url": "/1/1/related-nice-guidance"}, {"current": false, "isReactRoute": true, "label": "7 Updating the guideline", "url": "/1/1/updating-the-guideline"}, {"current": false, "isReactRoute": true, "label": "Appendix A: The Guideline Development Group and NICE project team", "url": "/1/1/appendix-a-the-guideline-development-group-and-nice-project-team"}, {"current": false, "isReactRoute": true, "label": "Appendix B: The Guideline Review Panel", "url": "/1/1/appendix-b-the-guideline-review-panel"}, {"current": false, "isReactRoute": true, "label": "Appendix C: The algorithm", "url": "/1/1/appendix-c-the-algorithm"}, {"current": false, "isReactRoute": true, "label": "Changes after publication", "url": "/1/1/changes-after-publication"}, {"current": false, "isReactRoute": true, "label": "About this guideline", "url": "/1/1/about-this-guideline"}]);
});

it("getDocumentLinks for Commentable documents", () => {
	const document = new Document();
	expect(
		document.getDocumentLinks(true, "This is the title", documents, 1, 1),
	).toHaveProperty("links", [
		{
			current: false,
			label: "Download Document",
			url: "/1/2/chapter-two-slug",
			isReactRoute: true,
		},
		{
			current: false,
			label: "Document Four",
			url: "/1/4/chapter-two-slug",
			isReactRoute: true,
		},
	]);
});

it("getDocumentLinks for Supporting documents", () => {
	const document = new Document();
	expect(
		document.getDocumentLinks(false, "This is the title", documents, 1, 1),
	).toHaveProperty("links", [
		{
			current: true,
			label: "Document One",
			url: "/guidance/one/123",
			isReactRoute: false,
		},
		{
			current: false,
			label: "Document Three",
			url: "/guidance/three/123",
			isReactRoute: false,
		},
	]);
});

it("should not caution that guidance is draft if the consultation is open", () => {
	const mock = new MockAdapter(axios);
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
				"/consultations/api/Chapter?consultationId=1&documentId=1&chapterSlug=introduction",
			)
			.reply(() => {
				resolve();
				return [200, ChapterData];
			});
	});
	render(
		<MemoryRouter>
			<Document {...fakeProps} />
		</MemoryRouter>,
	);
	return Promise.all([
		documentsPromise,
		consulatationPromise,
		chapterPromise,
	]).then(async () => {
		const warningShown = screen.queryAllByText("The content on this page is not current guidance and is only for the purposes of the consultation process.").length > 0;
		expect(warningShown).toEqual(false);
	});
});
