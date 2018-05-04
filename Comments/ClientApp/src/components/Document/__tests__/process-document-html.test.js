/* eslint-env jest */

import { processDocumentHtml } from "../process-document-html";
import React from "react";
import { mount } from "enzyme";

describe("[ClientApp]", () => {
	describe("Render Document HTML", () => {

		function setupHtml(html) {
			const URI = "/1/1/guidance";
			const clickFunction = jest.fn();
			return {
				wrapper: mount(
					<div>{processDocumentHtml(html, clickFunction, URI)}</div>
				),
				clickFunction
			};
		}

		it("renders a button if the html contains an anchor with a type of 'section'", () => {
			const instance = setupHtml(
				"<div><a id='bar' href='#test' data-heading-type='section'>Foo</a></div>"
			);
			expect(instance.wrapper.find("button").text()).toEqual("Comment on section: Foo");
		});

		it("renders a button if the html contains an anchor with a type of 'chapter'", () => {
			const instance = setupHtml(
				"<div><a id='bar' href='#test' data-heading-type='chapter'>Bar</a></div>"
			);
			expect(instance.wrapper.find("button").text()).toEqual("Comment on chapter: Bar");
		});

		it("doesn't render a button if there is no anchor with a type", () => {
			const instance = setupHtml(
				"<div><a id='bar' href='#test'>Bar</a></div>"
			);
			expect(instance.wrapper.find("button").length).toEqual(0);
		});

		it("fires passed function with expected object", () => {
			const instance = setupHtml(
				"<div><a id='bar' href='#test' data-heading-type='section'>Foo</a></div>"
			);
			instance.wrapper.find("button").simulate("click");
			expect(instance.clickFunction).toHaveBeenCalledWith({
				placeholder: "Comment on Foo",
				sourceURI: "/1/1/guidance",
				commentText: "",
				commentOn: "section",
				htmlElementID: "bar"
			});
		});
	});
});
