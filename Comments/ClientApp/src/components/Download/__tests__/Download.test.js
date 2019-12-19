/* global jest */

import React from "react";
import { mount } from "enzyme";
import { MemoryRouter } from "react-router";
import toJson from "enzyme-to-json";

import Download from "../Download";

describe("[ClientApp] ", () => {
	describe("Download Component", () => {
		const fakeProps = {
			match: {
				url: "/1/review",
				params: {
					consultationId: 1,
				},
			},
			location: {
				pathname: "/1/review",
				search: "?sourceURI=consultations%3A%2F%2F.%2Fconsultation%2F1%2Fdocument%2F1",
			},
			history:{
				location:{
					search: "",
				},
				listen: function(){},
			},
			basename: "/consultations",
		};

		it("should match snapshot with supplied data", () => {
			window.__PRELOADED__ = { isAuthorised: true};
			const wrapper = mount(
				<MemoryRouter>
					<Download {...fakeProps} />
				</MemoryRouter>
			);
			
			expect(
				toJson(wrapper, {
					noKey: true,
					mode: "deep",
				})
			).toMatchSnapshot();

			wrapper.unmount();
		});

	});
});