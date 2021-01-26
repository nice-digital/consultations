/* eslint-env jest */
import React from "react";
import { shallow } from "enzyme";

import { DownloadResultsInfo } from "../DownloadResultsInfo";

describe("[Consultations]", () => {
	describe("DownloadResultsInfo", () => {
		describe("product count and loading", () => {

			it("product count shows 1 comment and 1 question when not loading", () => {
				const resultsInfo = shallow(<DownloadResultsInfo consultationCount={1} paginationPositions={{ start: 0, finish: 1 }} appliedFilters={[]} path="" history={null} isLoading={false} />);

				expect(resultsInfo.find("#results-info-count span").text()).toEqual("Showing 1 consultation");
			});

			it("product count shows plural with comments and questions when not loading", () => {
				const resultsInfo = shallow(<DownloadResultsInfo consultationCount={51} paginationPositions={{ start: 0, finish: 25 }} appliedFilters={[]} path="" history={null} isLoading={false} />);

				expect(resultsInfo.find("#results-info-count span").text()).toEqual("Showing 1 to 25 of 51 consultations");
			});

		});
	});
});
