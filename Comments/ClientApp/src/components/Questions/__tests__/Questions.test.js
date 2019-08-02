/* global jest */

import React from "react";
import { mount } from "enzyme";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { MemoryRouter } from "react-router";
import { nextTick } from "../../../helpers/utils";
import toJson from "enzyme-to-json";
import QuestionsData from "./QuestionsData.json";
import { Questions, AddQuestionButton } from "../Questions";
import { OpenQuestion } from "../../QuestionTypes/OpenQuestion/OpenQuestion";

const textQuestionId = QuestionsData.questionTypes[0].questionTypeId;
const yesNoQuestionId = QuestionsData.questionTypes[1].questionTypeId;

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

describe("[ClientApp] ", () => {
	describe("Question administration Component", () => {

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

		it("should match the supplied snapshot", async () => {
			const wrapper = mount(
				<MemoryRouter>
					<Questions {...fakeProps}/>
				</MemoryRouter>,
			);

			await nextTick();
			wrapper.update();

			expect(
				toJson(wrapper, {
					noKey: true,
					mode: "deep",
				}),
			).toMatchSnapshot();
		});

		it("should not render an add question button at the default URL (e.g. 'questions/1)", async () => {
			const wrapper = mount(
				<MemoryRouter>
					<Questions {...fakeProps}/>
				</MemoryRouter>,
			);

			await nextTick();
			wrapper.update();

			expect(wrapper.find(<OpenQuestion/>).length).toEqual(0);
		});

		it("should render a single consultation question", async () => {
			const localProps = Object.assign({}, fakeProps);
			localProps.match.params.documentId = "consultation";

			const wrapper = mount(
				<MemoryRouter>
					<Questions {...localProps}/>
				</MemoryRouter>,
			);

			await nextTick();
			wrapper.update();
			const length = wrapper.find(OpenQuestion).length;

			expect(length).toEqual(1);
		});

		it("should render a marker with the correct quantity of questions", async () => {
			const localProps = Object.assign({}, fakeProps);
			localProps.match.params.documentId = "consultation";

			const wrapper = mount(
				<MemoryRouter>
					<Questions {...localProps}/>
				</MemoryRouter>,
			);

			await nextTick();
			wrapper.update();

			const text = wrapper.find("[aria-current='page']").find("span.text-right").text();
			expect(text).toEqual("(1)");
		});

		it("should increment the question count marker when the add question button is clicked ", async () => {
			const localProps = Object.assign({}, fakeProps);
			localProps.match.params.documentId = "consultation";

			const wrapper = mount(
				<MemoryRouter>
					<Questions {...localProps}/>
				</MemoryRouter>,
			);

			await nextTick();
			wrapper.update();

			expect(wrapper.find("[aria-current='page']").find("span.text-right").text()).toEqual("(1)");

			wrapper.find(".btn--cta").first().simulate("click");

			await nextTick();
			wrapper.update();

			expect(wrapper.find("[aria-current='page']").find("span.text-right").text()).toEqual("(2)");
		});

		it("get consultation level questions", () => {
			const thing = new Questions();
			const result = thing.getQuestionsToDisplay(
				"consultation",
				QuestionsData,
			);
			expect(result).toEqual(QuestionsData.consultationQuestions);
		});

		it("get document level questions", () => {
			const thing = new Questions();
			const result = thing.getQuestionsToDisplay(
				"1",
				QuestionsData,
			);
			expect(result).toEqual(QuestionsData.documents[0].documentQuestions);
		});

		it("should add a blank open question to state when the add open question button is clicked", async () => {
			const localProps = Object.assign({}, fakeProps);
			localProps.match.params.documentId = "consultation";
			const wrapper = mount(<MemoryRouter><Questions {...localProps}/></MemoryRouter>);
			await nextTick();
			wrapper.update();
			const questionInstance = wrapper.find(Questions).instance();
			expect(questionInstance.state.questionsData.consultationQuestions.length).toEqual(1);
			const textQuestionButton = wrapper.find("button[children='Add an open question']");
			textQuestionButton.simulate("click");
			await nextTick();
			wrapper.update();
			expect(questionInstance.state.questionsData.consultationQuestions.length).toEqual(2);
			expect(questionInstance.state.questionsData.consultationQuestions[1].questionTypeId).toEqual(textQuestionId);
		});

		it("should add a yes / no question when the yes / no button is clicked", async () => {
			const localProps = Object.assign({}, fakeProps);
			localProps.match.params.documentId = "consultation";
			const wrapper = mount(<MemoryRouter><Questions {...localProps}/></MemoryRouter>);
			await nextTick();
			wrapper.update();
			const questionInstance = wrapper.find(Questions).instance();
			const textQuestionButton = wrapper.find("button[children='Add a yes/no question']");
			textQuestionButton.simulate("click");
			await nextTick();
			wrapper.update();
			expect(questionInstance.state.questionsData.consultationQuestions[1].questionTypeId).toEqual(yesNoQuestionId);
		});

		it("should render an add question button for each of the question types in the data", async () => {
			const localProps = Object.assign({}, fakeProps);
			localProps.match.params.documentId = "consultation";
			const wrapper = mount(<MemoryRouter><Questions {...localProps}/></MemoryRouter>);
			await nextTick();
			wrapper.update();
			expect(wrapper.find(AddQuestionButton).length).toEqual(QuestionsData.questionTypes.length);
		});

	});
});
