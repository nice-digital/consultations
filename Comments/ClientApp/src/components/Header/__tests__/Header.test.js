import React from "react";
import { render } from "@testing-library/react";
import { MemoryRouter } from "react-router";
import { Header } from "../Header";

test("should display correctly when consulation hasn't started", () => {
	const dateNow = Date.now;
	Date.now = jest.fn(() => new Date(Date.UTC(2022, 7, 9, 8)).valueOf());
	const FakeProps = {
		title: "My title",
		subtitle1: "Subtitle 1",
		subtitle2: "Subtitle 2",
		consultationState: {
			endDate: "1994-11-05T08:15:30-05:00",
			consultationIsOpen: false,
			consultationHasNotStartedYet: true,
		},
	};
	const {container} = render(<MemoryRouter><Header {...FakeProps}/></MemoryRouter>);
	expect(container).toMatchSnapshot();
	Date.now = dateNow;
});

test("should display correctly when consulation is in progress", () => {
	const FakeProps = {
		title: "My title",
		subtitle1: "Subtitle 1",
		consultationState: {
			endDate: "1994-11-05T08:15:30-05:00",
			consultationIsOpen: true,
			consultationHasNotStartedYet: false,
		},
	};
	const {container} = render(<MemoryRouter><Header {...FakeProps}/></MemoryRouter>);
	expect(container).toMatchSnapshot();
});

test("should display correctly when consulation has ended", () => {
	const FakeProps = {
		title: "My title has changed",
		subtitle1: "Subtitle 1",
		consultationState: {
			endDate: "1994-11-05T08:15:30-05:00",
			consultationIsOpen: false,
			consultationHasNotStartedYet: false,
		},
	};
	const {container} = render(<MemoryRouter><Header {...FakeProps}/></MemoryRouter>);
	expect(container).toMatchSnapshot();
});
