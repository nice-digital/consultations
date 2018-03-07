import React from "react";
import { MemoryRouter } from "react-router-dom";
import { shallow, mount } from "enzyme";

import { StackedNav, RootLink, ListLink } from "./StackedNav";

const props = {
	links: {
		root: {
			label: "Root Label",
			url: "root-url"
		},
		links: [
			{
				label: "Sub Link 1 Label",
				url: "sub-link-1-url"
			},
			{
				label: "Sub Link 2 Label",
				url: "sub-link-2-url"
			}
		]
	}
};

describe("[ClientApp] ", () => {
	describe("StackedNav ", () => {
		it("Mounts", () => {
			const wrapper = shallow(<StackedNav {...props} />);
			const el = wrapper.find("nav");
			expect(el.length).toEqual(1);
		});

		it("Renders an H2 with child anchor if provided with links.root", () => {
			const wrapper = mount(
				<MemoryRouter>
					<StackedNav {...props} />
				</MemoryRouter>
			);

			const tag = wrapper.find("h2");
			expect(tag.text()).toEqual("Root Label");

			const anchor = tag.find("a");
			expect(anchor.prop("href")).toEqual("root-url");
		});

		it("Renders a link for each item in props.links", () => {
			const wrapper = mount(
				<MemoryRouter>
					<StackedNav {...props} />
				</MemoryRouter>
			);

			// there's a UL
			const ul = wrapper.find("ul");
			expect(ul.length).toEqual(1);

			// there're two LIs
			expect(ul.find("li").length).toEqual(2);

			// in one LI there's one A
			expect(
				ul
					.find("li")
					.first()
					.find("a").length
			).toEqual(1);
		});

		it("<ListLink /> renders an LI around A with supplied props", () => {
			const linkValues = props.links.links[0];

			const wrapper = mount(
				<MemoryRouter>
					<ListLink {...linkValues} />
				</MemoryRouter>
			);

			const list = wrapper.find("li");
			expect(list.length).toEqual(1);

			const anchor = wrapper.find("a");
			expect(anchor.length).toEqual(1);
			expect(anchor.prop("href")).toEqual(linkValues.url);
			expect(anchor.text()).toEqual(linkValues.label);
		});

		it("<RootLink /> renders an H2 around A with supplied props", () => {
			const linkValues = props.links.root;

			const wrapper = mount(
				<MemoryRouter>
					<RootLink {...linkValues} />
				</MemoryRouter>
			);

			const tag = wrapper.find("h2");
			expect(tag.length).toEqual(1);

			const anchor = wrapper.find("a");
			expect(anchor.length).toEqual(1);
			expect(anchor.prop("href")).toEqual(linkValues.url);
			expect(anchor.text()).toEqual(linkValues.label);
		});
	});
});
