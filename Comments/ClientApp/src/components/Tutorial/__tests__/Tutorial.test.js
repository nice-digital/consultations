import React from "react";
import { render } from "@testing-library/react";
import { Tutorial } from "../Tutorial";
import Cookies from "js-cookie";

test("should match snapshot and display the tutorial when no cookie is set", () => {
	const {container} = render(<Tutorial />);
	expect(container).toMatchSnapshot();
});

test("should match snapshot and display the tutorial when no cookie is set to true", () => {
	Cookies.get = jest.fn().mockImplementation(() => "true");
	const {container} = render(<Tutorial />);
	expect(container).toMatchSnapshot();
});

test("should match snapshot and display the tutorial when no cookie is set to false", () => {
	Cookies.get = jest.fn().mockImplementation(() => "false");
	const {container} = render(<Tutorial />);
	expect(container).toMatchSnapshot();
});
