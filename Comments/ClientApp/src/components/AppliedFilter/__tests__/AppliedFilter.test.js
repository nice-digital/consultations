/* eslint-env jest */
import React from "react";
import { shallow } from "enzyme";

import { AppliedFilter } from "../AppliedFilter";

describe("[TopicList]", () => {
	describe("AppliedFilter", () => {

		const appliedFilterModel = {
			groupTitle: "Group title",
			optionLabel: "Option label",
			groupId: "gId",
			optionId: "oId"
		};

		it("renders a tag with remove anchor wrapped in a list item", () => {
			const appliedFilter = shallow(<AppliedFilter appliedFilter={appliedFilterModel} path="/test" />);
			expect(appliedFilter.is("li")).toEqual(true);
			expect(appliedFilter.find("li .tag").length).toEqual(1);
			expect(appliedFilter.find("li .tag a.tag__remove").length).toEqual(1);
		});

		it("remove anchor has visually hidden link text for screenreaders", () => {
			const appliedFilter = shallow(<AppliedFilter appliedFilter={appliedFilterModel} path="/test" />);
			const visuallyHiddenText = appliedFilter.find("li .tag a .visually-hidden").text();
			expect(visuallyHiddenText.toLowerCase()).toContain("remove");
			expect(visuallyHiddenText).toContain("Group title: Option label");
		});

		it("getHref removes group and option id from query string", () => {
			const appliedFilter = shallow(<AppliedFilter appliedFilter={appliedFilterModel} path="/test?a=b&gId=oId" />);
			expect(appliedFilter.instance().getHref()).toEqual("/test?a=b");
		});

		it("tag remove anchor href is set correctly", () => {
			const appliedFilter = shallow(<AppliedFilter appliedFilter={appliedFilterModel} path="/test?a=b&gId=oId" />);
			expect(appliedFilter.find("li .tag a.tag__remove").prop("href")).toEqual("/test?a=b");
		});

		it("prevents default and pushes history state when remove link is clicked", () => {
			const history = { push: jest.fn() };
			const event = { preventDefault: jest.fn() };

			const appliedFilter = shallow(<AppliedFilter appliedFilter={appliedFilterModel} history={history} path="/test?a=b&gId=oId" />);

			appliedFilter.find("a").simulate("click", event);

			expect(event.preventDefault).toHaveBeenCalled();
			expect(history.push).toHaveBeenCalledWith(appliedFilter.instance().getHref());
		});

	});
});
