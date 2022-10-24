import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import { GenerateCode } from "../GenerateCode";

const fakePropsNoCode = {
	organisationCodes: [{
		canGenerateCollationCode: true,
		collationCode: null,
		organisationAuthorisationId: 0,
		organisationId: 999,
		organisationName: "Super Magic Club",
	}],
	consultationId: 111,
};

test("should match snapshot with no code having been generated", () => {
	const {container} = render(<GenerateCode {...fakePropsNoCode} />);
	const shareOrganisationButton = screen.getByRole("button", { name: "Share with organisation" });
	fireEvent.click(shareOrganisationButton);
	expect(container).toMatchSnapshot();
});

test("should match snapshot with code having been generated", () => {
	let fakePropsCode = {
		organisationCodes: [{...fakePropsNoCode.organisationCodes[0]}],
		consultationId: 111,
	};
	fakePropsCode.organisationCodes[0].canGenerateCollationCode = false;
	fakePropsCode.organisationCodes[0].collationCode = "1234 5678 9123";
	const {container} = render(<GenerateCode {...fakePropsCode} />);
	const shareOrganisationButton = screen.getByRole("button", { name: "Share with organisation" });
	fireEvent.click(shareOrganisationButton);
	expect(container).toMatchSnapshot();
});

test("shouldn't show the panel when first loaded", () => {
	render(<GenerateCode {...fakePropsNoCode} />);
	const organisationCodesHeading = screen.queryAllByRole("heading", { name: "Generate a code to share the consultation" });
	expect(organisationCodesHeading.length).toBe(0);
});

test("should show panel when share button is clicked", () => {
	render(<GenerateCode {...fakePropsNoCode} />);
	const shareOrganisationButton = screen.getByRole("button", { name: "Share with organisation" });
	fireEvent.click(shareOrganisationButton);
	const organisationCodesHeading = screen.queryAllByRole("heading", { name: "Generate a code to share the consultation" });
	expect(organisationCodesHeading.length).toBe(1);
});
