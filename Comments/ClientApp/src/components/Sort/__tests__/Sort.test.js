/* eslint-env jest */
import React from "react";
import { shallow } from "enzyme";

import { Sort } from "./../Sort";

describe("[TopicList]", () => {
	describe("Sort", () => {

		it("renders two sort links", () => {
			const sort = shallow(<Sort sortOrder="TestOrder" path="test-path" />);

			expect(sort.find("[sortOrder]").length).toEqual(2);
		});

		it("renders date desc sort link with correct props", () => {
			const sort = shallow(<Sort sortOrder="TestOrder" path="test-path" />);

			const sortLink = sort.find({ sortOrder: "DateDesc" });

			expect(sortLink.prop("currentSortOrder")).toEqual("TestOrder");
			expect(sortLink.prop("path")).toEqual("test-path");
		});

		it("renders title asc sort link with correct props", () => {
			const sort = shallow(<Sort sortOrder="TestOrder" path="test-path" />);

			const sortLink = sort.find({ sortOrder: "TitleAsc" });

			expect(sortLink.prop("currentSortOrder")).toEqual("TestOrder");
			expect(sortLink.prop("path")).toEqual("test-path");
		});
	});
});
