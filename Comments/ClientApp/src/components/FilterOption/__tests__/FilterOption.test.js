/* eslint-env jest */
import React from "react";
import { shallow } from "enzyme";

import { FilterOption } from "../FilterOption";

describe("[TopicList]", () => {
	describe("FilterOption", () => {

		describe("getHref", () => {
			it("appends query string to href when checkbox is selected", () => {
				const optionModel = {
					id: "oId",
					isSelected: true
				};
				const filterOption = shallow(<FilterOption groupId="gId" option={optionModel} path="/test" />);
				expect(filterOption.instance().getHref()).toEqual("/test?gId=oId");
			});

			it("removes query string from href when checkbox is unselected", () => {
				const optionModel = {
					id: "oId",
					isSelected: false
				};
				const filterOption = shallow(<FilterOption groupId="gId" option={optionModel} path="/test?gId=oId" />);
				expect(filterOption.instance().getHref()).toEqual("/test");
			});
		});

		it("renders a checkbox wrapped in a label", () => {
			const optionModel = {
				id: "oId",
				isSelected: false,
				label: "Test"
			};
			const filterOption = shallow(<FilterOption groupId="gId" option={optionModel} />);
			expect(filterOption.is("label")).toEqual(true);
			expect(filterOption.find("label input").length).toEqual(1);
		});

		it("label for attribute matches checkbox id", () => {
			const optionModel = {
				id: "oId",
				isSelected: false,
				label: "Test"
			};
			const filterOption = shallow(<FilterOption groupId="gId" option={optionModel} />);
			const label = filterOption.find("label");
			const input = filterOption.find("input");
			expect(label.prop("htmlFor")).toEqual(input.prop("id"));
		});

		it("passes correct props to checkbox", () => {
			const optionModel = {
				id: "oId",
				isSelected: true,
				label: "Test option"
			};
			const filterOption = shallow(<FilterOption groupId="gId" groupName="Test group" option={optionModel} />);
			const input = filterOption.find("input");

			const expectedProps = {
				type: "checkbox",
				id: "filter_gId_oId",
				name: "gId",
				value: "oId",
				checked: true,
				"aria-controls": "results-info-count",
				title: "Test group - Test option",
				className: "gtm-topic-list-filter-deselect",
				onChange: filterOption.instance().handleCheckboxChange
			};

			expect(input.props()).toEqual(expectedProps);
		});

		it("label element text is option prop label", () => {
			const optionModel = {
				label: "Test"
			};
			const filterOption = shallow(<FilterOption option={optionModel} />);
			const label = filterOption.find("label");
			expect(label.text()).toEqual("Test");
		});

		it("pushes history state when checkbox is changed", () => {
			const optionModel = {
				id: "oId",
				isSelected: false
			};
			const history = { push: jest.fn() };
			const event = { preventDefault: jest.fn() };

			const filterOption = shallow(<FilterOption groupId="gId" option={optionModel} history={history} path="/test?a=b" />);

			filterOption.find("input").simulate("change", event);

			expect(history.push).toHaveBeenCalledWith(filterOption.instance().getHref());
		});

	});
});
