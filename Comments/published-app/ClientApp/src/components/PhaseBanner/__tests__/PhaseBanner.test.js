import React from "react";
import { shallow } from "enzyme";

import { PhaseBanner } from "../PhaseBanner";

describe("[ClientApp] ", () => {
	describe("PhaseBanner ", () => {
		it("Renders with correct link", () => {
			const wrapper = shallow(
				<PhaseBanner />
			);
			expect(wrapper.find("p span a").prop("href")).toEqual(
				"https://github.com/nhsevidence/consultations"
			);
		});
	});
});
