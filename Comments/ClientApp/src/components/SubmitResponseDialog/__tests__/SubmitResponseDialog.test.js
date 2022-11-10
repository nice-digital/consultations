import React from "react";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { SubmitResponseDialog } from "../SubmitResponseDialog";
import questionsWithAnswer from "../../SubmitResponseFeedback/__tests__/questionsWithAnswer.json";
import questionsWithMultipleAnswers from "../../SubmitResponseFeedback/__tests__/questionsWithMultipleAnswers.json";

const fakeProps = {
	isAuthorised: true,
	submittedDate: null,
	validToSubmit: true,
	submitConsultation: jest.fn(),
	fieldsChangeHandler: jest.fn(),
	organisationName: "",
	tobaccoDisclosure: "",
	hasTobaccoLinks: "no",
	respondingAsOrganisation: "no",
	unsavedIds: [],
	questions: questionsWithAnswer,
};

test("should fire parent change handler if the input values change", () => {
	const localProps = Object.assign({}, fakeProps);
	localProps.respondingAsOrganisation = true;
	render(<SubmitResponseDialog {...localProps} />);
	const orgNameInput = screen.getByLabelText("Enter the name of your organisation");
	fireEvent.change(orgNameInput, {target: {value: "Hi!"}});
	expect(localProps.fieldsChangeHandler).toHaveBeenCalled();
});

test("should not reveal text input unless radio button 'yes' is selected", async () => {
	const localProps = Object.assign({}, fakeProps);
	localProps.respondingAsOrganisation = false;
	const {rerender} = render(<SubmitResponseDialog {...localProps} />);
	const orgNameInput = screen.queryAllByLabelText("Enter the name of your organisation");
	expect(orgNameInput.length).toEqual(0);
	localProps.respondingAsOrganisation = "yes";
	rerender(<SubmitResponseDialog {...localProps} />);
	expect(screen.getByLabelText("Enter the name of your organisation")).toBeInTheDocument();
});

test("should not reveal organisation questions if the user is a lead for an organisation", () => {
	const localProps = Object.assign({}, fakeProps);
	localProps.isLead = true;
	render(<SubmitResponseDialog {...localProps} />);
	expect(screen.queryAllByLabelText("Yes", { selector: "[name='respondingAsOrganisation']" }).length).toEqual(0);
});

test("should not allow submission if the mandatory questions have not been answered 1", async () => {
	const localProps = Object.assign({}, fakeProps);
	localProps.validToSubmit = true;
	localProps.hasTobaccoLinks = null; // not answered at all
	localProps.respondingAsOrganisation = null; // not answered at all
	render(<SubmitResponseDialog {...localProps} />);
	fireEvent.click(screen.getByRole("button", { name: "Submit my response" }));
	await waitFor(() => screen.getByRole("button", { name: "Yes submit my response" }));
	fireEvent.click(screen.getByRole("button", { name: "Yes submit my response" }));
	await waitFor(() => screen.getByText("You can't submit your response yet"));
	expect(screen.getByText("You can't submit your response yet")).toBeInTheDocument();
});

test("should not allow submission if the mandatory questions have not been answered 2", async ()=> {
	const localProps = Object.assign({}, fakeProps);
	localProps.validToSubmit = true;
	localProps.hasTobaccoLinks = false;
	localProps.respondingAsOrganisation = null; // not answered at all
	render(<SubmitResponseDialog {...localProps} />);
	fireEvent.click(screen.getByRole("button", { name: "Submit my response" }));
	await waitFor(() => screen.getByRole("button", { name: "Yes submit my response" }));
	fireEvent.click(screen.getByRole("button", { name: "Yes submit my response" }));
	await waitFor(() => screen.getByText("You can't submit your response yet"));
	expect(screen.getByText("You can't submit your response yet")).toBeInTheDocument();
});

test("should not allow submission if the mandatory questions have not been answered 3", async ()=> {
	const localProps = Object.assign({}, fakeProps);
	localProps.validToSubmit = true;
	localProps.hasTobaccoLinks = null; // not answered at all
	localProps.respondingAsOrganisation = false;
	render(<SubmitResponseDialog {...localProps} />);
	fireEvent.click(screen.getByRole("button", { name: "Submit my response" }));
	await waitFor(() => screen.getByRole("button", { name: "Yes submit my response" }));
	fireEvent.click(screen.getByRole("button", { name: "Yes submit my response" }));
	await waitFor(() => screen.getByText("You can't submit your response yet"));
	expect(screen.getByText("You can't submit your response yet")).toBeInTheDocument();
});

test("should allow submission if the mandatory questions have been answered 1", async ()=> {
	const localProps = Object.assign({}, fakeProps);
	localProps.validToSubmit = true;
	localProps.hasTobaccoLinks = false;
	localProps.respondingAsOrganisation = false;
	render(<SubmitResponseDialog {...localProps} />);
	fireEvent.click(screen.getByRole("button", { name: "Submit my response" }));
	await waitFor(() => screen.getByRole("button", { name: "Yes submit my response" }));
	fireEvent.click(screen.getByRole("button", { name: "Yes submit my response" }));
	const submitResponseFeedback = screen.queryAllByText("You can't submit your response yet");
	expect(submitResponseFeedback.length).toEqual(0);
});

test("should allow submission if the mandatory questions have been answered 2", async ()=> {
	const localProps = Object.assign({}, fakeProps);
	localProps.validToSubmit = true;
	localProps.hasTobaccoLinks = true;
	localProps.respondingAsOrganisation = false;
	localProps.tobaccoDisclosure = "test";
	render(<SubmitResponseDialog {...localProps} />);
	fireEvent.click(screen.getByRole("button", { name: "Submit my response" }));
	await waitFor(() => screen.getByRole("button", { name: "Yes submit my response" }));
	fireEvent.click(screen.getByRole("button", { name: "Yes submit my response" }));
	const submitResponseFeedback = screen.queryAllByText("You can't submit your response yet");
	expect(submitResponseFeedback.length).toEqual(0);
});

test("should allow submission if the mandatory questions have been answered 3", async ()=> {
	const localProps = Object.assign({}, fakeProps);
	localProps.validToSubmit = true;
	localProps.hasTobaccoLinks = false;
	localProps.respondingAsOrganisation = true;
	localProps.organisationName = "test";
	render(<SubmitResponseDialog {...localProps} />);
	fireEvent.click(screen.getByRole("button", { name: "Submit my response" }));
	await waitFor(() => screen.getByRole("button", { name: "Yes submit my response" }));
	fireEvent.click(screen.getByRole("button", { name: "Yes submit my response" }));
	const submitResponseFeedback = screen.queryAllByText("You can't submit your response yet");
	expect(submitResponseFeedback.length).toEqual(0);
});

test("should fire parent submit function when the submit button is clicked", async () => {
	const localProps = {
		isAuthorised: true,
		submittedDate: null,
		validToSubmit: true,
		organisationName: "",
		tobaccoDisclosure: "",
		hasTobaccoLinks: false,
		respondingAsOrganisation: false,
		submitConsultation: jest.fn(),
		fieldsChangeHandler: jest.fn(),
		unsavedIds: [],
		questions: questionsWithAnswer,
	};
	render(<SubmitResponseDialog {...localProps} />);
	fireEvent.click(screen.getByRole("button", { name: "Submit my response" }));
	await waitFor(() => screen.getByRole("button", { name: "Yes submit my response" }));
	fireEvent.click(screen.getByRole("button", { name: "Yes submit my response" }));
	expect(localProps.submitConsultation).toHaveBeenCalled();
});

test("should not fire parent submit function when the submit button is clicked, if form is in invalid state, and should instead show the feedback panel", async () => {
	const localProps = {
		isAuthorised: true,
		submittedDate: null,
		validToSubmit: false,
		submitConsultation: jest.fn(),
		fieldsChangeHandler: jest.fn(),
		organisationName: "",
		tobaccoDisclosure: "",
		hasTobaccoLinks: "no",
		respondingAsOrganisation: "no",
		unsavedIds: [],
		questions: questionsWithAnswer,
	};
	render(<SubmitResponseDialog {...localProps} />);
	fireEvent.click(screen.getByRole("button", { name: "Submit my response" }));
	await waitFor(() => screen.getByRole("button", { name: "Yes submit my response" }));
	fireEvent.click(screen.getByRole("button", { name: "Yes submit my response" }));
	await waitFor(() => screen.getByText("You can't submit your response yet"));
	expect(screen.getByText("You can't submit your response yet")).toBeInTheDocument();
	expect(localProps.submitConsultation).toHaveBeenCalledTimes(0);
});

test("should prevent submission if there are currently unsaved changes to any comments or answers", async () => {
	const localProps = fakeProps;
	localProps.unsavedIds = ["1001q","2002c"];
	render(<SubmitResponseDialog {...localProps} />);
	fireEvent.click(screen.getByRole("button", { name: "Submit my response" }));
	await waitFor(() => screen.getByRole("button", { name: "Yes submit my response" }));
	fireEvent.click(screen.getByRole("button", { name: "Yes submit my response" }));
	await waitFor(() => screen.getByText("You can't submit your response yet"));
	expect(screen.getByText("You can't submit your response yet")).toBeInTheDocument();
});

test("should prevent submission if there are too many answers to a question", async () => {
	const localProps = fakeProps;
	localProps.questions = questionsWithMultipleAnswers;
	render(<SubmitResponseDialog {...localProps} />);
	fireEvent.click(screen.getByRole("button", { name: "Submit my response" }));
	await waitFor(() => screen.getByRole("button", { name: "Yes submit my response" }));
	fireEvent.click(screen.getByRole("button", { name: "Yes submit my response" }));
	await waitFor(() => screen.getByText("You can't submit your response yet"));
	expect(screen.getByText("You can't submit your response yet")).toBeInTheDocument();
});

test("should not display a submit button if the current user isn't authorised", () => {
	const localProps = fakeProps;
	localProps.isAuthorised = false;
	render(<SubmitResponseDialog {...localProps} />);
	expect(screen.queryAllByRole("button").length).toEqual(0);
});
