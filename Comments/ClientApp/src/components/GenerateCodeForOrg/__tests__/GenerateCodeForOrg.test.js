import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import { GenerateCodeForOrg } from "../../GenerateCodeForOrg/GenerateCodeForOrg";

it("should show generate button when no code has been generated", () => {
	const fakePropsNoCode = {
		canGenerateCollationCode: true,
		collationCode: null,
		organisationAuthorisationId: 0,
		organisationId: 999,
		organisationName: "Super Magic Club",
		consultationId: 111,
	};
	render(<GenerateCodeForOrg {...fakePropsNoCode} />);
	expect(screen.getByRole("button", { name: "Generate" })).toBeInTheDocument();
});

it("should show the copy button when a code has been generated", () => {
	const fakePropsCode = {
		canGenerateCollationCode: false,
		collationCode: "1234 5678 9123",
		organisationAuthorisationId: 0,
		organisationId: 999,
		organisationName: "Super Magic Club",
		consultationId: 111,
	};
	render(<GenerateCodeForOrg {...fakePropsCode} />);
	expect(screen.getByRole("button", { name: "Copy code" })).toBeInTheDocument();
});

it("should show copied text after the code has been copied", () => {
	const windowPrompt = window.prompt;
  	window.prompt = jest.fn();
	const fakePropsCode = {
		canGenerateCollationCode: false,
		collationCode: "1234 5678 9123",
		organisationAuthorisationId: 0,
		organisationId: 999,
		organisationName: "Super Magic Club",
		consultationId: 111,
	};
	render(<GenerateCodeForOrg {...fakePropsCode} />);
	fireEvent.click(screen.getByRole("button", { name: "Copy code" }));
	const copySuccess = screen.getByRole("button", { name: "Copied" });
	expect(copySuccess).toBeInTheDocument();
	window.prompt = windowPrompt;
});
