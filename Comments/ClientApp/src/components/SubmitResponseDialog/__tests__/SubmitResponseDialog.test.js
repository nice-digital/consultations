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
			inputChangeHandler: jest.fn(),
			organisationName: "",
			tobaccoDisclosure: "",
		};

		it("should fire parent change handler if the input values change", () => {
			const wrapper = shallow(<SubmitResponseDialog {...fakeProps} />);
			wrapper.find("input#organisationName").simulate("change", {target: {value: "h"}});
			wrapper.find("input#organisationName").simulate("change", {target: {value: "i"}});
			wrapper.find("input#organisationName").simulate("change", {target: {value: "!"}});
			expect(fakeProps.inputChangeHandler.mock.calls[2][0].target.value).toBe("!");
			expect(fakeProps.inputChangeHandler.mock.calls.length).toEqual(3);
		});

		it("should fire parent submit function when the submit button is clicked", () => {
			const wrapper = shallow(<SubmitResponseDialog {...fakeProps} />);
			wrapper.find("button").simulate("click");
			expect(fakeProps.submitConsultation.mock.calls.length).toEqual(1);
		});

		it("should not fire parent submit function when the submit button is clicked, if form is in invalid state", () => {
			const localProps = fakeProps;
			localProps.validToSubmit = false;
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			const button = wrapper.find("button");
			expect(button.prop("disabled")).toBe(true);
		});

		it("should not display a submit button if the current user isn't authorised", () => {
			const localProps = fakeProps;
			localProps.isAuthorised = false;
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.find("button").length).toEqual(0);
		});

		it("should show thank you message if user has submitted", () => {
			const localProps = fakeProps;
			localProps.userHasSubmitted = true;
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.find("p").text()).toEqual("Thank you, your response has been submitted.");
		});

	});
});
