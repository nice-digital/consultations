/* global jest */

import React from "react";
import { shallow, mount } from "enzyme";
import { SubmitResponseDialog } from "../SubmitResponseDialog";

describe("[ClientApp] ", () => {
	describe("Submit response dialog", () => {

		const fakeProps = {
			isAuthorised: true,
			userHasSubmitted: false,
			validToSubmit: true,
			submitConsultation: () => {
				return jest.mock();
			},
			inputChangeHandler: () => {
				return jest.mock();
			},
			organisationName: "NICE",
			tobaccoDisclosure: "Used to work for a tobacco company",
		};

		it("should show thank you message if user has submitted", () => {
			const localProps = fakeProps;
			localProps.userHasSubmitted = true;
			const wrapper = shallow(<SubmitResponseDialog {...localProps} />);
			expect(wrapper.find("p").text()).toEqual("Thank you, your response has been submitted.");
		});

		it("should fire parent change handler if the input values change", () => {
			const wrapper = shallow(<SubmitResponseDialog {...fakeProps} />);
		});

		it("should fire parent submit function when the submit button is clicked", () => {
			const wrapper = shallow(<SubmitResponseDialog {...fakeProps} />);
		});

		it("should not render anything if the user isn't authorised", () => {
			const wrapper = shallow(<SubmitResponseDialog {...fakeProps} />);
		});

	});
});
