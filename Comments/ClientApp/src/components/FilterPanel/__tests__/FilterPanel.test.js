/* eslint-env jest */
import React from "react";
import { shallow } from "enzyme";

import { FilterPanel } from "../FilterPanel";
import FilterGroup from "./../FilterGroup/FilterGroup";

describe("[TopicList]", () => {
	describe("FilterPanel", () => {

		describe("getFilterGroupsToDisplay", () => {
			it("returns groups that have at least one selected option", () => {
				const filters = [
					{
						id: "a",
						options: [
							{ isSelected: true, filteredResultCount: 0 },
							{ isSelected: false, filteredResultCount: 1 }
						]
					},
					{
						id: "b", options: [ { isSelected: false, filteredResultCount: 0 } ]
					}
				];
				const filterPanel = shallow(<FilterPanel filters={filters} />);
				const groupsToDisplay = filterPanel.instance().getFilterGroupsToDisplay();
				expect(groupsToDisplay.length).toEqual(1);
				expect(groupsToDisplay[0].id).toEqual("a");
			});

			it("returns groups that have at least one option with filtered results", () => {
				const filters = [
					{
						id: "a",
						options: [ { filteredResultCount: 0 } ]
					},
					{
						id: "b",
						options: [
							{ filteredResultCount: 1 },
							{ filteredResultCount: 0 }
						]
					}
				];
				const filterPanel = shallow(<FilterPanel filters={filters} />);
				const groupsToDisplay = filterPanel.instance().getFilterGroupsToDisplay();
				expect(groupsToDisplay.length).toEqual(1);
				expect(groupsToDisplay[0].id).toEqual("b");
			});

			it("doesn't return groups that have no selected option or options with results", () => {
				const filters = [
					{
						id: "a",
						options: [ { filteredResultCount: 0 } ]
					},
					{
						id: "b",
						options: [
							{ filteredResultCount: 0 }
						]
					}
				];
				const filterPanel = shallow(<FilterPanel filters={filters} />);
				const groupsToDisplay = filterPanel.instance().getFilterGroupsToDisplay();
				expect(groupsToDisplay.length).toEqual(0);
			});
		});

		describe("handleClick", () => {

			it("clicking heading button collapses expanded panel", () => {
				const filterPanel = shallow(<FilterPanel filters={[]} />);

				expect(filterPanel.find("button.filter-panel__heading").prop("aria-expanded")).toEqual(true);
				expect(filterPanel.find(".filter-panel__body").prop("aria-hidden")).toEqual(false);

				filterPanel.find("button.filter-panel__heading").simulate("click", {});

				expect(filterPanel.find("button.filter-panel__heading").prop("aria-expanded")).toEqual(false);
				expect(filterPanel.find(".filter-panel__body").prop("aria-hidden")).toEqual(true);
			});

			it("clicking heading button expands collapsed panel", () => {
				const filterPanel = shallow(<FilterPanel filters={[]} />);

				filterPanel.setState({ isExpanded: false });

				expect(filterPanel.find("button.filter-panel__heading").prop("aria-expanded")).toEqual(false);
				expect(filterPanel.find(".filter-panel__body").prop("aria-hidden")).toEqual(true);

				filterPanel.find("button.filter-panel__heading").simulate("click", {});

				expect(filterPanel.find("button.filter-panel__heading").prop("aria-expanded")).toEqual(true);
				expect(filterPanel.find(".filter-panel__body").prop("aria-hidden")).toEqual(false);
			});

		});

		describe("render", () => {
			it("renders rootElement with filter-group class", () => {
				const filterPanel = shallow(<FilterPanel filters={[]} />);
				expect(filterPanel.is(".filter-panel")).toEqual(true);
			});

			it("panel title renders as a heading 2 when not mounted", () => {
				const filterPanel = shallow(<FilterPanel filters={[]} />, { disableLifecycleMethods: true });
				expect(filterPanel.find(".filter-panel__heading").is("h2")).toEqual(true);
			});

			it("panel title renders as a button when mounted", () => {
				const filterPanel = shallow(<FilterPanel filters={[]} />);
				expect(filterPanel.find(".filter-panel__heading").is("button")).toEqual(true);
			});

			it("panel title button aria controls the panel body", () => {
				const filterPanel = shallow(<FilterPanel filters={[]} />);
				const buttonHeading = filterPanel.find("button.filter-panel__heading");
				expect(buttonHeading.prop("aria-controls")).toEqual("filter-panel-body");
			});

			it("panel title button is aria expanded by default when mounted", () => {
				const filterPanel = shallow(<FilterPanel filters={[]} />);
				const buttonHeading = filterPanel.find("button.filter-panel__heading");
				expect(buttonHeading.prop("aria-expanded")).toEqual(true);
			});

			it("panel body has id", () => {
				const filterPanel = shallow(<FilterPanel filters={[]} />);
				const panelBody = filterPanel.find(".filter-panel__body");
				expect(panelBody.prop("id")).toEqual("filter-panel-body");
			});

			it("panel body is aria hidden by default when mounted", () => {
				const filterPanel = shallow(<FilterPanel filters={[]} />);
				const panelBody = filterPanel.find(".filter-panel__body");
				expect(panelBody.prop("aria-hidden")).toEqual(false);
			});

			it("renders list of filter groups", () => {
				const filters = [ { id: "a", options: [ { isSelected: true } ] }, { id: "b", options: [ { filteredResultCount: 1 } ] } ];
				const filterPanel = shallow(<FilterPanel filters={filters} />);
				const filterGroups = filterPanel.find(FilterGroup);
				expect(filterGroups.length).toEqual(2);
			});

			it("passes correct props to filter group components", () => {
				const filters = [ { id: "a", options: [ { isSelected: true } ] } ];
				const filterPanel = shallow(<FilterPanel filters={filters} path="/test" />);
				const filterGroup = filterPanel.find(FilterGroup).at(0);
				const expectedProps = {
					path: "/test",
					filterGroup: filters[0]
				};
				expect(filterGroup.props()).toEqual(expectedProps);
			});

			it("renders submit filters button when not mounted", () => {
				const filterPanel = shallow(<FilterPanel filters={[]} />, { disableLifecycleMethods: true });
				const button = filterPanel.find(".filter-panel__submit");
				expect(button.length).toEqual(1);
				expect(button.is("button")).toEqual(true);
				expect(button.prop("type")).toEqual("submit");
				expect(button.prop("className")).toContain("btn");
				expect(button.text()).toEqual("Apply filters");
			});

			it("doesn't render submit filters button when mounted", () => {
				const filterPanel = shallow(<FilterPanel filters={[]} />);
				const button = filterPanel.find(".filter-panel__submit");
				expect(button.length).toEqual(0);
			});
		});

	});
});
