import React from "react";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { Answer } from "../Answer";
import answerWithAnswer from "./answerWithAnswer.json";
import answerWithoutAnswer from "./answerWithoutAnswer.json";

const answerPropsWithAnswer = {
	answer: answerWithAnswer,
	readOnly: false,
	saveHandler: jest.fn(),
	deleteHandler: jest.fn(),
	unique: "string",
	updateUnsavedIds: jest.fn(),
	questionType: {
		type: "Text",
	},
};

const answerPropsWithoutAnswer = {
	answer: answerWithoutAnswer,
	readOnly: false,
	saveHandler: jest.fn(),
	deleteHandler: jest.fn(),
	unique: "string",
	updateUnsavedIds: jest.fn(),
	questionType: {
		type: "Text",
	},
};

test("sets text area with comment text correctly", async () => {
	render(<Answer {...answerPropsWithAnswer} />);
	expect(screen.getByDisplayValue("some answer text")).toBeInTheDocument();
});

test("unsavedChanges function is fired correctly on text area change", async () => {
	render(<Answer {...answerPropsWithAnswer} />);
	const textArea = screen.getByDisplayValue("some answer text");
	const user = userEvent.setup();
	textArea.focus();
	await user.type(textArea, " that's been updated");
	user.tab();
	expect(screen.getByDisplayValue("some answer text that's been updated")).toBeInTheDocument();
	expect(answerPropsWithAnswer.updateUnsavedIds).toHaveBeenCalledWith("22q", true);
});

test("should match snapshot with answer", () => {
	const {container} = render(<Answer {...answerPropsWithAnswer} />);
	expect(container).toMatchSnapshot();
});

test("should match snapshot without answer", () => {
	const {container} = render(<Answer {...answerPropsWithoutAnswer} />);
	expect(container).toMatchSnapshot();
});
