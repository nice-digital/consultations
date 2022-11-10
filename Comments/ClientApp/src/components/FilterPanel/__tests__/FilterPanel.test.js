import React from "react";
import { render, screen } from "@testing-library/react";
import { FilterPanel } from "../FilterPanel";

test("returns groups that have at least one selected option", () => {
	const filters = [
		{
			id: "a",
			options: [{ isSelected: true, filteredResultCount: 0 },	{ isSelected: false, filteredResultCount: 1 }],
		},
		{
			id: "b", options: [ { isSelected: false, filteredResultCount: 0 } ],
		},
	];
	render(<FilterPanel filters={filters} />);
	const groupsToDisplay = screen.queryAllByRole("group", { selector: "filter-group__options" });
	expect(groupsToDisplay.length).toEqual(1);
	expect(groupsToDisplay[0].getAttribute("id")).toEqual("group-a");
});

test("returns groups that have at least one option with filtered results", () => {
	const filters = [
		{
			id: "a",
			options: [ { filteredResultCount: 0 } ],
		},
		{
			id: "b",
			options: [{ filteredResultCount: 1 }, { filteredResultCount: 0 }],
		},
	];
	render(<FilterPanel filters={filters} />);
	const groupsToDisplay = screen.queryAllByRole("group", { selector: "filter-group__options" });
	expect(groupsToDisplay.length).toEqual(1);
	expect(groupsToDisplay[0].getAttribute("id")).toEqual("group-b");
});

test("doesn't return groups that have no selected option or options with results", () => {
	const filters = [
		{
			id: "a",
			options: [{ filteredResultCount: 0 }],
		},
		{
			id: "b",
			options: [{ filteredResultCount: 0 }],
		},
	];
	render(<FilterPanel filters={filters} />);
	const groupsToDisplay = screen.queryAllByRole("group", { selector: "filter-group__options" });
	expect(groupsToDisplay.length).toEqual(0);
});

test("renders list of filter groups", () => {
	const filters = [{ id: "a", options: [{ isSelected: true }] }, { id: "b", options: [{ filteredResultCount: 1 }] }];
	render(<FilterPanel filters={filters} />);
	const filterGroupOptions = screen.queryAllByRole("group", { selector: "filter-group__options" });
	expect(filterGroupOptions.length).toEqual(2);
});

test("doesn't render submit filters button when mounted", () => {
	render(<FilterPanel filters={[]} />);
	const button = screen.queryAllByText("Apply filters", { selector: "button.filter-panel__submit" });
	expect(button.length).toEqual(0);
});
