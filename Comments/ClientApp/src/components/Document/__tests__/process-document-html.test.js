/* eslint-env jest */

import { processDocumentHtml } from "../process-document-html";
import React, { Fragment } from "react";
import { mount } from "enzyme";

describe("[ClientApp]", () => {
	describe("Render Document HTML", () => {
		const html =
			"<div><a id='bar' href='#test' data-heading-type='section'>Foo</a></div>";
		const clickFunction = jest.fn();
		const URI = "/1/1/guidance";

		it("renders a button if the html contains an anchor with a type of 'section'", () => {
			const wrapper = mount(
				<div>{processDocumentHtml(html, clickFunction, URI)}</div>
			);

			expect(wrapper.find("button").text()).toEqual("Comment on section: Foo");
		});

		it("fires passed function with expected object", () => {
			const wrapper = mount(
				<div>{processDocumentHtml(html, clickFunction, URI)}</div>
			);

			wrapper.find("button").simulate("click");
			expect(clickFunction).toHaveBeenCalledWith({
				placeholder: "Comment on Foo",
				sourceURI: "/1/1/guidance",
				commentText: "",
				commentOn: "Section",
				htmlElementID: "bar"
			});
		});
	});
});
