import React from "react";
import { render, screen } from "@testing-library/react";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { UserContext } from "../../../context/UserContext";
import { LoginBanner } from "../LoginBanner";
import OrganisationCode from "./OrganisationCode.json";

const mock = new MockAdapter(axios);

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
			value: { isAuthorised: true },
		},
	};
});

afterEach(() => {
	mock.reset();
});

test("should match snapshot for idam login and not in comments panel", async () => {
	const {container} =  render(<LoginBanner {...fakeProps} />, contextWrapper);
	expect(container).toMatchSnapshot();
});

test("should match snapshot for idam login and in comments panel", async () => {
	fakeProps.isInCommentsPanel = true;
	fakeProps.title = "Some title";
	fakeProps.signInButton = false;
	fakeProps.signInText = null;
	const {container} = render(<LoginBanner {...fakeProps} />, contextWrapper);
	expect(container).toMatchSnapshot();
});

test("should match snapshot for code login only", async () => {
	fakeProps.allowOrganisationCodeLogin = true;
	fakeProps.codeLoginOnly = true;
	fakeProps.title = null;
	const {container} = render(<LoginBanner {...fakeProps} />, contextWrapper);
	expect(container).toMatchSnapshot();
});

test("organisation code should be prepopulated from querystring", async () => {
	const expectedOrganisationCode = "123412341234";
	const updatedProps = Object.assign(fakeProps, {location: {pathname: "/1/1/introduction", search:`?code=${expectedOrganisationCode}`}});
	render(<LoginBanner {...updatedProps} />, contextWrapper);
	const orgCodeInput = screen.getByLabelText("Enter your organisation code");
	expect(orgCodeInput.value).toEqual(expectedOrganisationCode);
});
