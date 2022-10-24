import React from "react";
import { render, screen, within } from "@testing-library/react";
import { AppliedFilter } from "../AppliedFilter";

const appliedFilterModel = {
	groupTitle: "Group title",
	optionLabel: "Option label",
	groupId: "gId",
	optionId: "oId",
};

test("renders a tag with remove anchor wrapped in a list item", () => {
	render(<AppliedFilter appliedFilter={appliedFilterModel} path="/test" />);
	const appliedFilter = screen.getByRole("listitem");
	const { queryAllByRole } = within(appliedFilter);
	const filterRemoveTag = queryAllByRole("link");
	expect(filterRemoveTag.length).toEqual(1);
});

test("remove anchor has visually hidden link text for screenreaders", () => {
	render(<AppliedFilter appliedFilter={appliedFilterModel} path="/test" />);
	const filterTitle = `Remove ‘${appliedFilterModel.groupTitle}: ${appliedFilterModel.optionLabel}’ filter`;
	expect(screen.getByText(filterTitle)).toBeInTheDocument();
});

test("tag remove anchor href is set correctly", () => {
	render(<AppliedFilter appliedFilter={appliedFilterModel} path="/test?a=b&gId=oId" />);
	const appliedFilter = screen.getByRole("listitem");
	const { getByRole } = within(appliedFilter);
	const filterRemoveTag = getByRole("link");
	expect(filterRemoveTag.getAttribute("href")).toEqual("/test?a=b");
});
