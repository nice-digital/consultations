import React from "react";
import { shallow } from "enzyme";

import { PhaseBanner } from "../PhaseBanner";
import toJson from "enzyme-to-json";

describe("[ClientApp] ", () => {
	describe("PhaseBanner ", () => {
		it("Matches snapshot", () => {
			const FakeProps = {
				name: "My Project",
				repo: "http://mygitrepo.com",
				phase: "delta",
			};
			const wrapper = shallow(<PhaseBanner {...FakeProps}/>);
			expect(
				toJson(wrapper, {
					noKey: true,
					mode: "deep",
				}),
			).toMatchSnapshot();
		});
	});
});
