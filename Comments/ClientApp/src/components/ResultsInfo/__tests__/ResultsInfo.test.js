/* eslint-env jest */
import React from "react";
import { shallow } from "enzyme";

import ResultsInfo from "./../ResultsInfo";
import Sort from "./../../Sort/Sort";
import AppliedFilter from "./../../AppliedFilter/AppliedFilter";

const appliedFilters = [
	{
		groupId: "groupA",
		groupTitle: "Group A",
		optionId: "optionB",
		optionLabel: "Option B"
	},
	{
		groupId: "groupB",
		groupTitle: "Group B",
		optionId: "optionC",
		optionLabel: "Option C"
	}
];

describe("[TopicList]", () => {
	describe("ResultsInfo", () => {
		describe("product count and loading", () => {
			it("product count is aria-live assertive", () => {
				const resultsInfo = shallow(<ResultsInfo count={1} sortOrder="" appliedFilters={[]} path="" history={null} isLoading={false} />);

				expect(resultsInfo.find("#results-info-count").prop("aria-live")).toEqual("assertive");
			});

			it("product count shows singular with one product when not loading", () => {
				const resultsInfo = shallow(<ResultsInfo count={1} sortOrder="" appliedFilters={[]} path="" history={null} isLoading={false} />);

				expect(resultsInfo.find("#results-info-count").text()).toEqual("Showing 1 product");
			});

			it("product count shows plural with multiple products when not loading", () => {
				const resultsInfo = shallow(<ResultsInfo count={2} sortOrder="" appliedFilters={[]} path="" history={null} isLoading={false} />);

				expect(resultsInfo.find("#results-info-count").text()).toEqual("Showing 2 products");
			});

			it("loading message shows with aria-busy when loading", () => {
				const resultsInfo = shallow(<ResultsInfo count={2} sortOrder="" appliedFilters={[]} path="" history={null} isLoading={true} />);

				const loading = resultsInfo.find("#results-info-count [aria-busy]");
				expect(loading.length).toEqual(1);
				expect(loading.text().toLowerCase()).toContain("loading");
			});
		});

		describe("sort", () => {
			it("sort links are hidden on print", () => {
				const resultsInfo = shallow(<ResultsInfo count={1} sortOrder="" appliedFilters={[]} path="" history={null} isLoading={false} />);

				expect(resultsInfo.find(".results-info__sort").is(".hide-print")).toEqual(true);
			});

			it("passes props to sort component", () => {
				const resultsInfo = shallow(<ResultsInfo count={1} sortOrder="TestOrder" appliedFilters={[]} path="test-path" history={null} isLoading={false} />);

				let sort = resultsInfo.find(Sort);

				expect(sort.prop("sortOrder")).toEqual("TestOrder");
				expect(sort.prop("path")).toEqual("test-path");
			});
		});

		describe("applied filter links", () => {
			it("doesn't render applied filter list when no applied filters", () => {
				const resultsInfo = shallow(<ResultsInfo appliedFilters={[]} />);
				expect(resultsInfo.find(".results-info__filters").length).toEqual(0);
			});

			it("renders a list of applied filters", () => {
				const resultsInfo = shallow(<ResultsInfo appliedFilters={appliedFilters} />);

				expect(resultsInfo.find(".results-info__filters").length).toEqual(1);
			});

			it("applied filters links are hidden on print", () => {
				const resultsInfo = shallow(<ResultsInfo appliedFilters={appliedFilters} />);

				expect(resultsInfo.find(".results-info__filters").is(".hide-print")).toEqual(true);
			});

			it("renders an applied filter component for ever applied filter", () => {
				const resultsInfo = shallow(<ResultsInfo appliedFilters={appliedFilters} />);

				expect(resultsInfo.find(AppliedFilter).length).toEqual(2);
			});

			it("passes correct props to child applied filter components", () => {
				const resultsInfo = shallow(<ResultsInfo count={1} appliedFilters={appliedFilters} path="test-path" history={null} isLoading={false} />);

				var appliedFilterComponents = resultsInfo.find(AppliedFilter);
				expect(appliedFilterComponents.at(0).prop("appliedFilter")).toEqual(appliedFilters[0]);
				expect(appliedFilterComponents.at(0).prop("path")).toEqual("test-path");
			});
		});
	});
});
