import React from "react";
import { render, screen } from "@testing-library/react";
import { LiveAnnouncer } from "react-aria-live";
import { ResultsInfo } from "../ResultsInfo";

const appliedFilters = [
	{
		groupId: "groupA",
		groupTitle: "Group A",
		optionId: "optionB",
		optionLabel: "Option B",
	},
	{
		groupId: "groupB",
		groupTitle: "Group B",
		optionId: "optionC",
		optionLabel: "Option C",
	},
];

it("product count shows 1 comment and 1 question when not loading", () => {
	render(<LiveAnnouncer><ResultsInfo commentCount={1} showCommentsCount={true} questionCount={1} showQuestionsCount={true}  sortOrder="" appliedFilters={[]} path="" history={null} isLoading={false} /></LiveAnnouncer>);
	expect(screen.getByText("Showing 1 question and 1 comment", { selector: "span" })).toBeInTheDocument();
});

it("product count shows plural with comments and questions when not loading", () => {
	render(<LiveAnnouncer><ResultsInfo commentCount={2} showCommentsCount={true} questionCount={2} showQuestionsCount={true}  sortOrder="" appliedFilters={[]} path="" history={null} isLoading={false} /></LiveAnnouncer>);
	expect(screen.getByText("Showing 2 questions and 2 comments", { selector: "span" })).toBeInTheDocument();
});

it("product count shows questions only when comments not allowed and zero passed", () => {
	render(<LiveAnnouncer><ResultsInfo commentCount={0} showCommentsCount={false} questionCount={2} showQuestionsCount={true}  sortOrder="" appliedFilters={[]} path="" history={null} isLoading={false} /></LiveAnnouncer>);
	expect(screen.getByText("Showing 2 questions", { selector: "span" })).toBeInTheDocument();
});

it("product count shows questions only when comments not allowed and non-zero passed", () => {
	render(<LiveAnnouncer><ResultsInfo commentCount={2} showCommentsCount={false} questionCount={2} showQuestionsCount={true}  sortOrder="" appliedFilters={[]} path="" history={null} isLoading={false} /></LiveAnnouncer>);
	expect(screen.getByText("Showing 2 questions", { selector: "span"})).toBeInTheDocument();
});

it("product count shows comments only when questions not allowed and zero passed", () => {
	render(<LiveAnnouncer><ResultsInfo commentCount={2} showCommentsCount={true} questionCount={0} showQuestionsCount={false}  sortOrder="" appliedFilters={[]} path="" history={null} isLoading={false} /></LiveAnnouncer>);
	expect(screen.getByText("Showing 2 comments", { selector: "span" })).toBeInTheDocument();
});

it("product count shows comments only when questions not allowed and non-zero passed", () => {
	render(<LiveAnnouncer><ResultsInfo commentCount={2} showCommentsCount={true} questionCount={2} showQuestionsCount={false}  sortOrder="" appliedFilters={[]} path="" history={null} isLoading={false} /></LiveAnnouncer>);
	expect(screen.getByText("Showing 2 comments", { selector: "span" })).toBeInTheDocument();
});

it("loading message shows with aria-busy when loading", () => {
	render(<LiveAnnouncer><ResultsInfo commentCount={1} showCommentsCount={true} questionCount={1} showQuestionsCount={true}  sortOrder="" appliedFilters={[]} path="" history={null} isLoading={true} /></LiveAnnouncer>);
	expect(screen.getByText("Loading…", { selector: "[aria-busy]" })).toBeInTheDocument();
});

it("doesn't render applied filter list when no applied filters", () => {
	render(<LiveAnnouncer><ResultsInfo appliedFilters={[]} /></LiveAnnouncer>);
	expect(screen.queryAllByRole("list").length).toEqual(0);
});

it("renders a list of applied filters", () => {
	render(<LiveAnnouncer><ResultsInfo appliedFilters={appliedFilters} path="" /></LiveAnnouncer>);
	expect(screen.getByRole("list").getAttribute("class")).toContain("results-info__filters");
});

it("applied filters links are hidden on print", () => {
	render(<LiveAnnouncer><ResultsInfo appliedFilters={appliedFilters} path="" /></LiveAnnouncer>);
	expect(screen.getByRole("list").getAttribute("class")).toContain("hide-print");
});

it("renders an applied filter component for ever applied filter", () => {
	render(<LiveAnnouncer><ResultsInfo appliedFilters={appliedFilters} path="" /></LiveAnnouncer>);
	expect(screen.queryAllByRole("listitem").length).toEqual(2);
});
