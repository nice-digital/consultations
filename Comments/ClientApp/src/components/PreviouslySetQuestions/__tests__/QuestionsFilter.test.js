/* global jest */

import React from "react";
import { shallow } from "enzyme";
import {QuestionsFilter} from "../QuestionsFilter";
import toJson from "enzyme-to-json";

const fakeProps = {
	handleFilter: jest.fn(),
	filter: {
		direcorate: true,
	},
};

const e = {};

describe("[ClientApp] ", () => {
	describe("QuestionsFilter Component", () => {

		it("should fire the handleFilter function when a input is changed", () => {
			const wrapper = shallow(<QuestionsFilter {...fakeProps}/>);
			const radio1 = wrapper.find("input#filterByRole--all");
			radio1.simulate("change", {});
			expect(fakeProps.handleFilter).toHaveBeenCalledWith(e, false);
		});

		it("should match the snapshot when the filter is set to directorate", () => {
			const wrapper = shallow(<QuestionsFilter {...fakeProps} />);
			expect(
				toJson(wrapper, {
					noKey: true,
					mode: "deep",
				}),
			).toMatchSnapshot();
		});

		it("should match the snapshot when the filter is set to all", () => {
			const localProps = Object.assign({}, fakeProps);
			localProps.filter.directorate = false;
			const wrapper = shallow(<QuestionsFilter {...localProps} />);
			expect(
				toJson(wrapper, {
					noKey: true,
					mode: "deep",
				}),
			).toMatchSnapshot();
		});

	});
});
