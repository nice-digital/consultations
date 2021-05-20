/* eslint-env jest */

import React from "react";
import { mount } from "enzyme";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import toJson from "enzyme-to-json";

import { nextTick } from "../../../helpers/utils";
import OrganisationCode from "./OrganisationCode.json";
import { UserContext } from "../../../context/UserContext";
import { LoginBanner } from "../LoginBanner";

const mock = new MockAdapter(axios);

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

		let contextWrapper = null;

		beforeEach(() => {
			mock
				.onGet()
				.reply(200, OrganisationCode);

			contextWrapper = {
				wrappingComponent: UserContext.Provider,
				wrappingComponentProps: {
					value: { isAuthorised: true, organisationalCommentingFeature: true },
				},
			};
		});

		afterEach(() => {
			mock.reset();
		});

		it("should match snapshot for idam login and not in comments panel", async () => {
			const wrapper = mount(
				<LoginBanner {...fakeProps} />,
				contextWrapper,
			);

			await nextTick();
			wrapper.update();

			expect(toJson(wrapper, {noKey: true, mode: "deep"})).toMatchSnapshot();

			wrapper.unmount();
		});

		it("should match snapshot for idam login and in comments panel", async () => {
			fakeProps.isInCommentsPanel = true;
			fakeProps.title = "Some title";
			fakeProps.signInButton = false;
			fakeProps.signInText = null;

			const wrapper = mount(
				<LoginBanner {...fakeProps} />,
				contextWrapper,
			);

			await nextTick();
			wrapper.update();

			expect(toJson(wrapper, {noKey: true, mode: "deep"})).toMatchSnapshot();

			wrapper.unmount();
		});

		it("should match snapshot for code login only", async () => {
			fakeProps.allowOrganisationCodeLogin = true;
			fakeProps.codeLoginOnly = true;
			fakeProps.title = null;

			const wrapper = mount(
				<LoginBanner {...fakeProps} />,
				contextWrapper,
			);

			await nextTick();
			wrapper.update();

			expect(toJson(wrapper, {noKey: true, mode: "deep"})).toMatchSnapshot();

			wrapper.unmount();
		});

		it("organisation code should be prepopulated from querystring", async () => {
			const expectedOrganisationCode = "123412341234";
			const updatedProps = Object.assign(fakeProps, {location: {pathname: "/1/1/introduction", search:`?code=${expectedOrganisationCode}`}});

			const wrapper = mount(
				<LoginBanner {...updatedProps} />,
				contextWrapper,
			);

			await nextTick();
			wrapper.update();

			expect(wrapper.state().userEnteredCollationCode).toEqual(expectedOrganisationCode);

			wrapper.unmount();
		});

	});
});
