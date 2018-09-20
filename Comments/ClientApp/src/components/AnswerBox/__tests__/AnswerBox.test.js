/* global jest */
import React from "react";
import { shallow, mount } from "enzyme";
import { AnswerBox } from "../AnswerBox";
import answerWithAnswer from "./answerWithAnswer.json";
import answerWithoutAnswer from "./answerWithoutAnswer.json";
import toJson from "enzyme-to-json";

describe("[ClientApp] ", () => {
	describe("AnswerBox Component", () => {

		const answerPropsWithAnswer = {
			isVisible: true,
			answer: answerWithAnswer,
			readOnly: false,
			saveHandler: jest.fn(),
			deleteHandler: jest.fn(),
			unique: "string",
		};

		const answerPropsWithoutAnswer = {
			isVisible: true,
			answer: answerWithoutAnswer,
			readOnly: false,
			saveHandler: jest.fn(),
			deleteHandler: jest.fn(),
			unique: "string",
		};

		it("sets text area with comment text correctly", () => {
			const wrapper = shallow(<AnswerBox {...answerPropsWithAnswer} />);
			expect(wrapper.find("textarea").length).toEqual(1);
			expect(wrapper.find("textarea").props().value).toEqual("some answer text");
		});

		it("should update UnsavedChanges if lastupdateddate has changed", () => {
			const wrapper = mount(<AnswerBox {...answerPropsWithAnswer} />);
			wrapper.setState({unsavedChanges: true});
			const updatedProps = {
				answer: {
					answerId: answerWithoutAnswer.answerId,
					answerText: "an updated answer",
					lastModifiedDate: new Date("02/04/2018").toISOString(),
				},
			};
			wrapper.setProps(updatedProps);
			expect(wrapper.state().unsavedChanges).toEqual(false);
		});

		it("should not update UnsavedChanges if lastupdateddate has not changed", () => {
			const wrapper = mount(<AnswerBox {...answerPropsWithAnswer} />);
			wrapper.setState({unsavedChanges: true});
			wrapper.setProps(answerPropsWithAnswer);
			expect(wrapper.state().unsavedChanges).toEqual(true);
		});

		it("updated comment text in state after new props received", () => {
			const wrapper = mount(<AnswerBox {...answerPropsWithAnswer} />);
			const updatedProps = {
				answer: {
					answerId: answerWithoutAnswer.answerId,
					answerText: "an updated answer",
				},
			};
			wrapper.setProps(updatedProps);
			expect(wrapper.state().answer.answerText).toEqual("an updated answer");
		});

		it("should match snapshot with answer", () => {
			const wrapper = mount(
				<AnswerBox {...answerPropsWithAnswer} />
			);
			expect(toJson(wrapper, {
				noKey: true,
				mode: "deep",
			})).toMatchSnapshot();			
		});

		it("should match snapshot without answer", () => {
			const wrapper = mount(
				<AnswerBox {...answerPropsWithoutAnswer} />
			);
			expect(toJson(wrapper, {
				noKey: true,
				mode: "deep",
			})).toMatchSnapshot();			
		});
	});
});
