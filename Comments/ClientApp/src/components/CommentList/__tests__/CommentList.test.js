import React from "react";
import { fireEvent, render, screen, waitForElementToBeRemoved } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter } from "react-router-dom";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { LiveAnnouncer } from "react-aria-live";
import { CommentList } from "../CommentList";
import { createQuestionPdf } from "../../QuestionView/QuestionViewDocument";
import { UserContext } from "../../../context/UserContext";
import { generateUrl } from "../../../data/loader";
import sampleComments from "./sampleComments.json";
import sampleQuestions from "./sampleQuestions.json";
import sampleQuestionsWithNoAnswers from "./sampleQuestionsWithNoAnswers.json";
import sampleConsultation from "./sampleconsultation.json";
import emptyCommentsResponse from "./EmptyCommentsResponse.json";

const mock = new MockAdapter(axios);

jest.mock("../../QuestionView/QuestionViewDocument", () => {
	return {
		createQuestionPdf: jest.fn(),
	};
});

const fakeProps = {
	match: {
		url: "/1/1/introduction",
		params: {
			consultationId: 1,
			documentId: 1,
			chapterSlug: "introduction",
		},
	},
	location: {
		pathname: "",
	},
	comment: {
		commentId: 1,
	},
};

let contextWrapper = null;

beforeEach(() => {
	contextWrapper = { isAuthorised: true, isOrganisationCommenter: false, isLead: false };
});

afterEach(() => {
	mock.reset();
});

test("save handler put's to the api with updated comment", async () => {
	const commentToUpdate = sampleComments.comments[0];
	mock.onGet(generateUrl("comments", undefined, [], { sourceURI: fakeProps.match.url }))
		.reply(200, sampleComments);
	mock.onPut("/consultations/api/Comment/" + commentToUpdate.commentId)
		.reply(config => {
			expect(JSON.parse(config.data)).toEqual(commentToUpdate);
			return [200, commentToUpdate];
		});
	render(
		<UserContext.Provider value={contextWrapper}>
			<MemoryRouter>
				<LiveAnnouncer>
					<CommentList {...fakeProps} />
				</LiveAnnouncer>
			</MemoryRouter>
		</UserContext.Provider>,
	);
	const textArea = await screen.findByDisplayValue("first comment");
	const user = userEvent.setup();
	textArea.focus();
	await user.type(textArea, " - updated");
	user.tab();
	const saveButton = await screen.findByRole("button", { name: "Save comment" });
	fireEvent.click(saveButton);
});

test("should show login panel and not render any comment boxes without authorisation", async () => {
	mock.onGet()
		.reply(200, sampleComments);
	const updatedContextWrapper = Object.assign({}, contextWrapper);
	updatedContextWrapper.isAuthorised = false;
	render(
		<UserContext.Provider value={updatedContextWrapper}>
			<MemoryRouter>
				<LiveAnnouncer>
					<CommentList {...fakeProps} />
				</LiveAnnouncer>
			</MemoryRouter>
		</UserContext.Provider>,
	);
	await waitForElementToBeRemoved(() => screen.getByText("Loading..."));
	expect(screen.getByRole("heading", { name: "How are you taking part in this consultation?" })).toBeInTheDocument();
});

test("should show 5 (current user) comment boxes with authorisation and sample data", async () => {
	mock.onGet()
		.reply(200, sampleComments);
	const {container} = render(
		<UserContext.Provider value={contextWrapper}>
			<MemoryRouter>
				<LiveAnnouncer>
					<CommentList {...fakeProps} />
				</LiveAnnouncer>
			</MemoryRouter>
		</UserContext.Provider>,
	);
	await waitForElementToBeRemoved(() => screen.getByText("Loading..."));
	expect(container.querySelectorAll("textarea").length).toEqual(5);
});

test("should match snapshot post login, with 5 personal comments and 5 read only comments from others", async () => {
	mock.onGet(generateUrl("comments", undefined, [], {	sourceURI: fakeProps.match.url }))
		.reply(200, sampleComments);
	mock.onGet(generateUrl("commentsForOtherOrgCommenters", undefined, [], { sourceURI: fakeProps.match.url }))
		.reply(200, sampleComments);
	const updatedContextWrapper = Object.assign({}, contextWrapper);
	updatedContextWrapper.isOrganisationCommenter = true;
	const {container} = render(
		<UserContext.Provider value={updatedContextWrapper}>
			<MemoryRouter>
				<LiveAnnouncer>
					<CommentList {...fakeProps} />
				</LiveAnnouncer>
			</MemoryRouter>
		</UserContext.Provider>,
	);
	await waitForElementToBeRemoved(() => screen.getByText("Loading..."));
	expect(container).toMatchSnapshot();
});

test("should hit the commentsForOtherOrgCommenters endpoint (when using the code) and populate comment list with read only comments", async () => {
	mock.onGet(generateUrl("comments", undefined, [], {	sourceURI: fakeProps.match.url }))
		.reply(200, emptyCommentsResponse);
	mock.onGet(generateUrl("commentsForOtherOrgCommenters", undefined, [], { sourceURI: fakeProps.match.url }))
		.reply(200, sampleComments);
	const updatedContextWrapper = Object.assign({}, contextWrapper);
	updatedContextWrapper.isOrganisationCommenter = true;
	const {container} = render(
		<UserContext.Provider value={updatedContextWrapper}>
			<MemoryRouter>
				<LiveAnnouncer>
					<CommentList {...fakeProps} />
				</LiveAnnouncer>
			</MemoryRouter>
		</UserContext.Provider>,
	);
	await waitForElementToBeRemoved(() => screen.getByText("Loading..."));
	const textAreasDisabled = container.querySelectorAll("textarea[disabled]");
	expect(textAreasDisabled.length).toEqual(5);
	expect(screen.queryAllByRole("button", { name: "Delete" }).length).toEqual(0);
});

test("should hit the commentsForOtherOrgCommenters endpoint (when org commenter) and populate question list with read only comments", async () => {
	mock.onGet(generateUrl("comments", undefined, [], {	sourceURI: fakeProps.match.url }))
		.reply(200, sampleQuestionsWithNoAnswers);
	mock.onGet(generateUrl("commentsForOtherOrgCommenters", undefined, [], { sourceURI: fakeProps.match.url }))
		.reply(200, sampleQuestions);
	const updatedContextWrapper = Object.assign({}, contextWrapper);
	updatedContextWrapper.isOrganisationCommenter = true;
	const {container} = render(
		<UserContext.Provider value={updatedContextWrapper}>
			<MemoryRouter>
				<LiveAnnouncer>
					<CommentList {...fakeProps} />
				</LiveAnnouncer>
			</MemoryRouter>
		</UserContext.Provider>,
	);
	await waitForElementToBeRemoved(() => screen.getByText("Loading..."));
	const textAreasDisabledAmount = container.querySelectorAll("textarea[disabled]").length;
	const textAreasEnabledAmount = container.querySelectorAll("textarea").length - textAreasDisabledAmount;
	// new answer boxes appear as the user hasn't added any themselves
	expect(textAreasEnabledAmount).toEqual(2);
	// readonly / disabled answer boxe for three existing questions
	expect(textAreasDisabledAmount).toEqual(3);
});

test("should call createQuestionPDF with title, end date, and questions when the download questions button is clicked", async () => {
	mock.onGet("/consultations/api/Comments?sourceURI=%2F1%2F1%2Fintroduction")
		.reply(200, sampleComments)
		.onGet("/consultations/api/Consultation?consultationId=1&isReview=false")
		.reply(200, sampleConsultation);

	const questionsForPDF = sampleComments.questions;
	const titleForPDF = sampleConsultation.title;
	const endDate = sampleComments.consultationState.endDate;
	const getTitleFunction = () => { return titleForPDF;};
	render(
		<UserContext.Provider value={contextWrapper}>
			<MemoryRouter>
				<LiveAnnouncer>
					<CommentList {...fakeProps} getTitleFunction={getTitleFunction} />
				</LiveAnnouncer>
			</MemoryRouter>
		</UserContext.Provider>
	);
	await waitForElementToBeRemoved(() => screen.getByText("Loading..."));
	const button = screen.getByRole("button", { name: "Download questions (PDF)" });
	fireEvent.click(button);
	expect(createQuestionPdf).toHaveBeenCalledTimes(1);
	expect(createQuestionPdf).toHaveBeenCalledWith(questionsForPDF, titleForPDF, endDate);
});
