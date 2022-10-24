import React from "react";
import { render, screen } from "@testing-library/react";
import { Pagination } from "../Pagination";

const paginationBaseFakeProps = {
	onChangePage: jest.fn(),
	onChangeAmount: jest.fn(),
	consultationCount: 300,
};

const paginationLargeFakeProps = {
	itemsPerPage: 25,
	currentPage: 8,
};

const paginationNoneFakeProps = {
	itemsPerPage: "all",
	currentPage: 1,
};

test("shows first page pager when there are 6 pages or more", () => {
	render(<Pagination {...paginationBaseFakeProps} {...paginationLargeFakeProps} />);
	const pages = screen.queryAllByRole("listitem");
	expect(pages[1]).toHaveClass("first");
});

test("shows last page pager when there are 6 pages or more", () => {
	render(<Pagination {...paginationBaseFakeProps} {...paginationLargeFakeProps} />);
	const pages = screen.queryAllByRole("listitem");
	expect(pages[pages.length - 2]).toHaveClass("last");
});

test("shows previous page pager when there are multiple pages", () => {
	render(<Pagination {...paginationBaseFakeProps} {...paginationLargeFakeProps} />);
	const pages = screen.queryAllByRole("listitem");
	expect(pages[0]).toHaveClass("previous");
});

test("shows next page pager when there are multiple pages", () => {
	render(<Pagination {...paginationBaseFakeProps} {...paginationLargeFakeProps} />);
	const pages = screen.queryAllByRole("listitem");
	expect(pages[pages.length - 1]).toHaveClass("next");
});

test("hides pagers when all is selected in amount dropdown", () => {
	render(<Pagination {...paginationBaseFakeProps} {...paginationNoneFakeProps}/>);
	expect(screen.queryAllByRole("listitem").length).toBe(0);
});

test("shows correct value in amount dropdown", () => {
	render(<Pagination {...paginationBaseFakeProps} {...paginationNoneFakeProps} />);
	const itemsPerPage = screen.getByRole("option", { name: "All" });
	expect(itemsPerPage.selected).toBe(true);
});
