import React from "react";
import { render, screen } from "@testing-library/react";
import { LiveAnnouncer } from "react-aria-live";
import { DownloadResultsInfo } from "../DownloadResultsInfo";

test("product count shows 1 comment and 1 question when not loading", () => {
	render(<LiveAnnouncer><DownloadResultsInfo consultationCount={1} paginationPositions={{ start: 0, finish: 1 }} appliedFilters={[]} path="" history={null} isLoading={false} /></LiveAnnouncer>);
	expect(screen.getByText("Showing 1 consultation", { selector: "span" })).toBeInTheDocument();
});

test("product count shows plural with comments and questions when not loading", () => {
	render(<LiveAnnouncer><DownloadResultsInfo consultationCount={51} paginationPositions={{ start: 0, finish: 25 }} appliedFilters={[]} path="" history={null} isLoading={false} /></LiveAnnouncer>);
	expect(screen.getByText("Showing 1 to 25 of 51 consultations", { selector: "span" })).toBeInTheDocument();
});
