/* global jest */

import React from "react";
import { shallow } from "enzyme";
import { SubmitResponseDialog } from "../SubmitResponseDialog";
import questionsWithAnswer from "../../SubmitResponseFeedback/__tests__/questionsWithAnswer.json";
import questionsWithMultipleAnswers from "../../SubmitResponseFeedback/__tests__/questionsWithMultipleAnswers.json";

describe("[ClientApp] ", () => {
	describe("Submit response dialog", () => {

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

		it("should fire parent change handler if the input values change", () => {
			const localProps = fakeProps;
			localProps.respondingAsOrganisation = true;
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			wrapper.find("input#organisationName").simulate("change", {target: {value: "h"}});
			wrapper.find("input#organisationName").simulate("change", {target: {value: "i"}});
			wrapper.find("input#organisationName").simulate("change", {target: {value: "!"}});
			expect(localProps.fieldsChangeHandler.mock.calls[2][0].target.value).toBe("!");
			expect(localProps.fieldsChangeHandler.mock.calls.length).toEqual(3);
		});

		it("should not reveal text input unless radio button 'yes' is selected", () => {
			const localProps = fakeProps;
			localProps.respondingAsOrganisation = false;
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.find("input#organisationName").length).toEqual(0);
			localProps.respondingAsOrganisation = "yes";
			const newWrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(newWrapper.find("input#organisationName").length).toEqual(1);
		});

		it("should not reveal organisation questions if the user is a lead for an organisation", () => {
			const localProps = fakeProps;
			localProps.isLead = true;
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.find("input#respondingAsOrganisation--true").length).toEqual(0);
		});

		it("should not allow submission if the mandatory questions have not been answered 1", () => {
			const localProps = fakeProps;
			localProps.validToSubmit = true;
			localProps.hasTobaccoLinks = null; // not answered at all
			localProps.respondingAsOrganisation = null; // not answered at all
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.state().feedbackVisible).toEqual(false);
			expect(wrapper.state().showSubmitWarning).toEqual(false);
			wrapper.find("#submitButton").simulate("click");
			wrapper.update();
			expect(wrapper.state().showSubmitWarning).toEqual(true);
			wrapper.find("#submitButton").simulate("click");
			wrapper.update();
			expect(wrapper.state().feedbackVisible).toEqual(true);
		});

		it("should not allow submission if the manadatory questions have not been answered 2", ()=> {
			const localProps = fakeProps;
			localProps.validToSubmit = true;
			localProps.hasTobaccoLinks = false;
			localProps.respondingAsOrganisation = null; // not answered at all
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.state().feedbackVisible).toEqual(false);
			expect(wrapper.state().showSubmitWarning).toEqual(false);
			wrapper.find("#submitButton").simulate("click");
			wrapper.update();
			expect(wrapper.state().showSubmitWarning).toEqual(true);
			wrapper.find("#submitButton").simulate("click");
			wrapper.update();
			expect(wrapper.state().feedbackVisible).toEqual(true);
		});

		it("should not allow submission if the manadatory questions have not been answered 3", ()=> {
			const localProps = fakeProps;
			localProps.validToSubmit = true;
			localProps.hasTobaccoLinks = null; // not answered at all
			localProps.respondingAsOrganisation = false;
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.state().feedbackVisible).toEqual(false);
			expect(wrapper.state().showSubmitWarning).toEqual(false);
			wrapper.find("#submitButton").simulate("click");
			wrapper.update();
			expect(wrapper.state().showSubmitWarning).toEqual(true);
			wrapper.find("#submitButton").simulate("click");
			wrapper.update();
			expect(wrapper.state().feedbackVisible).toEqual(true);
		});

		it("should allow submission if the manadatory questions have been answered 1", ()=> {
			const localProps = fakeProps;
			localProps.validToSubmit = true;
			localProps.hasTobaccoLinks = false;
			localProps.respondingAsOrganisation = false;
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.state().feedbackVisible).toEqual(false);
			expect(wrapper.state().showSubmitWarning).toEqual(false);
			wrapper.find("#submitButton").simulate("click");
			wrapper.update();
			expect(wrapper.state().showSubmitWarning).toEqual(true);
			wrapper.find("#submitButton").simulate("click");
			wrapper.update();
			expect(wrapper.state().feedbackVisible).toEqual(false);
		});

		it("should allow submission if the manadatory questions have been answered 2", ()=> {
			const localProps = fakeProps;
			localProps.validToSubmit = true;
			localProps.hasTobaccoLinks = true;
			localProps.respondingAsOrganisation = false;
			localProps.tobaccoDisclosure = "test";
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.state().feedbackVisible).toEqual(false);
			expect(wrapper.state().showSubmitWarning).toEqual(false);
			wrapper.find("#submitButton").simulate("click");
			wrapper.update();
			expect(wrapper.state().showSubmitWarning).toEqual(true);
			wrapper.find("#submitButton").simulate("click");
			wrapper.update();
			expect(wrapper.state().feedbackVisible).toEqual(false);
		});

		it("should allow submission if the manadatory questions have been answered 3", ()=> {
			const localProps = fakeProps;
			localProps.validToSubmit = true;
			localProps.hasTobaccoLinks = false;
			localProps.respondingAsOrganisation = true;
			localProps.organisationName = "test";
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.state().feedbackVisible).toEqual(false);
			expect(wrapper.state().showSubmitWarning).toEqual(false);
			wrapper.find("#submitButton").simulate("click");
			wrapper.update();
			expect(wrapper.state().showSubmitWarning).toEqual(true);
			wrapper.find("#submitButton").simulate("click");
			wrapper.update();
			expect(wrapper.state().feedbackVisible).toEqual(false);
		});

		it("should not allow submission if the manadatory questions have not been answered 7", ()=> {
			const localProps = fakeProps;
			localProps.validToSubmit = true;
			localProps.hasTobaccoLinks = false;
			localProps.respondingAsOrganisation = true;
			localProps.organisationName = "";
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.state().feedbackVisible).toEqual(false);
			expect(wrapper.state().showSubmitWarning).toEqual(false);
			wrapper.find("#submitButton").simulate("click");
			wrapper.update();
			expect(wrapper.state().showSubmitWarning).toEqual(true);
			wrapper.find("#submitButton").simulate("click");
			wrapper.update();
			expect(wrapper.state().feedbackVisible).toEqual(true);
		});

		it("should fire parent submit function when the submit button is clicked", () => {
			const localProps = {
				isAuthorised: true,
				submittedDate: null,
				validToSubmit: true,
				organisationName: "",
				tobaccoDisclosure: "",
				hasTobaccoLinks: false,
				respondingAsOrganisation: false,
				submitConsultation: jest.fn(),
				unsavedIds: [],
				questions: questionsWithAnswer,
			};
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.state().feedbackVisible).toEqual(false);
			expect(wrapper.state().showSubmitWarning).toEqual(false);
			wrapper.find("#submitButton").simulate("click");
			wrapper.update();
			expect(wrapper.state().showSubmitWarning).toEqual(true);
			wrapper.find("#submitButton").simulate("click");
			wrapper.update();
			expect(wrapper.state().feedbackVisible).toEqual(false);
		});

		it("should not fire parent submit function when the submit button is clicked, if form is in invalid state, and should instead show the feedback panel", () => {
			const localProps = fakeProps;
			localProps.validToSubmit = false;
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.state().feedbackVisible).toEqual(false);
			expect(wrapper.state().showSubmitWarning).toEqual(false);
			wrapper.find("#submitButton").simulate("click");
			wrapper.update();
			expect(wrapper.state().showSubmitWarning).toEqual(true);
			wrapper.find("#submitButton").simulate("click");
			wrapper.update();
			expect(wrapper.state().feedbackVisible).toEqual(true);
		});

		it("should prevent submission if there are currently unsaved changes to any comments or answers",() => {
			const localProps = fakeProps;
			localProps.unsavedIds = ["1001q","2002c"];
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.state().feedbackVisible).toEqual(false);
			expect(wrapper.state().showSubmitWarning).toEqual(false);
			wrapper.find("#submitButton").simulate("click");
			wrapper.update();
			expect(wrapper.state().showSubmitWarning).toEqual(true);
			wrapper.find("#submitButton").simulate("click");
			wrapper.update();
			expect(wrapper.state().feedbackVisible).toEqual(true);
		});

		it("should prevent submission if there are too many answers to a question",() => {
			const localProps = fakeProps;
			localProps.questions = questionsWithMultipleAnswers;
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.state().feedbackVisible).toEqual(false);
			expect(wrapper.state().showSubmitWarning).toEqual(false);
			wrapper.find("#submitButton").simulate("click");
			wrapper.update();
			expect(wrapper.state().showSubmitWarning).toEqual(true);
			wrapper.find("#submitButton").simulate("click");
			wrapper.update();
			expect(wrapper.state().feedbackVisible).toEqual(true);
		});

		it("should not display a submit button if the current user isn't authorised", () => {
			const localProps = fakeProps;
			localProps.isAuthorised = false;
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.find("button").length).toEqual(0);
		});

	});
});
