import React from "react";
import { fireEvent, render, screen, waitForElementToBeRemoved, within } from "@testing-library/react";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { MemoryRouter } from "react-router";
import QuestionsData from "./QuestionsData.json";
import { Questions } from "../Questions";

const mock = new MockAdapter(axios);

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
	match: {
		params: {
			consultationId: 1,
		},
	},
	location: {
		pathname: "/admin/questions/1",
	},
};

beforeEach(() => {
	mock
		.onGet()
		.reply(200, QuestionsData);
});

afterEach(() => {
	mock.reset();
});

test("should match the supplied snapshot", async () => {
	const {container} = render(
		<MemoryRouter>
			<Questions {...fakeProps}/>
		</MemoryRouter>,
	);
	await waitForElementToBeRemoved(() => screen.getByText("Loading...", { selector: "h1" }));
	expect(container).toMatchSnapshot();
});

test("should not render an add question button at the default URL (e.g. 'questions/1)", async () => {
	render(
		<MemoryRouter>
			<Questions {...fakeProps}/>
		</MemoryRouter>,
	);
	await waitForElementToBeRemoved(() => screen.getByText("Loading...", { selector: "h1" }));
	const openQuestion = screen.queryAllByRole("heading", { name: "Question 1 - Open Question" });
	expect(openQuestion.length).toEqual(0);
});

test("should render a single consultation question", async () => {
	const localProps = Object.assign({}, fakeProps);
	localProps.match.params.documentId = "consultation";
	render(
		<MemoryRouter>
			<Questions {...localProps}/>
		</MemoryRouter>,
	);
	await waitForElementToBeRemoved(() => screen.getByText("Loading...", { selector: "h1" }));
	const openQuestion = screen.queryAllByRole("heading", { name: "Question 1 - Open Question" });
	expect(openQuestion.length).toEqual(1);
});

test("should render a marker with the correct quantity of questions", async () => {
	const localProps = Object.assign({}, fakeProps);
	localProps.match.params.documentId = "consultation";
	const {container} = render(
		<MemoryRouter>
			<Questions {...localProps}/>
		</MemoryRouter>,
	);
	await waitForElementToBeRemoved(() => screen.getByText("Loading...", { selector: "h1" }));
	const currentMarkerContainer = Array.from(container.querySelectorAll("[aria-current='page']"));
	const { queryAllByText } = within(currentMarkerContainer[0]);
	const stackedNavMarker = queryAllByText("(1)", { selector: "span.text-right"});
	expect(stackedNavMarker.length).toBeGreaterThan(0);
});

test("should increment the question count marker when the add question button is clicked ", async () => {
	const localProps = Object.assign({}, fakeProps);
	localProps.match.params.documentId = "consultation";
	const {container} = render(
		<MemoryRouter>
			<Questions {...localProps}/>
		</MemoryRouter>,
	);
	await waitForElementToBeRemoved(() => screen.getByText("Loading...", { selector: "h1" }));
	const currentMarkerContainer = Array.from(container.querySelectorAll("[aria-current='page']"));
	const { queryAllByText } = within(currentMarkerContainer[0]);
	const stackedNavMarker = queryAllByText("(1)", { selector: "span.text-right"});
	expect(stackedNavMarker.length).toBeGreaterThan(0);
	const addQuestionButton = screen.getByRole("button", { name: "Add an open question" });
	fireEvent.click(addQuestionButton);
	expect(queryAllByText("(2)", { selector: "span.text-right"}).length).toBeGreaterThan(0);
});

test("get consultation level questions", () => {
	const thing = new Questions();
	const result = thing.getQuestionsToDisplay(
		"consultation",
		QuestionsData,
	);
	expect(result).toEqual(QuestionsData.consultationQuestions);
});

test("get document level questions", () => {
	const thing = new Questions();
	const result = thing.getQuestionsToDisplay(
		"1",
		QuestionsData,
	);
	expect(result).toEqual(QuestionsData.documents[0].documentQuestions);
});

test("should render an add question button for each of the question types in the data", async () => {
	const localProps = Object.assign({}, fakeProps);
	localProps.match.params.documentId = "consultation";
	render(<MemoryRouter><Questions {...localProps}/></MemoryRouter>);
	await waitForElementToBeRemoved(() => screen.getByText("Loading...", { selector: "h1" }));
	const addOpenQuestionButton = screen.queryAllByRole("button", { name: "Add an open question" });
	const addYesNoQuestionButton = screen.queryAllByRole("button", { name: "Add a yes/no question" });
	expect(addOpenQuestionButton.length + addYesNoQuestionButton.length).toEqual(QuestionsData.questionTypes.length);
});
