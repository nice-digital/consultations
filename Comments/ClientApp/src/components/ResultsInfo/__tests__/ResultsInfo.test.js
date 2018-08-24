/* eslint-env jest */
import React from "react";
import { shallow } from "enzyme";

import ResultsInfo from "../ResultsInfo";
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

describe("[Consultations]", () => {
	describe("ResultsInfo", () => {
		describe("product count and loading", () => {
			it("product count is aria-live assertive", () => {
				const resultsInfo = shallow(<ResultsInfo commentCount={1} showCommentsCount={true} questionCount={1} showQuestionsCount={true} sortOrder="" appliedFilters={[]} path="" history={null} isLoading={false} />);

				expect(resultsInfo.find("#results-info-count").prop("aria-live")).toEqual("assertive");
			});

			it("product count shows 1 comment and 1 question when not loading", () => {
				const resultsInfo = shallow(<ResultsInfo commentCount={1} showCommentsCount={true} questionCount={1} showQuestionsCount={true}  sortOrder="" appliedFilters={[]} path="" history={null} isLoading={false} />);

				expect(resultsInfo.find("#results-info-count").text()).toEqual("Showing 1 question and 1 comment");
			});

			it("product count shows plural with comments and questions when not loading", () => {
				const resultsInfo = shallow(<ResultsInfo commentCount={2} showCommentsCount={true} questionCount={2} showQuestionsCount={true}  sortOrder="" appliedFilters={[]} path="" history={null} isLoading={false} />);

				expect(resultsInfo.find("#results-info-count").text()).toEqual("Showing 2 questions and 2 comments");
			});
			
			it("product count shows questions only when comments not allowed and zero passed", () => {
				const resultsInfo = shallow(<ResultsInfo commentCount={0} showCommentsCount={false} questionCount={2} showQuestionsCount={true}  sortOrder="" appliedFilters={[]} path="" history={null} isLoading={false} />);

				expect(resultsInfo.find("#results-info-count").text()).toEqual("Showing 2 questions");
			});

			it("product count shows questions only when comments not allowed and non-zero passed", () => {
				const resultsInfo = shallow(<ResultsInfo commentCount={2} showCommentsCount={false} questionCount={2} showQuestionsCount={true}  sortOrder="" appliedFilters={[]} path="" history={null} isLoading={false} />);

				expect(resultsInfo.find("#results-info-count").text()).toEqual("Showing 2 questions");
			});

			it("product count shows comments only when questions not allowed and zero passed", () => {
				const resultsInfo = shallow(<ResultsInfo commentCount={2} showCommentsCount={true} questionCount={0} showQuestionsCount={false}  sortOrder="" appliedFilters={[]} path="" history={null} isLoading={false} />);

				expect(resultsInfo.find("#results-info-count").text()).toEqual("Showing 2 comments");
			});

			it("product count shows comments only when questions not allowed and non-zero passed", () => {
				const resultsInfo = shallow(<ResultsInfo commentCount={2} showCommentsCount={true} questionCount={2} showQuestionsCount={false}  sortOrder="" appliedFilters={[]} path="" history={null} isLoading={false} />);

				expect(resultsInfo.find("#results-info-count").text()).toEqual("Showing 2 comments");
			});

			it("loading message shows with aria-busy when loading", () => {
				const resultsInfo = shallow(<ResultsInfo commentCount={1} showCommentsCount={true} questionCount={1} showQuestionsCount={true}  sortOrder="" appliedFilters={[]} path="" history={null} isLoading={true} />);

				const loading = resultsInfo.find("#results-info-count [aria-busy]");
				expect(loading.length).toEqual(1);
				expect(loading.text().toLowerCase()).toContain("loading");
			});
		});

		//sort tests commented out since the sort links have been commented out too.
		//describe("sort", () => {
			
			// it("sort links are hidden on print", () => {
			// 	const resultsInfo = shallow(<ResultsInfo commentCount={1} showCommentsCount={true} questionCount={1} showQuestionsCount={true}  sortOrder="" appliedFilters={[]} path="" history={null} isLoading={false} />);

			// 	expect(resultsInfo.find(".results-info__sort").is(".hide-print")).toEqual(true);
			// });

			// it("passes props to sort component", () => {
			// 	const resultsInfo = shallow(<ResultsInfo commentCount={1} showCommentsCount={true} questionCount={1} showQuestionsCount={true}  sortOrder="TestOrder" appliedFilters={[]} path="test-path" history={null} isLoading={false} />);

			// 	let sort = resultsInfo.find(Sort);

			// 	expect(sort.prop("sortOrder")).toEqual("TestOrder");
			// 	expect(sort.prop("path")).toEqual("test-path");
			// });
		//});

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
				const resultsInfo = shallow(<ResultsInfo commentCount={1} showCommentsCount={true} questionCount={1} showQuestionsCount={true}  appliedFilters={appliedFilters} path="test-path" history={null} isLoading={false} />);

				var appliedFilterComponents = resultsInfo.find(AppliedFilter);
				expect(appliedFilterComponents.at(0).prop("appliedFilter")).toEqual(appliedFilters[0]);
				expect(appliedFilterComponents.at(0).prop("path")).toEqual("test-path");
			});
		});
	});
});
