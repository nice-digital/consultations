/* global jest */

import React from "react";
import { shallow } from "enzyme";
import { SubmitResponseDialog } from "../SubmitResponseDialog";

describe("[ClientApp] ", () => {
	describe("Submit response dialog", () => {

		const fakeProps = {
			isAuthorised: true,
			userHasSubmitted: false,
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

		it("should not allow submission if the mandatory questions have not been answered", () => {
			const localProps = fakeProps;
			localProps.validToSubmit = true;

			localProps.hasTobaccoLinks = ""; // not answered at all
			localProps.respondingAsOrganisation = ""; // not answered at all
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.find("button").prop("aria-disabled")).toBe(true);

			localProps.hasTobaccoLinks = "no";
			localProps.respondingAsOrganisation = ""; // not answered at all
			const newWrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(newWrapper.find("button").prop("aria-disabled")).toBe(true);

			localProps.hasTobaccoLinks = ""; // not answered at all
			localProps.respondingAsOrganisation = "no";
			const newerWrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(newerWrapper.find("button").prop("aria-disabled")).toBe(true);

			localProps.hasTobaccoLinks = "no";
			localProps.respondingAsOrganisation = "no";
			const evenNewerWrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(evenNewerWrapper.find("button").prop("aria-disabled")).toBe(false);

			localProps.hasTobaccoLinks = "yes";
			localProps.respondingAsOrganisation = "no";
			localProps.tobaccoDisclosure = "test";
			const newestWrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(newestWrapper.find("button").prop("aria-disabled")).toBe(false);

			localProps.hasTobaccoLinks = "no";
			localProps.respondingAsOrganisation = "yes";
			localProps.organisationName = "test";
			const evenNewestWrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(evenNewestWrapper.find("button").prop("aria-disabled")).toBe(false);

			localProps.hasTobaccoLinks = "no";
			localProps.respondingAsOrganisation = "yes";
			localProps.organisationName = "";
			const lastWrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(lastWrapper.find("button").prop("aria-disabled")).toBe(true);
		});

		it("should fire parent submit function when the submit button is clicked", () => {
			const localProps = {
				isAuthorised: true,
				userHasSubmitted: false,
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
			const button = wrapper.find("button");
			expect(button.prop("aria-disabled")).toBe(true);
		});

		it("should not display a submit button if the current user isn't authorised", () => {
			const localProps = fakeProps;
			localProps.isAuthorised = false;
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.find("button").length).toEqual(0);
		});

		it("should show nothing if user has submitted", () => {
			const localProps = fakeProps;
			localProps.userHasSubmitted = true;
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.html()).toEqual(null);
		});

	});
});
