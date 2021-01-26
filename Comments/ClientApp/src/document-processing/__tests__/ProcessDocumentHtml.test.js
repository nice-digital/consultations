/* eslint-env jest */

import { ProcessDocumentHtml } from "../ProcessDocumentHtml";
import React from "react";
import { mount } from "enzyme";

describe("[ClientApp]", () => {

	describe("Render Document HTML", () => {

		function setupHtml(allowComments, html) {
			const url = "/1/1/guidance";
			const clickFunction = jest.fn();
			return {
				wrapper:
					mount(
						<div>
							<ProcessDocumentHtml content={html} onNewCommentClick={clickFunction} url={url} allowComments={allowComments}/>
						</div>,
					),
				clickFunction,
			};
		}

		it("renders a button if the html contains an anchor with a type of 'section'", () => {
			const instance = setupHtml(true,
				"<div><a id='bar' href='#test' data-heading-type='section'>Foo</a></div>",
			);
			expect(instance.wrapper.find("button").text()).toEqual("Comment on section: Foo");
		});

		it("renders a button if the html contains an anchor with a type of 'chapter'", () => {
			const instance = setupHtml(true,
				"<div><a id='bar' href='#test' data-heading-type='chapter'>Bar</a></div>",
			);
			expect(instance.wrapper.find("button").text()).toEqual("Comment on chapter: Bar");
		});

		it("doesn't render a button if there is no anchor with a type", () => {
			const instance = setupHtml(true,
				"<div><a id='bar' href='#test'>Bar</a></div>",
			);
			expect(instance.wrapper.find("button").length).toEqual(0);
		});

		it("renders a button if the html contains a numbered paragraph (subsection)", () => {
			const instance = setupHtml(true,
				"<p class='numbered-paragraph annotator-numbered-paragraph' id='np-1-3-1' data-heading-type='numbered-paragraph' title='Comment on numbered paragraph'>" +
				"        <span class='paragraph-number'>1.3.1 </span>Healthcare providers should:</p>",
			);
			expect(instance.wrapper.find("button").text()).toEqual("Comment on subsection: 1.3.1 ");
		});

		it("button fires passed function with expected object for a chapter", () => {

			const instance = setupHtml(true,
				"<h2 class=\"title\">\n" +
				"    <a id=\"recommendations\" style=\"position:relative\" class=\"annotator-chapter\" data-heading-type=\"chapter\" title=\"Comment on chapter\" xmlns=\"\">Recommendations<span class=\"annotator-adder\" /></a>\n" +
				"  </h2>",
			);
			instance.wrapper.find("button").simulate("click");
			expect(instance.clickFunction).toHaveBeenCalledWith(expect.anything(), {
				sourceURI: "/1/1/guidance",
				commentText: "",
				commentOn: "chapter",
				htmlElementID: "",
				quote: "Recommendations",
			});
		});

		it("button fires passed function with expected object for a section", () => {
			const instance = setupHtml(true,	"<div><a id='bar' href='#test' data-heading-type='section'>Foo</a></div>",
			);
			instance.wrapper.find("button").simulate("click");
			expect(instance.clickFunction).toHaveBeenCalledWith(expect.anything(), {
				sourceURI: "/1/1/guidance",
				commentText: "",
				commentOn: "section",
				htmlElementID: "bar",
				quote: "Foo",
			});
		});

		it("button fires passed function with expected object for a subsection", () => {
			const instance = setupHtml(true, "<p class='numbered-paragraph annotator-numbered-paragraph' id='np-1-3-1' data-heading-type='numbered-paragraph' title='Comment on numbered paragraph'><span class='paragraph-number'>1.3.1 </span>Healthcare providers should:</p>");
			instance.wrapper.find("button").simulate("click");
			expect(instance.clickFunction).toHaveBeenCalledWith(expect.anything(), {
				sourceURI: "/1/1/guidance",
				commentText: "",
				commentOn: "subsection",
				htmlElementID: "np-1-3-1",
				quote: "1.3.1 ",
			});
		});

		it("does not render a button when allowComments is false, even if the html contains an anchor with a type of 'section'", () => {
			const instance = setupHtml(false,
				"<div><a id='bar' href='#test' data-heading-type='section'>Foo</a></div>",
			);
			expect(instance.wrapper.find("button").length).toEqual(0);
		});

		it("does not render a button when allowComments is false, even if the html contains an anchor with a type of 'chapter'", () => {
			const instance = setupHtml(false,
				"<div><a id='bar' href='#test' data-heading-type='chapter'>Bar</a></div>",
			);
			expect(instance.wrapper.find("button").length).toEqual(0);
		});
	});
});
