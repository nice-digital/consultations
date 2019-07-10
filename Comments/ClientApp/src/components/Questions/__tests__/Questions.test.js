/* global jest */

import React from "react";
import { shallow, mount } from "enzyme";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { MemoryRouter } from "react-router";
import { nextTick } from "../../../helpers/utils";
import toJson from "enzyme-to-json";
import QuestionsData from "./QuestionsData.json";
import { Questions } from "../Questions";
import { TextQuestion } from "../../QuestionTypes/TextQuestion/TextQuestion";
import { LiveAnnouncer } from "react-aria-live";

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

			expect(wrapper.find(<TextQuestion/>).length).toEqual(0);
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

			const length = wrapper.find(TextQuestion).length;

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

		it("should add a text question when the text question button is clicked", async () => {
			const localProps = Object.assign({}, fakeProps);
			localProps.match.params.documentId = "consultation";

			const wrapper = mount(
				<MemoryRouter>
					<Questions {...localProps}/>
				</MemoryRouter>,
			);

			await nextTick();
			wrapper.update();

			// existing question from the data
			expect(wrapper.find(TextQuestion).length).toEqual(1);

			const textQuestionButton = wrapper.find("button[children='Add text response question']");

			textQuestionButton.simulate("click");

			await nextTick();
			wrapper.update();

			expect(wrapper.find(TextQuestion).length).toEqual(2);
		});

		it("should add a yes / no question when the yes / no button is clicked", () => {
			expect(1).toEqual(2);
		});

		it("should render an add question button for each of the question types in the data", function () {
			expect(1).toEqual(2);
		});

	});
});
