import React from "react";
import { fireEvent, render, screen } from "@testing-library/react";
import { SingleQuestion } from "../SingleQuestion";

test("should fire newQuestion when the Insert button is clicked at consultation level", () => {
	const fakeProps1 = {
		questionText: "fake question text",
		newQuestion: jest.fn(),
		currentDocumentId: "consultation",
		currentConsultationId: 1,
		questionTypeId: 3232,
		questionType: {
			type: "Text",
		},
		allRoles: [
			"Administrator",
			"SCSTeam",
		],
	};
	render(<SingleQuestion {...fakeProps1}/>);
	const button = screen.getByText("Insert");
	fireEvent.click(button);
	expect(fakeProps1.newQuestion).toHaveBeenCalled();
});

test("should fire newQuestion when the Insert button is clicked", () => {
	const fakeProps2 = {
		questionText: "fake question text",
		newQuestion: jest.fn(),
		currentDocumentId: 2,
		currentConsultationId: 1,
		questionTypeId: 3232,
		questionType: {
			type: "Text",
		},
		allRoles: [
			"Administrator",
			"SCSTeam",
		],
	};
	render(<SingleQuestion {...fakeProps2}/>);
	const button = screen.getByText("Insert");
	fireEvent.click(button);
	expect(fakeProps2.newQuestion).toHaveBeenCalled();
});

test("should match the snapshot", () => {
	const fakeProps1 = {
		questionText: "fake question text",
		newQuestion: jest.fn(),
		currentDocumentId: "consultation",
		currentConsultationId: 1,
		questionTypeId: 3232,
		questionType: {
			type: "Text",
		},
		allRoles: [
			"Administrator",
			"SCSTeam",
		],
	};
	const {container} = render(<SingleQuestion {...fakeProps1}/>);
	expect(container).toMatchSnapshot();
});
