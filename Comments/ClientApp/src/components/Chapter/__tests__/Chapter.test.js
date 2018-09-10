/* eslint-env jest */

import { Chapter } from "../Chapter";
import React from "react";
import { mount } from "enzyme";
import htmlparser from "htmlparser2";
import domutils from "domutils"
//import stringifyObject from "stringify-object";

describe("[ClientApp]", () => {
	describe("Render Document HTML", () => {

		function setupHtml(allowComments, html) {
			const URI = "/1/1/guidance";
			const clickFunction = jest.fn();
			const wrapper = mount(
				<Chapter html={html} newCommentClickFunc={clickFunction} sourceURI={URI} allowComments={allowComments}/>
			)
			wrapper.update();
			let handler = new htmlparser.DomHandler();
			let parser = new htmlparser.Parser(handler);
			//console.log(wrapper.html());
			parser.write(wrapper.html());
			parser.end();
			return handler.dom;
		}

		it("renders a button if the html contains an anchor with a type of 'section'", () => {
			const dom = setupHtml(true,
				"<div><a id='bar' href='#test' data-heading-type='section'>Foo</a></div>"
			);
			let button = domutils.find((node) => node.name === "button", dom, true);
			expect(domutils.getText(button)).toEqual("Comment on section: Foo");
		});
		
		it("renders a button if the html contains an anchor with a type of 'chapter'", () => {
			const dom = setupHtml(true,
				"<div><a id='bar' href='#test' data-heading-type='chapter'>Bar</a></div>"
			);
			let button = domutils.find((node) => node.name === "button", dom, true);
			expect(domutils.getText(button)).toEqual("Comment on chapter: Bar");
		});

		it("doesn't render a button if there is no anchor with a type", () => {
			const dom = setupHtml(true,
				"<div><a id='bar' href='#test'>Bar</a></div>"
			);
			let button = domutils.find((node) => node.name === "button", dom, true);
			expect(button.length).toEqual(0);
		});

		it("renders a button if the html contains a numbered paragraph (subsection)", () => {
			const dom = setupHtml(true,
				"<div><p class='numbered-paragraph annotator-numbered-paragraph' id='np-1-3-1' data-heading-type='numbered-paragraph' title='Comment on numbered paragraph'>" +
				"        <span class='paragraph-number'>1.3.1 </span>Healthcare providers should:</p></div>"
			);
			let button = domutils.find((node) => node.name === "button", dom, true);
			expect(domutils.getText(button)).toEqual("Comment on subsection: 1.3.1 ");
		});

		//TODO: somehow test events.
		// it("button fires passed function with expected object for a chapter", () => {
		// 	const dom = setupHtml(true,
		// 		"<h2 class=\"title\">\n" +
		// 		"    <a id=\"recommendations\" style=\"position:relative\" class=\"annotator-chapter\" data-heading-type=\"chapter\" title=\"Comment on chapter\" xmlns=\"\">Recommendations<span class=\"annotator-adder\" /></a>\n" +
		// 		"  </h2>"
		// 	);
		// 	let button = domutils.find((node) => node.name === "button", dom, true);
		// 	instance.wrapper.find("button").simulate("click");
		// 	expect(instance.clickFunction).toHaveBeenCalledWith(expect.anything(), {
		// 		sourceURI: "/1/1/guidance",
		// 		commentText: "",
		// 		commentOn: "chapter",
		// 		htmlElementID: "",
		// 		quote: "Recommendations"
		// 	});
		// });

		// it("button fires passed function with expected object for a section", () => {
		// 	const instance = setupHtml(true,	"<div><a id='bar' href='#test' data-heading-type='section'>Foo</a></div>"
		// 	);
		// 	instance.wrapper.find("button").simulate("click");
		// 	expect(instance.clickFunction).toHaveBeenCalledWith(expect.anything(), {
		// 		sourceURI: "/1/1/guidance",
		// 		commentText: "",
		// 		commentOn: "section",
		// 		htmlElementID: "bar",
		// 		quote: "Foo"
		// 	});
		// });

		// it("button fires passed function with expected object for a subsection", () => {
		// 	const instance = setupHtml(true, "<p class='numbered-paragraph annotator-numbered-paragraph' id='np-1-3-1' data-heading-type='numbered-paragraph' title='Comment on numbered paragraph'><span class='paragraph-number'>1.3.1 </span>Healthcare providers should:</p>");
		// 	instance.wrapper.find("button").simulate("click");
		// 	expect(instance.clickFunction).toHaveBeenCalledWith(expect.anything(), {
		// 		sourceURI: "/1/1/guidance",
		// 		commentText: "",
		// 		commentOn: "subsection",
		// 		htmlElementID: "np-1-3-1",
		// 		quote: "1.3.1 "
		// 	});
		// });

		it("does not render a button when allowComments is false, even if the html contains an anchor with a type of 'section'", () => {
			const dom = setupHtml(false,
				"<div><a id='bar' href='#test' data-heading-type='section'>Foo</a></div>"
			);
			let button = domutils.find((node) => node.name === "button", dom, true);
			expect(button.length).toEqual(0);
		});
		
		it("does not render a button when allowComments is false, even if the html contains an anchor with a type of 'chapter'", () => {
			const dom = setupHtml(false,
				"<div><a id='bar' href='#test' data-heading-type='chapter'>Bar</a></div>"
			);
			let button = domutils.find((node) => node.name === "button", dom, true);
			expect(button.length).toEqual(0);
		});
	});
});
