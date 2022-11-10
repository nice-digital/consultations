import React from "react";
import { render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { StackedNav } from "../StackedNav";

const props = {
	links: {
		title: "Root Label",
		links: [
			{
				label: "Sub Link 1 Label",
				url: "sub-link-1-url",
				current: true,
				isReactRoute: true,
			},
			{
				label: "Sub Link 2 Label",
				url: "sub-link-2-url",
				isReactRoute: true,
			},
			{
				label: "Sub Link 3 Label",
				url: "sub-link-3-url",
				isReactRoute: true,
			},
			{
				label: "External Link",
				url: "https://external-link.com",
				isReactRoute: false,
			},
		],
	},
};

test("should render a H2 with text that matches the supplied title", () => {
	render(<MemoryRouter><StackedNav {...props} /></MemoryRouter>);
	const titleHeading = screen.getByText("Root Label", { selector: "h2" });
	expect(titleHeading).toBeInTheDocument();
});

test("should render the number of links supplied in props with anchors that match", () => {
	render(<MemoryRouter><StackedNav {...props} /></MemoryRouter>);
	const el = screen.queryAllByRole("link");
	expect(el.length).toEqual(4);
	expect(el[0]).toHaveAttribute("href", "/sub-link-1-url");
	expect(el[2].textContent).toBe("Sub Link 3 Label");
});

test("should render a link with aria-current attribute set if link is current", () => {
	render(<MemoryRouter><StackedNav {...props} /></MemoryRouter>);
	const el = screen.queryAllByRole("link");
	expect(el[0].getAttribute("aria-current")).toEqual("page");
});

test("should render a standard anchor tag if the destination contains http", () => {
	render(<MemoryRouter><StackedNav {...props} /></MemoryRouter>);
	const el = screen.queryAllByRole("link");
	expect(el[el.length - 1].getAttribute("target")).toEqual("_blank");
});
