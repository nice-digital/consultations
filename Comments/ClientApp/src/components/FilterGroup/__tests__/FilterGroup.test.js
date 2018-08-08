/* eslint-env jest */
import React from "react";
import { shallow } from "enzyme";

import { FilterGroup } from "../FilterGroup";

jest.mock("../../HistoryContext/HistoryContext", () => {
	return {
		withHistory: (Component) => {
			return Component;
		}
	};
});

describe("[Consultations]", () => {
	describe("FilterGroup", () => {

		const zeroResultsOptions = [
			{
				id: "0",
				label: "Zero - no results",
				isSelected: false,
				filteredResultCount: 0,
				unFilteredResultCount: 0
			}
		];

		const selectedOptions = [
			{
				id: "1",
				label: "One - selected",
				isSelected: true,
				filteredResultCount: 0,
				unfilteredResultCount: 1
			},
			{
				id: "2",
				label: "Two - selected",
				isSelected: true,
				filteredResultCount: 0,
				unfilteredResultCount: 1
			}
		];

		const unSelectedOptions = [
			{
				id: "3",
				label: "Three - unselected",
				isSelected: false,
				filteredResultCount: 0,
				unfilteredResultCount: 1
			},
			{
				id: "4",
				label: "Four - unselected",
				isSelected: false,
				filteredResultCount: 0,
				unfilteredResultCount: 1
			}
		];

		const optionsWithResults = [].concat(selectedOptions, unSelectedOptions);

		const filterGroupModel = {
			id: "gId",
			title: "Test",
			options: [].concat(zeroResultsOptions, optionsWithResults)
		};

		const allOptionsUnselectedFilterGroupModel = {
			id: "gId",
			title: "Test",
			options: unSelectedOptions
		};

		describe("getSelectedCount", () => {
			it("returns correct number of selected options", () => {
				const filterGroup = shallow(<FilterGroup filterGroup={filterGroupModel} />);
				expect(filterGroup.instance().getSelectedCount()).toEqual(2);
			});
		});

		describe("getOptionsToRender", () => {
			it("returns options that have at least 1 result", () => {
				const filterGroup = shallow(<FilterGroup filterGroup={filterGroupModel} />);
				const actualOptions = filterGroup.instance().getOptionsToRender();
				expect(actualOptions).toEqual(optionsWithResults);
			});
		});

		describe("handleTitleClick", () => {
			it("clicking heading button expands collapsed group", () => {
				const filterGroup = shallow(<FilterGroup filterGroup={allOptionsUnselectedFilterGroupModel} />);

				expect(filterGroup.find("button.filter-group__heading").prop("aria-expanded")).toEqual(false);
				expect(filterGroup.find(".filter-group__options").prop("aria-hidden")).toEqual(true);

				filterGroup.find("button.filter-group__heading").simulate("click", {});

				expect(filterGroup.find("button.filter-group__heading").prop("aria-expanded")).toEqual(true);
				expect(filterGroup.find(".filter-group__options").prop("aria-hidden")).toEqual(false);
			});

			it("clicking heading button collapses expanded group", () => {
				const filterGroup = shallow(<FilterGroup filterGroup={filterGroupModel} />);

				expect(filterGroup.find("button.filter-group__heading").prop("aria-expanded")).toEqual(true);
				expect(filterGroup.find(".filter-group__options").prop("aria-hidden")).toEqual(false);

				filterGroup.find("button.filter-group__heading").simulate("click", {});

				expect(filterGroup.find("button.filter-group__heading").prop("aria-expanded")).toEqual(false);
				expect(filterGroup.find(".filter-group__options").prop("aria-hidden")).toEqual(true);
			});
		});

		describe("render", () => {
			it("renders rootElement with filter-group class", () => {
				const filterGroup = shallow(<FilterGroup filterGroup={filterGroupModel} />);
				expect(filterGroup.is(".filter-group")).toEqual(true);
			});

			it("group title renders as a heading 3 on the server", () => {
				const filterGroup = shallow(<FilterGroup filterGroup={filterGroupModel} />, { disableLifecycleMethods: true });
				expect(filterGroup.find(".filter-group__heading").is("h3")).toEqual(true);
			});

			it("group title heading contains selected count", () => {
				const filterGroup = shallow(<FilterGroup filterGroup={filterGroupModel} />, { disableLifecycleMethods: true });
				expect(filterGroup.find(".filter-group__heading .filter-group__count").text()).toEqual("2 selected");
			});

			it("group title renders as a button on the client", () => {
				const filterGroup = shallow(<FilterGroup filterGroup={filterGroupModel} />);
				expect(filterGroup.find(".filter-group__heading").is("button")).toEqual(true);
			});

			it("group title buttons contains selected count", () => {
				const filterGroup = shallow(<FilterGroup filterGroup={filterGroupModel} />);
				expect(filterGroup.find(".filter-group__heading .filter-group__count").text()).toEqual("2 selected");
			});

			it("group is aria labelled by the heading id", () => {
				const filterGroup = shallow(<FilterGroup filterGroup={filterGroupModel} />);
				const expectedId = "group-title-gId";
				const title = filterGroup.find(`#${ expectedId }`);
				expect(title.length).toEqual(1);
				const optionsGroup = filterGroup.find("[role=\"group\"]");
				expect(optionsGroup.prop("aria-labelledby")).toEqual(expectedId);
			});

			it("heading button aria controls the group of options", () => {
				const filterGroup = shallow(<FilterGroup filterGroup={filterGroupModel} />);
				const expectedGroupId = "group-gId";
				const groupElement = filterGroup.find(`#${ expectedGroupId }`);
				expect(groupElement.length).toEqual(1);
				const buttonHeading = filterGroup.find("button.filter-group__heading");
				expect(buttonHeading.prop("aria-controls")).toEqual(expectedGroupId);
			});

			it("group is initially expanded when it has selected results", () => {
				const filterGroup = shallow(<FilterGroup filterGroup={filterGroupModel} />);
				expect(filterGroup.find(".filter-group__heading").prop("aria-expanded")).toEqual(true);
			});

			it("group is initially collapsed when it has no selected results", () => {
				const filterGroup = shallow(<FilterGroup filterGroup={allOptionsUnselectedFilterGroupModel} />);
				expect(filterGroup.find(".filter-group__heading").prop("aria-expanded")).toEqual(false);
			});

			it("renders list of filter options", () => {
				const filterGroup = shallow(<FilterGroup filterGroup={filterGroupModel} />);
				expect(filterGroup.find(".filter-group__options FilterOption").length).toEqual(4);
			});

			it("passes correct props to child filter options", () => {
				const filterGroup = shallow(<FilterGroup filterGroup={filterGroupModel} path="/test" />);

				const expectedProps = {
					groupId: "gId",
					groupName: "Test",
					path: "/test",
					option: {
						id: "1",
						label: "One - selected",
						isSelected: true,
						filteredResultCount: 0,
						unfilteredResultCount: 1
					}
				};

				expect(filterGroup.find("FilterOption").at(0).props()).toEqual(expectedProps);
			});
		});
	});
});
