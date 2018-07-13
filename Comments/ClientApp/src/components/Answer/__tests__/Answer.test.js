/* global jest */

import React from "react";
import { shallow, mount } from "enzyme";
// import {MemoryRouter} from "react-router";
import { Question } from "../../Question/Question";
import { Answer } from "../Answer";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { nextTick } from "../../../helpers/utils";
import answerWithAnswer from "./answerWithAnswer.json";
import answerWithoutAnswer from "./answerWithoutAnswer.json";
// import questionWithAnswer from "./questionWithAnswer.json";
// import questionWithoutAnswer from "./questionWithoutAnswer.json";
import toJson from "enzyme-to-json";

const mock = new MockAdapter(axios);

describe("[ClientApp] ", () => {
	describe("Answer Component", () => {

		const answerPropsWithAnswer = {
			isVisible: true,
			answer: answerWithAnswer,
			readOnly: false,
			saveHandler: jest.fn(),
			deleteHandler: jest.fn(),
			unique: "string"
		};

		const answerPropsWithoutAnswer = {
			isVisible: true,
			answer: answerWithoutAnswer,
			readOnly: false,
			saveHandler: jest.fn(),
			deleteHandler: jest.fn(),
			unique: "string"
		};

		// const questionPropsWithAnswer = {
		// 	readOnly: false,
		// 	isVisible: true,
		// 	key: 1,
		// 	unique: "unique",
		// 	question: questionWithAnswer
		// };

		// const questionPropsWithoutAnswer = {
		// 	readOnly: false,
		// 	isVisible: true,
		// 	key: 1,
		// 	unique: "unique",
		// 	question: questionWithoutAnswer
		// };

		it("sets text area with comment text correctly", () => {
			const wrapper = shallow(<Answer {...answerPropsWithAnswer} />);
			expect(wrapper.find("textarea").length).toEqual(1);
			expect(wrapper.find("textarea").props().value).toEqual("some answer text");
		});

		it("unsavedChanges state is updated correctly on text area change", () => {
			const wrapper = mount(<Answer {...answerPropsWithAnswer} />);
			expect(wrapper.state().unsavedChanges).toEqual(false);
			const textArea = wrapper.find("textarea");
			textArea.simulate("change", {
				target: {
					value: "an updated answer"
				}
			});
			expect(wrapper.state().answer.answerText).toEqual("an updated answer");
			expect(wrapper.state().unsavedChanges).toEqual(true);
		});

		it("should update UnsavedChanges if lastupdateddate has changed", () => {
			const wrapper = mount(<Answer {...answerPropsWithAnswer} />);
			wrapper.setState({unsavedChanges: true});
			const updatedProps = {
				answer: {
					answerId: answerWithoutAnswer.answerId,
					answerText: "an updated answer",
					lastModifiedDate: new Date("02/04/2018").toISOString()
				}
			};
			wrapper.setProps(updatedProps);
			expect(wrapper.state().unsavedChanges).toEqual(false);
		});

		it("should not update UnsavedChanges if lastupdateddate has not changed", () => {
			const wrapper = mount(<Answer {...answerPropsWithAnswer} />);
			wrapper.setState({unsavedChanges: true});
			wrapper.setProps(answerPropsWithAnswer);
			expect(wrapper.state().unsavedChanges).toEqual(true);
		});

		it("updated comment text in state after new props received", () => {
			const wrapper = mount(<Answer {...answerPropsWithAnswer} />);
			const updatedProps = {
				answer: {
					answerId: answerWithoutAnswer.answerId,
					answerText: "an updated answer"
				}
			};
			wrapper.setProps(updatedProps);
			expect(wrapper.state().answer.answerText).toEqual("an updated answer");
		});

		it("should match snapshot with answer", () => {
			const wrapper = mount(
				<Answer {...answerPropsWithAnswer} />					
			);

			expect(toJson(wrapper, {
				noKey: true,
				mode: "deep"
			})).toMatchSnapshot();			
		});

		it("should match snapshot without answer", () => {
			const wrapper = mount(
				<Answer {...answerPropsWithoutAnswer} />					
			);

			expect(toJson(wrapper, {
				noKey: true,
				mode: "deep"
			})).toMatchSnapshot();			
		});
	});
});
