import React from "react";
import { mount } from "enzyme";
import { Question } from "../Question";
import questionWithAnswer from "./questionWithAnswer.json";
import questionWithoutAnswer from "./questionWithoutAnswer.json";
import toJson from "enzyme-to-json";

describe("[ClientApp] ", () => {
	describe("Question Component", () => {
		const fakePropsWithAnswer = {
			readOnly: false,
			isVisible: true,
			key: 1,
			unique: "unique",
			question: questionWithAnswer,
			showAnswer: true,
		};

		const fakePropsWithoutAnswer = {
			readOnly: false,
			isVisible: true,
			key: 1,
			unique: "unique",
			question: questionWithoutAnswer,
			showAnswer: false,
		};

		it("should match snapshot with answer", () => {
			const wrapper = mount(
				<Question {...fakePropsWithAnswer} />,					
			);

			expect(toJson(wrapper, {
				noKey: true,
				mode: "deep",
			})).toMatchSnapshot();			
		});

		it("should match snapshot without answer", () => {
			const wrapper = mount(
				<Question {...fakePropsWithoutAnswer} />,					
			);

			expect(toJson(wrapper, {
				noKey: true,
				mode: "deep",
			})).toMatchSnapshot();			
		});
	});
});
