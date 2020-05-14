/* eslint-env jest */
import React from "react";
import { shallow } from "enzyme";

import DownloadResultsInfo from "../DownloadResultsInfo";
//import Sort from "./../../Sort/Sort";
import AppliedFilter from "./../../AppliedFilter/AppliedFilter";

const appliedFilters = [
	{
		groupId: "groupA",
		groupTitle: "Group A",
		optionId: "optionB",
		optionLabel: "Option B",
	},
	{
		groupId: "groupB",
		groupTitle: "Group B",
		optionId: "optionC",
		optionLabel: "Option C",
	},
];

describe("[Consultations]", () => {
	describe("DownloadResultsInfo", () => {
		describe("product count and loading", () => {

			it("product count shows 1 comment and 1 question when not loading", () => {
				const resultsInfo = shallow(<DownloadResultsInfo consultationCount={1} appliedFilters={[]} path="" history={null} isLoading={false} />);

				expect(resultsInfo.find("#results-info-count span").text()).toEqual("Showing 1 consultation");
			});

		 	it("product count shows plural with comments and questions when not loading", () => {
				const resultsInfo = shallow(<DownloadResultsInfo consultationCount={2} paginationPositions={{ start: 0, finish: 2 }} appliedFilters={[]} path="" history={null} isLoading={false} />);

		 		expect(resultsInfo.find("#results-info-count span").text()).toEqual("Showing 1 to 2 of 2 consultations");
		 	});

		});
	});
});
