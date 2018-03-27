import React from "react";
import { MemoryRouter } from "react-router-dom";
import { mount } from "enzyme";

import { StackedNav } from "../StackedNav";

describe("[ClientApp] ", () => {
	describe("StackedNav ", () => {
		const props = {
			links: {
				title: "Root Label",
				links: [
					{
						label: "Sub Link 1 Label",
						url: "sub-link-1-url",
						current: true
					},
					{
						label: "Sub Link 2 Label",
						url: "sub-link-2-url"
					},
					{
						label: "Sub Link 3 Label",
						url: "sub-link-3-url"
					}
				]
			}
		};

		let wrapper;

		beforeEach(() => {
			wrapper = mount(
				<MemoryRouter>
					<StackedNav {...props} />
				</MemoryRouter>
			);
		});

		it("should render a H2 with text that matches the supplied title", () => {
			const el = wrapper.find("h2");
			expect(el.text()).toEqual("Root Label");
		});

		it("should render the number of links supplied in props with anchors that match", () => {
			const el = wrapper.find("ul li a");
			expect(el.length).toEqual(3);
			expect(el.first().prop("href")).toEqual("sub-link-1-url");
			expect(el.last().text()).toEqual("Sub Link 3 Label");
		});

		it("should render a link with aria-current attribute set if link is current", () => {
			const el = wrapper.find("ul li a").first();
			expect(el.prop("aria-current")).toEqual("page");
		});
	});
});
