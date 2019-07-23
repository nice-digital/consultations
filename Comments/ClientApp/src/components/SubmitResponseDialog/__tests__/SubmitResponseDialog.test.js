/* global jest */

import React from "react";
import { shallow } from "enzyme";
import { SubmitResponseDialog } from "../SubmitResponseDialog";

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
		};

		it("should fire parent change handler if the input values change", () => {
			const localProps = fakeProps;
			localProps.respondingAsOrganisation = "yes";
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			wrapper.find("input#organisationName").simulate("change", {target: {value: "h"}});
			wrapper.find("input#organisationName").simulate("change", {target: {value: "i"}});
			wrapper.find("input#organisationName").simulate("change", {target: {value: "!"}});
			expect(localProps.fieldsChangeHandler.mock.calls[2][0].target.value).toBe("!");
			expect(localProps.fieldsChangeHandler.mock.calls.length).toEqual(3);
		});

		it("should not reveal text input unless radio button 'yes' is selected", () => {
			const localProps = fakeProps;
			localProps.respondingAsOrganisation = "no";
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.find("input#organisationName").length).toEqual(0);
			localProps.respondingAsOrganisation = "yes";
			const newWrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(newWrapper.find("input#organisationName").length).toEqual(1);
		});

		it("should not allow submission if the mandatory questions have not been answered 1", () => {
			const localProps = fakeProps;
			localProps.validToSubmit = true;
			localProps.hasTobaccoLinks = ""; // not answered at all
			localProps.respondingAsOrganisation = ""; // not answered at all
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.state().feedbackVisible).toEqual(false);
			wrapper.find("button").simulate("click");
			wrapper.update();
			expect(wrapper.state().feedbackVisible).toEqual(true);
		});

		it("should not allow submission if the manadatory questions have not been answered 2", ()=> {
			const localProps = fakeProps;
			localProps.validToSubmit = true;
			localProps.hasTobaccoLinks = "no";
			localProps.respondingAsOrganisation = ""; // not answered at all
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.state().feedbackVisible).toEqual(false);
			wrapper.find("button").simulate("click");
			wrapper.update();
			expect(wrapper.state().feedbackVisible).toEqual(true);
		});

		it("should not allow submission if the manadatory questions have not been answered 3", ()=> {
			const localProps = fakeProps;
			localProps.validToSubmit = true;
			localProps.hasTobaccoLinks = ""; // not answered at all
			localProps.respondingAsOrganisation = "no";
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.state().feedbackVisible).toEqual(false);
			wrapper.find("button").simulate("click");
			wrapper.update();
			expect(wrapper.state().feedbackVisible).toEqual(true);
		});

		it("should allow submission if the manadatory questions have been answered 1", ()=> {
			const localProps = fakeProps;
			localProps.validToSubmit = true;
			localProps.hasTobaccoLinks = "no";
			localProps.respondingAsOrganisation = "no";
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.state().feedbackVisible).toEqual(false);
			wrapper.find("button").simulate("click");
			wrapper.update();
			expect(wrapper.state().feedbackVisible).toEqual(false);
		});

		it("should allow submission if the manadatory questions have been answered 2", ()=> {
			const localProps = fakeProps;
			localProps.validToSubmit = true;
			localProps.hasTobaccoLinks = "yes";
			localProps.respondingAsOrganisation = "no";
			localProps.tobaccoDisclosure = "test";
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.state().feedbackVisible).toEqual(false);
			wrapper.find("button").simulate("click");
			wrapper.update();
			expect(wrapper.state().feedbackVisible).toEqual(false);
		});

		it("should allow submission if the manadatory questions have been answered 3", ()=> {
			const localProps = fakeProps;
			localProps.validToSubmit = true;
			localProps.hasTobaccoLinks = "no";
			localProps.respondingAsOrganisation = "yes";
			localProps.organisationName = "test";
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.state().feedbackVisible).toEqual(false);
			wrapper.find("button").simulate("click");
			wrapper.update();
			expect(wrapper.state().feedbackVisible).toEqual(false);
		});

		it("should not allow submission if the manadatory questions have not been answered 7", ()=> {
			const localProps = fakeProps;
			localProps.validToSubmit = true;
			localProps.hasTobaccoLinks = "no";
			localProps.respondingAsOrganisation = "yes";
			localProps.organisationName = "";
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.state().feedbackVisible).toEqual(false);
			wrapper.find("button").simulate("click");
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
				hasTobaccoLinks: "no",
				respondingAsOrganisation: "no",
				submitConsultation: jest.fn(),
				unsavedIds: [],
			};
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.state().feedbackVisible).toEqual(false);
			wrapper.find("button").simulate("click");
			expect(wrapper.state().feedbackVisible).toEqual(false);
		});

		it("should not fire parent submit function when the submit button is clicked, if form is in invalid state, and should instead show the feedback panel", () => {
			const localProps = fakeProps;
			localProps.validToSubmit = false;
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			const button = wrapper.find("button");
			button.simulate("click");
			wrapper.update();
			expect(wrapper.state().feedbackVisible).toEqual(true);
		});

		it("should prevent submission if there are currently unsaved changes to any comments or answers",() => {
			const localProps = fakeProps;
			localProps.unsavedIds = ["1001q","2002c"];
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			wrapper.find("button").simulate("click");
			wrapper.update();
			expect(wrapper.state().feedbackVisible).toEqual(true);
		});

		it("should not display a submit button if the current user isn't authorised", () => {
			const localProps = fakeProps;
			localProps.isAuthorised = false;
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.find("button").length).toEqual(0);
		});

		it("should show nothing if user has submitted", () => {
			const localProps = fakeProps;
			localProps.submittedDate = "2019-07-15T14:24:18.4735291";
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.html()).toEqual(null);
		});

	});
});
