/* global jest */

import React from "react";
import { Tutorial } from "../Tutorial";
import { shallow } from "enzyme";
import toJson from "enzyme-to-json";
import Cookies from "js-cookie";

describe("[ClientApp] ", () => {
	describe("Tutorial Component", () => {
		it("should match snapshot and display the tutorial when no cookie is set", () => {
			const wrapper = shallow(
				<Tutorial />,
			);
			expect(
				toJson(wrapper, {
					noKey: true,
					mode: "deep",
				}),
			).toMatchSnapshot();
		});

		it("should match snapshot and display the tutorial when no cookie is set to true", () => {
			Cookies.get = jest.fn().mockImplementation(() => "true");
			const wrapper = shallow(
				<Tutorial />,
			);
			expect(
				toJson(wrapper, {
					noKey: true,
					mode: "deep",
				}),
			).toMatchSnapshot();
		});

		it("should match snapshot and display the tutorial when no cookie is set to false", () => {
			Cookies.get = jest.fn().mockImplementation(() => "false");
			const wrapper = shallow(
				<Tutorial />,
			);
			expect(
				toJson(wrapper, {
					noKey: true,
					mode: "deep",
				}),
			).toMatchSnapshot();
		});
	});
});