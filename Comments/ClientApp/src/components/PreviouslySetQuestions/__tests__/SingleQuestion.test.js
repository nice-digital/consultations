/* global jest */

import React from "react";
import { shallow } from "enzyme";
import {SingleQuestion} from "../SingleQuestion";
import toJson from "enzyme-to-json";

const fakeProps1 = {
	questionText: "fake question text",
	newQuestion: jest.fn(),
	currentDocumentId: "consultation",
	currentConsultationId: 1,
	questionTypeId: 3232,
	questionType: {
		type: "Text",
	},
	allRoles: [
		"Administrator",
		"SCSTeam",
	],
};

const fakeProps2 = {
	questionText: "fake question text",
	newQuestion: jest.fn(),
	currentDocumentId: 2,
	currentConsultationId: 1,
	questionTypeId: 3232,
	questionType: {
		type: "Text",
	},
	allRoles: [
		"Administrator",
		"SCSTeam",
	],
};

const e = {};

describe("[ClientApp] ", () => {
	describe("SingleQuestion Component", () => {

		it("should fire newQuestion with the correct arguments when the Insert button is clicked at consultation level", () => {
			const wrapper = shallow(<SingleQuestion {...fakeProps1}/>);
			const button = wrapper.find("button");
			button.simulate("click", {});
			expect(fakeProps1.newQuestion).toHaveBeenCalledWith(e, 1, null, 3232, "fake question text");
		});

		it("should fire newQuestion with the correct arguments when the Insert button is clicked", () => {
			const wrapper = shallow(<SingleQuestion {...fakeProps2}/>);
			const button = wrapper.find("button");
			button.simulate("click", {});
			expect(fakeProps2.newQuestion).toHaveBeenCalledWith(e, 1, 2, 3232, "fake question text");
		});

		it("should match the snapshot", () => {
			const wrapper = shallow(<SingleQuestion {...fakeProps1} />);
			expect(
				toJson(wrapper, {
					noKey: true,
					mode: "deep",
				})
			).toMatchSnapshot();
		});

	});
});
