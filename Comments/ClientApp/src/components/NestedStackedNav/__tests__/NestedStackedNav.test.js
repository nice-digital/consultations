import React from "react";
import {MemoryRouter} from "react-router-dom";
import {mount} from "enzyme";
import toJson from "enzyme-to-json";
import {NestedStackedNav} from "../NestedStackedNav";

describe("[ClientApp] ", () => {
	describe("NestedStackedNav ", () => {
		const props = {
			navigationStructure:
				[
					{
						title: "consultation name",
						to: "/admin/questions/1/0",
						marker: 1,
						current: true,
						children: [
							{
								title: "consultation doc 1",
								to: "/admin/questions/1/1",
								marker: 1,
								current: false,
							},
							{
								title: "consultation doc 2",
								to: "/admin/questions/1/2",
								marker: 1,
								current: false,
							},
							{
								title: "consultation doc 3",
								to: "/admin/questions/1/3",
								marker: 1,
								current: false,
							},
							{
								title: "consultation doc 4",
								to: "/admin/questions/1/4",
								marker: 1,
								current: false,
							},
						],
					},
				],
		};

		it("should match the snapshot", () => {
			const wrapper = mount(
				<MemoryRouter>
					<NestedStackedNav {...props} />
				</MemoryRouter>,
			);
			expect(
				toJson(wrapper, {
					noKey: true,
					mode: "deep",
				})
			).toMatchSnapshot();
		});

		it("should render no markup if no documents are supplied", function () {
			const props = {};
			const wrapper = mount(
				<MemoryRouter>
					<NestedStackedNav {...props} />
				</MemoryRouter>,
			);
			const el = wrapper.find(".NestedStackedNav");
			expect(el.length).toEqual(0);
		});
	});
});
