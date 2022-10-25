import React from "react";
import { fireEvent, render, screen } from "@testing-library/react";
import { Selection } from "../Selection";

const consoleErrorReset = console.error;

beforeEach(() => {
	console.error = consoleErrorReset;
});

test("mouseup event with invalid range should hide tooltip", async () => {
	window.getSelection = () => {
		return {
			removeAllRanges: () => {},
			getRangeAt: () => {},
			isCollapsed: false,
			rangeCount: 0,
		};
	};
	render(<Selection allowComments={true}><p>some paragraph content</p></Selection>);
	expect(screen.queryAllByRole("button").length).toEqual(0);
	const mouseUpDiv = await screen.getByRole("presentation");
	fireEvent.mouseUp(mouseUpDiv);
	expect(screen.queryAllByRole("button").length).toEqual(0);
});

test("mouseup event with valid range should show tooltip", async () => {
	console.error = jest.fn();
	window.getSelection = () => {
		return {
			removeAllRanges: () => {},
			getRangeAt: () => { return "paragraph content";},
			isCollapsed: false,
			rangeCount: 1,
		};
	};
	render(<Selection allowComments={true}><p>some paragraph content</p></Selection>);
	expect(screen.queryAllByRole("button").length).toEqual(0);
	const mouseUpDiv = await screen.getByRole("presentation");
	fireEvent.mouseUp(mouseUpDiv);
	expect(screen.queryAllByRole("button").length).toEqual(1);
});
