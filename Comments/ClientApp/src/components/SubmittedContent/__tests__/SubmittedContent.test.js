import React from "react";
import { render } from "@testing-library/react";
import { MemoryRouter } from "react-router";
import { SubmittedContent } from "../SubmittedContent";

const fakeProps = {
	organisationName: "My Org",
	isOrganisationCommenter: false,
	isLead: false,
	consultationState:{
		supportsDownload: true,
		leadHasBeenSentResponse: false,
	},
	consultationId: 1,
	basename: "something",
	isSubmitted: true,
	linkToReviewPage: false,
};

Date.now = jest.fn(() => new Date(Date.UTC(2022, 7, 9, 8)).valueOf());

test("should match snapshot for individual commenters when displayed on the review page", () => {
	const {container} = render(<SubmittedContent {...fakeProps} />);
	expect(container).toMatchSnapshot();
});

test("should match snapshot for individual commenters when displayed on the submitted page", () => {
	let localProps = {...fakeProps};
	localProps.linkToReviewPage = true;
	const {container} = render(<SubmittedContent {...localProps} />, {wrapper: MemoryRouter});
	expect(container).toMatchSnapshot();
});

test("should match snapshot for organisation commenter when displayed on the review page", () => {
	let localProps = {...fakeProps};
	localProps.isOrganisationCommenter = true;
	const {container} = render(<SubmittedContent {...localProps} />);
	expect(container).toMatchSnapshot();
});

test("should match snapshot for organisation commenter when displayed on the submitted page", () => {
	const localProps = {...fakeProps};
	localProps.isOrganisationCommenter = true;
	localProps.linkToReviewPage = true;
	const {container} = render(<SubmittedContent {...localProps} />, {wrapper: MemoryRouter});
	expect(container).toMatchSnapshot();
});

test("should match snapshot for organisation lead when displayed on the review page without any responses from their org", () => {
	const localProps = {...fakeProps};
	localProps.isLead = true;
	const {container} = render(<SubmittedContent {...localProps} />, {wrapper: MemoryRouter});
	expect(container).toMatchSnapshot();
});

test("should match snapshot for organisation lead when displayed on the review page with responses from their org", () => {
	const localProps = {...fakeProps};
	localProps.isLead = true;
	localProps.consultationState.leadHasBeenSentResponse = true;
	const {container} = render(<SubmittedContent {...localProps} />, {wrapper: MemoryRouter});
	expect(container).toMatchSnapshot();
});

test("should match snapshot for organisation lead when displayed on the submitted page", () => {
	const localProps = {...fakeProps};
	localProps.isLead = true;
	localProps.linkToReviewPage = true;
	localProps.consultationState.leadHasBeenSentResponse = true;
	const {container} = render(<SubmittedContent {...localProps} />, {wrapper: MemoryRouter});
	expect(container).toMatchSnapshot();
});
