import React from "react";
import { fireEvent, render, screen, within } from "@testing-library/react";
import { FilterOption } from "../FilterOption";

it("appends query string to href when checkbox is selected", () => {
	const historyMock = { push: jest.fn() };
	const optionModel = {
		id: "oId",
		isSelected: false,
	};
	render(<FilterOption groupId="gId" option={optionModel} path="/test" history={historyMock} />);
	const checkbox = screen.getByRole("checkbox");
	fireEvent.click(checkbox);
	expect(checkbox.checked).toEqual(true);
	expect(historyMock.push).toHaveBeenCalledWith("/test?gId=oId");
});

it("removes query string from href when checkbox is unselected", () => {
	const historyMock = { push: jest.fn() };
	const optionModel = {
		id: "oId",
		isSelected: true,
	};
	render(<FilterOption groupId="gId" option={optionModel} path="/test?gId=oId" history={historyMock} />);
	const checkbox = screen.getByRole("checkbox");
	fireEvent.click(checkbox);
	expect(checkbox.checked).toEqual(false);
	expect(historyMock.push).toHaveBeenCalledWith("/test");
});


it("renders a checkbox wrapped in a label", () => {
	const optionModel = {
		id: "oId",
		isSelected: false,
		label: "Test",
	};
	render(<FilterOption groupId="gId" option={optionModel} />);
	const filterOption = screen.getByText("Test", { selector: "label" });
	const { queryAllByRole } = within(filterOption);
	expect(queryAllByRole("checkbox").length).toEqual(1);
});

it("label for attribute matches checkbox id", () => {
	const optionModel = {
		id: "oId",
		isSelected: false,
		label: "Test",
	};
	render(<FilterOption groupId="gId" option={optionModel} />);
	const label = screen.getByText("Test", { selector: "label" });
	const input = screen.getByRole("checkbox");
	expect(label.getAttribute("for")).toEqual(input.getAttribute("id"));
});

it("label element text is option prop label", () => {
	const optionModel = {
		label: "Test",
	};
	render(<FilterOption option={optionModel} />);
	const label = screen.getByText("Test", { selector: "label" });
	expect(label).toBeInTheDocument();
});
