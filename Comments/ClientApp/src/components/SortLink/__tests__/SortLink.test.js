/* eslint-env jest */
import React from "react";
import { shallow } from "enzyme";

import { SortLink } from "./../SortLink";

describe("[Consultations]", () => {
	describe("SortLink", () => {

		it("renders active text when sort order active", () => {
			const sortLink = shallow(<SortLink currentSortOrder="One" sortOrder="One" text="Sort name" />);
			expect(sortLink.text()).toEqual("Sort name");
		});

		it("renders anchor with sort querystring appended when inactive", () => {
			const sortLink = shallow(<SortLink currentSortOrder="One" sortOrder="Two" text="Sort name" path="/test?a=b" />);
			expect(sortLink.find("a").prop("href")).toEqual("/test?a=b&Sort=Two");
		});

		it("renders visually hidden 'sort by' anchor text", () => {
			const sortLink = shallow(<SortLink currentSortOrder="One" sortOrder="Two" text="Sort name" path="/test?a=b" />);
			expect(sortLink.find("a .visually-hidden").text()).toEqual("Sort by");
		});

		it("prevents default and pushes history state when link clicked", () => {
			const history = { push: jest.fn() };
			const event = { preventDefault: jest.fn() };

			const sortLink = shallow(<SortLink currentSortOrder="One" sortOrder="Two" text="Sort name" path="/test?a=b" history={history} />);

			sortLink.find("a").simulate("click", event);

			expect(event.preventDefault).toHaveBeenCalled();
			expect(history.push).toHaveBeenCalledWith("/test?a=b&Sort=Two");
		});
	});
});
