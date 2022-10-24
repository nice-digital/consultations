import React from "react";
import { render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router";
import { ConsultationItem } from "../ConsultationItem";

test("does not render link if document id or chapter slug is null", async () => {
	window.__PRELOADED__ = { isAuthorised: true};
	const fakeProps = {
		title:"Consultation Title",
		startDate : new Date("1995-12-17T03:24:00"),
		endDate : new Date("2059-12-17T03:24:00"),
		submissionCount:1,
		consultationId :123,
		documentId:1,
		chapterSlug :"introduction",
		gidReference :"TA-123",
		productTypeName :"TA",
		isOpen: true,
		isClosed: false,
		isUpcoming: false,
		show: true,
		basename: "fdfd",
		showShareWithOrganisationButton: true,
		allowGenerateOrganisationCode: true,
		submissionToLeadCount: 15,
	};
	render(<MemoryRouter><ConsultationItem {...fakeProps} chapterSlug={null} documentId={null} /></MemoryRouter>);
	const consultationLink = screen.queryAllByText(fakeProps.title, { selector: "a" });
  	expect(consultationLink.length).toEqual(0);
});

test("should match snapshot with supplied data", () => {
	window.__PRELOADED__ = { isAuthorised: true};
	const fakeProps = {
		title:"Consultation Title",
		startDate : new Date("1995-12-17T03:24:00"),
		endDate : new Date("2059-12-17T03:24:00"),
		submissionCount:1,
		consultationId :123,
		documentId:1,
		chapterSlug :"introduction",
		gidReference :"TA-123",
		productTypeName :"TA",
		isOpen: true,
		isClosed: false,
		isUpcoming: false,
		show: true,
		basename: "fdfd",
		showShareWithOrganisationButton: true,
		allowGenerateOrganisationCode: true,
		submissionToLeadCount: 15,
	};
	const {container} = render(<MemoryRouter><ConsultationItem {...fakeProps} /></MemoryRouter>);
	expect(container).toMatchSnapshot();
});

