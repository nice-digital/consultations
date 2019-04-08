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
import {TextQuestion} from "../../QuestionTypes/TextQuestion/TextQuestion";
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

		afterEach(()=>{
			mock.reset();
		});

		it("should match the supplied snapshot", async () => {
			const wrapper = mount(
				<MemoryRouter>
					<Questions {...fakeProps}/>
				</MemoryRouter>
			);

			await nextTick();
			wrapper.update();

			expect(
				toJson(wrapper, {
					noKey: true,
					mode: "deep",
				})
			).toMatchSnapshot();
		});

		it(`should not render an add question button at the default URL (e.g. 'questions/1)`, async () => {
			const wrapper = mount(
				<MemoryRouter>
					<Questions {...fakeProps}/>
				</MemoryRouter>
			);

			await nextTick();
			wrapper.update();

			expect(wrapper.find(<TextQuestion />).length).toEqual(0);
		});

		it(`should render a single consultation question`, async () => {
			const localProps = Object.assign({}, fakeProps);
			localProps.match.params.documentId = "consultation";


			const wrapper = mount(
				<MemoryRouter>
					<Questions {...localProps}/>
				</MemoryRouter>
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
				</MemoryRouter>
			);

			await nextTick();
			wrapper.update();

			const text = wrapper.find(`[aria-current="page"]`).find("span.text-right").text();
			expect(text).toEqual("(1)");

		});

		it("should increment a marker and add a new TextQuestion box when the add question button is clicked ", async () => {
			const localProps = Object.assign({}, fakeProps);
			localProps.match.params.documentId = "consultation";

			const wrapper = mount(
				<MemoryRouter>
					<Questions {...localProps}/>
				</MemoryRouter>
			);

			await nextTick();
			wrapper.update();

			wrapper.find(".btn--cta").simulate("click");

			await nextTick();
			wrapper.update();

			const text = wrapper.find(`[aria-current="page"]`).find("span.text-right").text();
			expect(text).toEqual("(2)");

			const length = wrapper.find(TextQuestion).length;
			expect(length).toEqual(2);
		});

		it("get consultation level questions", ()=>{
			const thing = new Questions();
			const result = thing.getQuestionsToDisplay(
				"consultation",
				QuestionsData
			);
			expect(result).toEqual(QuestionsData.consultationQuestions);
		});

		it("get document level questions", ()=>{
			const thing = new Questions();
			const result = thing.getQuestionsToDisplay(
				"1",
				QuestionsData
			);
			expect(result).toEqual(QuestionsData.documents[0].documentQuestions);
		});

	});
});
