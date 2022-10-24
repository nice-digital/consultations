import React from "react";
import { render } from "@testing-library/react";
import { Question } from "../Question";
import questionWithAnswer from "./questionWithAnswer.json";
import questionWithoutAnswer from "./questionWithoutAnswer.json";

test("should match snapshot with answer", () => {
	const fakePropsWithAnswer = {
		readOnly: false,
		key: 1,
		unique: "unique",
		question: questionWithAnswer,
		showAnswer: true,
	};
	const {container} = render(<Question {...fakePropsWithAnswer} />);
	expect(container).toMatchSnapshot();
});

test("should match snapshot without answer", () => {
	const fakePropsWithoutAnswer = {
		readOnly: false,
		key: 1,
		unique: "unique",
		question: questionWithoutAnswer,
		showAnswer: false,
	};
	const {container} = render(<Question {...fakePropsWithoutAnswer} />);
	expect(container).toMatchSnapshot();
});
