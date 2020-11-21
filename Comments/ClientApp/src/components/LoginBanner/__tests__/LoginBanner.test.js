/* global jest */

import React from "react";
import { mount } from "enzyme";
import { MemoryRouter } from "react-router";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import toJson from "enzyme-to-json";

import { nextTick } from "../../../helpers/utils";
import OrganisationCode from "./OrganisationCode.json";
import { LoginBanner } from "../LoginBanner";

const mock = new MockAdapter(axios);

jest.mock("../../../context/UserContext", () => {
	return {
		UserContext: {
			Consumer: (props) => {
				return props.children({
					isAuthorised: true,
				});
			},
		},
	};
});

describe("[ClientApp] ", () => {
	describe("LoginBanner Component", () => {
		const fakeProps = {
			match: {
				url: "/1/1/introduction",
				params: {
					consultationId: 1,
				},
			},
			location: {
				pathname: "/1/1/introduction",
				search: "",
			},
			history:{
				location:{
					search: "",
				},
				listen: function(){},
			},
			signInURL: "/signin-url",
			registerURL: "/register-url",
			signInButton: true,
			signInText: "signin text",
		};

		beforeEach(() => {
			mock
				.onGet()
				.reply(200, OrganisationCode);
		});

		afterEach(() => {
			window.__PRELOADED__ = { isAuthorised: true};
			mock.reset();
		});

		it("should match snapshot. no querystring", async () => {
				 
			const wrapper = mount(				
				<LoginBanner {...fakeProps} />				
			);
			
			await nextTick();
			wrapper.update();

			expect(toJson(wrapper, {noKey: true, mode: "deep",})).toMatchSnapshot();

			wrapper.unmount();
		});

		it("organisation code should be prepopulated from querystring", async () => {

			const expectedOrganisationCode = "123412341234";
			var updatedProps = Object.assign(fakeProps, {location: {pathname: "/1/1/introduction", search:`?code=${expectedOrganisationCode}`}})

			const wrapper = mount(				
				<LoginBanner {...updatedProps} />				
			);
			
			await nextTick();
			wrapper.update();

			expect(wrapper.state().userEnteredCollationCode).toEqual(expectedOrganisationCode);

			expect(toJson(wrapper, {noKey: true, mode: "deep",})).toMatchSnapshot();

			wrapper.unmount();
		});

	});
});