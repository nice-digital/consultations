import React from "react";
import { fireEvent, render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router";
import { UserContext } from "../../../context/UserContext";
import { LoginPanel } from "../LoginPanel";

const loginPanelFakeProps = {
	match: {
		url: "/1/1/introduction",
	},
};

test("shows first menu screen when first loaded", () => {
	render(
		<UserContext.Provider value={{}}>
			<LoginPanel {...loginPanelFakeProps} />
		</UserContext.Provider>,
	);
	expect(screen.getByRole("heading", { name: "How are you taking part in this consultation?" })).toBeInTheDocument();
});

it("shows second menu screen when organisation option has been selected", () => {
	render(
		<UserContext.Provider value={{}}>
			<LoginPanel { ...loginPanelFakeProps } />
		</UserContext.Provider>,
	);
	const radioButton = screen.getByLabelText("As part of an organisation");
	expect(screen.getByRole("heading", { name: "How are you taking part in this consultation?" })).toBeInTheDocument();
	fireEvent.click(radioButton);
	expect(screen.queryAllByRole("heading", { name: "How are you taking part in this consultation?" }).length).toBe(0);
	expect(screen.getByRole("heading", { name: "Do you have a code from your organisation?" })).toBeInTheDocument();
});

it("shows login when individual option has been selected", () => {
	render(
		<UserContext.Provider value={{}}>
			<MemoryRouter>
				<LoginPanel { ...loginPanelFakeProps } />
			</MemoryRouter>
		</UserContext.Provider>,
	);
	const radioButton = screen.getByLabelText("As an individual");
	expect(screen.getByRole("heading", { name: "How are you taking part in this consultation?" })).toBeInTheDocument();
	fireEvent.click(radioButton);
	expect(screen.queryAllByRole("heading", { name: "How are you taking part in this consultation?" }).length).toBe(0);
	expect(screen.getByRole("heading", { name: "Make or review comments as an individual" })).toBeInTheDocument();
});

it("shows second screen and organisation code text box when 'user has code' is selected", () => {
	render(
		<UserContext.Provider value={{}}>
			<MemoryRouter>
				<LoginPanel { ...loginPanelFakeProps } />
			</MemoryRouter>
		</UserContext.Provider>,
	);
	const radioButton = screen.getByLabelText("As part of an organisation");
	expect(screen.getByRole("heading", { name: "How are you taking part in this consultation?" })).toBeInTheDocument();
	fireEvent.click(radioButton);
	expect(screen.queryAllByRole("heading", { name: "How are you taking part in this consultation?" }).length).toBe(0);
	expect(screen.getByRole("heading", { name: "Do you have a code from your organisation?" })).toBeInTheDocument();
	const radioButton2 = screen.getByLabelText("Yes, I have a code provided by my organisation");
	fireEvent.click(radioButton2);
	expect(screen.getByLabelText("Enter your organisation code")).toBeInTheDocument();
});

it("goes back to previous screen when back has been clicked", () => {
	render(
		<UserContext.Provider value={{}}>
			<MemoryRouter>
				<LoginPanel { ...loginPanelFakeProps } />
			</MemoryRouter>
		</UserContext.Provider>,
	);
	const radioButton = screen.getByLabelText("As an individual");
	expect(screen.getByRole("heading", { name: "How are you taking part in this consultation?" })).toBeInTheDocument();
	fireEvent.click(radioButton);
	expect(screen.queryAllByRole("heading", { name: "How are you taking part in this consultation?" }).length).toBe(0);
	expect(screen.getByRole("heading", { name: "Make or review comments as an individual" })).toBeInTheDocument();
	const backButton = screen.getByText(/Back/i, { selector: "button" });
	fireEvent.click(backButton);
	expect(screen.queryAllByRole("heading", { name: "Make or review comments as an individual" }).length).toBe(0);
	expect(screen.getByRole("heading", { name: "How are you taking part in this consultation?" })).toBeInTheDocument();
});

it("clears radio buttons after navigating back to previous screen", () => {
	render(
		<UserContext.Provider value={{}}>
			<MemoryRouter>
				<LoginPanel { ...loginPanelFakeProps } />
			</MemoryRouter>
		</UserContext.Provider>,
	);
	const radioButton = screen.getByLabelText("As an individual");
	expect(screen.getByRole("heading", { name: "How are you taking part in this consultation?" })).toBeInTheDocument();
	fireEvent.click(radioButton);
	expect(screen.queryAllByRole("heading", { name: "How are you taking part in this consultation?" }).length).toBe(0);
	expect(screen.getByRole("heading", { name: "Make or review comments as an individual" })).toBeInTheDocument();
	const backButton = screen.getByText(/Back/i, { selector: "button" });
	fireEvent.click(backButton);
	expect(screen.queryAllByRole("heading", { name: "Make or review comments as an individual" }).length).toBe(0);
	expect(screen.getByRole("heading", { name: "How are you taking part in this consultation?" })).toBeInTheDocument();
	const allRadioButtons = screen.queryAllByRole("radio");
	allRadioButtons.map(node => {
		expect(node.checked).toEqual(false);
	});
});

it("should match the snapshot after first loading up", () => {
	const {container} = render(
		<UserContext.Provider value={{}}>
			<MemoryRouter>
				<LoginPanel { ...loginPanelFakeProps } />
			</MemoryRouter>
		</UserContext.Provider>,
	);
	expect(container).toMatchSnapshot();
});
