/* eslint-env jest */
import React from "react";
import { mount } from "enzyme";
import { MemoryRouter } from "react-router";
import toJson from "enzyme-to-json";

import { LoginPanel } from "../LoginPanel";

const loginPanelFakeProps = {
	organisationalCommentingFeature: true,
	match: {
		url: "/1/1/introduction",
	},
};

describe("LoginPanel", () => {

	it("shows first menu screen when first loaded", () => {
		const wrapper = mount(<LoginPanel { ...loginPanelFakeProps } />);

		expect(wrapper.find("#loginPanelScreen1").exists()).toBe(true);
	});

	it("shows second menu screen when organisation option has been selected", () => {
		const wrapper = mount(<LoginPanel { ...loginPanelFakeProps } />);
		const radioButton = wrapper.find("#respondingAsOrg--organisation");

		expect(wrapper.find("#loginPanelScreen1").exists()).toBe(true);

		radioButton.simulate("change");
		wrapper.update();

		expect(wrapper.find("#loginPanelScreen1").exists()).toBe(false);
		expect(wrapper.find("#loginPanelScreen2").exists()).toBe(true);
	});

	it("shows login when individual option has been selected", () => {
		const wrapper = mount(
			<MemoryRouter>
				<LoginPanel { ...loginPanelFakeProps } />
			</MemoryRouter>,
		);
		const radioButton = wrapper.find("#respondingAsOrg--individual");

		expect(wrapper.find("#loginPanelScreen1").exists()).toBe(true);

		radioButton.simulate("change");
		wrapper.update();

		expect(wrapper.find("#loginPanelScreen1").exists()).toBe(false);
		expect(wrapper.find("#loginBanner").exists()).toBe(true);
		expect(wrapper.find("h3").text()).toEqual("Make or review comments as an individual");
	});

	it("shows second screen and organisation code text box when 'user has code' is selected", () => {
		const wrapper = mount(
			<MemoryRouter>
				<LoginPanel { ...loginPanelFakeProps } />
			</MemoryRouter>,
		);
		const radioButton = wrapper.find("#respondingAsOrg--organisation");

		expect(wrapper.find("#loginPanelScreen1").exists()).toBe(true);

		radioButton.simulate("change");
		wrapper.update();

		expect(wrapper.find("#loginPanelScreen1").exists()).toBe(false);
		expect(wrapper.find("#loginPanelScreen2").exists()).toBe(true);

		const radioButton2 = wrapper.find("#respondingAsOrgType--code");

		radioButton2.simulate("change");
		wrapper.update();

		expect(wrapper.find("#loginPanelScreen2").exists()).toBe(true);
		expect(wrapper.find("#loginBanner").exists()).toBe(true);
		expect(wrapper.find(".input__label").text()).toEqual("Enter your organisation code");
	});

	it("goes back to previous screen when back has been clicked", () => {
		const wrapper = mount(
			<MemoryRouter>
				<LoginPanel { ...loginPanelFakeProps } />
			</MemoryRouter>,
		);
		const radioButton = wrapper.find("#respondingAsOrg--individual");

		expect(wrapper.find("#loginPanelScreen1").exists()).toBe(true);

		radioButton.simulate("change");
		wrapper.update();

		expect(wrapper.find("#loginPanelScreen1").exists()).toBe(false);
		expect(wrapper.find("#loginBanner").exists()).toBe(true);

		const backButton = wrapper.find("#loginPanelBackButton");

		backButton.simulate("mousedown");
		wrapper.update();

		expect(wrapper.find("#loginBanner").exists()).toBe(false);
		expect(wrapper.find("#loginPanelScreen1").exists()).toBe(true);
	});

	it("clears radio buttons after navigating back to previous screen", () => {
		const wrapper = mount(
			<MemoryRouter>
				<LoginPanel { ...loginPanelFakeProps } />
			</MemoryRouter>,
		);
		const radioButton = wrapper.find("#respondingAsOrg--individual");

		expect(wrapper.find("#loginPanelScreen1").exists()).toBe(true);

		radioButton.simulate("change");
		wrapper.update();

		expect(wrapper.find("#loginPanelScreen1").exists()).toBe(false);
		expect(wrapper.find("#loginBanner").exists()).toBe(true);

		const backButton = wrapper.find("#loginPanelBackButton");

		backButton.simulate("mousedown");
		wrapper.update();

		expect(wrapper.find("#loginBanner").exists()).toBe(false);
		expect(wrapper.find("#loginPanelScreen1").exists()).toBe(true);

		const allRadioButtons = wrapper.find({ type: "radio" });

		allRadioButtons.forEach(node => {
			expect(node.props().checked).toEqual(false);
    	});
	});

	it("should match the snapshot after first loading up", () => {
		const wrapper = mount(
			<MemoryRouter>
				<LoginPanel { ...loginPanelFakeProps } />
			</MemoryRouter>,
		);
		wrapper.update();
		expect(toJson(wrapper, { noKey: true, mode: "deep" })).toMatchSnapshot();
	});
});
