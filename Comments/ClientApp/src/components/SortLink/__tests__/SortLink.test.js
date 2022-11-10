import React from "react";
import { render, screen } from "@testing-library/react";
import { SortLink } from "./../SortLink";

test("renders active text when sort order active", () => {
	render(<SortLink currentSortOrder="One" sortOrder="One" text="Sort name" />);
	expect(screen.getByText("Sort name")).toBeInTheDocument();
});

test("renders anchor with sort querystring appended when inactive", () => {
	render(<SortLink currentSortOrder="One" sortOrder="Two" text="Sort name" path="/test?a=b" />);
	expect(screen.getByText("Sort name", { selector: "a" }).getAttribute("href")).toEqual("/test?a=b&Sort=Two");
});

test("renders visually hidden 'sort by' anchor text", () => {
	render(<SortLink currentSortOrder="One" sortOrder="Two" text="Sort name" path="/test?a=b" />);
	expect(screen.getByText("Sort by", { selector: ".visually-hidden" })).toBeInTheDocument();
});
