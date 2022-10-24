import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import { QuestionsFilter } from "../QuestionsFilter";

it("should fire the handleFilter function when a input is changed", () => {
	const fakeProps = {
		handleFilter: jest.fn(),
		filter: {
			direcorate: true,
		},
	};
	render(<QuestionsFilter {...fakeProps}/>);
	const radio1 = screen.getByLabelText("All Directorates");
	fireEvent.click(radio1);
	expect(fakeProps.handleFilter).toHaveBeenCalled();
});

test("should match the snapshot when the filter is set to directorate", () => {
	const fakeProps = {
		handleFilter: jest.fn(),
		filter: {
			direcorate: true,
		},
	};
	const {container} = render(<QuestionsFilter {...fakeProps} />);
	expect(container).toMatchSnapshot();
});

test("should match the snapshot when the filter is set to all", () => {
	const fakeProps = {
		handleFilter: jest.fn(),
		filter: {
			direcorate: false,
		},
	};
	const {container} = render(<QuestionsFilter {...fakeProps} />);
	expect(container).toMatchSnapshot();
});
