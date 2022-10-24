import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import { FilterGroup } from "../FilterGroup";

jest.mock("../../HistoryContext/HistoryContext", () => {
	return {
		withHistory: (Component) => {
			return Component;
		},
	};
});

const zeroResultsOptions = [
	{
		id: "0",
		label: "Zero - no results",
		isSelected: false,
		filteredResultCount: 0,
		unFilteredResultCount: 0,
	},
];

const selectedOptions = [
	{
		id: "1",
		label: "One - selected",
		isSelected: true,
		filteredResultCount: 0,
		unfilteredResultCount: 1,
	},
	{
		id: "2",
		label: "Two - selected",
		isSelected: true,
		filteredResultCount: 0,
		unfilteredResultCount: 1,
	},
];

const unSelectedOptions = [
	{
		id: "3",
		label: "Three - unselected",
		isSelected: false,
		filteredResultCount: 0,
		unfilteredResultCount: 1,
	},
	{
		id: "4",
		label: "Four - unselected",
		isSelected: false,
		filteredResultCount: 0,
		unfilteredResultCount: 1,
	},
];

const optionsWithResults = [].concat(selectedOptions, unSelectedOptions);

const filterGroupModel = {
	id: "gId",
	title: "Test",
	options: [].concat(zeroResultsOptions, optionsWithResults),
};

test("returns correct number of selected options", () => {
	render(<FilterGroup filterGroup={filterGroupModel} />);
	expect(screen.getByText("2 selected")).toBeInTheDocument();
});

test("returns options that have at least 1 result", () => {
	render(<FilterGroup filterGroup={filterGroupModel} />);
	const filterOptions = screen.queryAllByRole("checkbox");
	expect(filterOptions.length).toEqual(4);
});


test("clicking heading button collapses expanded group", () => {
	render(<FilterGroup filterGroup={filterGroupModel} />);
	const filterGroupHeadingBtn = screen.getByRole("button");
	const filterGroupOptions = screen.getByRole("group");
	expect(filterGroupHeadingBtn.getAttribute("aria-expanded")).toEqual("true");
	expect(filterGroupOptions.getAttribute("aria-hidden")).toEqual("false");
	fireEvent.click(filterGroupHeadingBtn);
	expect(filterGroupHeadingBtn.getAttribute("aria-expanded")).toEqual("false");
	expect(filterGroupOptions.getAttribute("aria-hidden")).toEqual("true");
});

it("group title renders as a button on the client", () => {
	render(<FilterGroup filterGroup={filterGroupModel} />);
	expect(screen.getByRole("button").textContent).toBe("Filter by Test2 selected");
});

it("group is aria labelled by the heading id", () => {
	render(<FilterGroup filterGroup={filterGroupModel} />);
	const expectedId = "group-title-gId";
	expect(screen.getByRole("group").getAttribute("aria-labelledby")).toEqual(expectedId);
});

it("heading button aria controls the group of options", () => {
	render(<FilterGroup filterGroup={filterGroupModel} />);
	const expectedGroupId = "group-gId";
	expect(screen.getByRole("button").getAttribute("aria-controls")).toEqual(expectedGroupId);
});

it("group is initially expanded when it has selected results", () => {
	render(<FilterGroup filterGroup={filterGroupModel} />);
	expect(screen.getByRole("button").getAttribute("aria-expanded")).toEqual("true");
});

it("renders list of filter options", () => {
	render(<FilterGroup filterGroup={filterGroupModel} />);
	const filterOptions = screen.queryAllByRole("checkbox");
	optionsWithResults.map((option, index) => {
		expect(filterOptions[index].getAttribute("title")).toBe(`${filterGroupModel.title} - ${option.label}`);
	});
});
