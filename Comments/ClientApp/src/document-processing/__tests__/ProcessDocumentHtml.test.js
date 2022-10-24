import React from "react";
import { fireEvent, render, screen } from "@testing-library/react";
import { ProcessDocumentHtml } from "../ProcessDocumentHtml";

test("renders a button if the html contains an anchor with a type of 'section'", () => {
	const url = "/1/1/guidance";
	const clickFunction = jest.fn();
	const html = "<div><a id='bar' href='#test' data-heading-type='section'>Foo</a></div>";
	render(
		<div>
			<ProcessDocumentHtml content={html} onNewCommentClick={clickFunction} url={url} allowComments={true}/>
		</div>,
	);
	expect(screen.getByRole("button", { name: "Comment on section: Foo" })).toBeInTheDocument();
});

test("renders a button if the html contains an anchor with a type of 'chapter'", () => {
	const url = "/1/1/guidance";
	const clickFunction = jest.fn();
	const html = "<div><a id='bar' href='#test' data-heading-type='chapter'>Bar</a></div>";
	render(
		<div>
			<ProcessDocumentHtml content={html} onNewCommentClick={clickFunction} url={url} allowComments={true}/>
		</div>,
	);
	expect(screen.getByRole("button", { name: "Comment on chapter: Bar" })).toBeInTheDocument();
});

test("doesn't render a button if there is no anchor with a type", () => {
	const url = "/1/1/guidance";
	const clickFunction = jest.fn();
	const html = "<div><a id='bar' href='#test'>Bar</a></div>";
	render(
		<div>
			<ProcessDocumentHtml content={html} onNewCommentClick={clickFunction} url={url} allowComments={true}/>
		</div>,
	);
	expect(screen.queryAllByRole("button").length).toEqual(0);
});

test("renders a button if the html contains a numbered paragraph (subsection)", () => {
	const url = "/1/1/guidance";
	const clickFunction = jest.fn();
	const html = "<p class='numbered-paragraph annotator-numbered-paragraph' id='np-1-3-1' data-heading-type='numbered-paragraph' title='Comment on numbered paragraph'>" +
	"        <span class='paragraph-number'>1.3.1 </span>Healthcare providers should:</p>";
	render(
		<div>
			<ProcessDocumentHtml content={html} onNewCommentClick={clickFunction} url={url} allowComments={true}/>
		</div>,
	);
	expect(screen.getByRole("button", { name: "Comment on subsection: 1.3.1" })).toBeInTheDocument();
});

test("button fires passed function with expected object for a chapter", () => {
	const url = "/1/1/guidance";
	const clickFunction = jest.fn();
	const html = "<h2 class=\"title\">\n" +
	"    <a id=\"recommendations\" style=\"position:relative\" class=\"annotator-chapter\" data-heading-type=\"chapter\" title=\"Comment on chapter\" xmlns=\"\">1.1 Recommendations<span class=\"annotator-adder\" /></a>\n" +
	"  </h2>";
	render(
		<div>
			<ProcessDocumentHtml content={html} onNewCommentClick={clickFunction} url={url} allowComments={true}/>
		</div>,
	);
	fireEvent.click(screen.getByRole("button"));
	expect(clickFunction).toHaveBeenCalledWith(expect.anything(), {
		sourceURI: "/1/1/guidance",
		commentText: "",
		commentOn: "chapter",
		htmlElementID: "",
		sectionNumber: "1.1",
		quote: "1.1 Recommendations",
	});
});

test("button fires passed function with expected object for a section", () => {
	const url = "/1/1/guidance";
	const clickFunction = jest.fn();
	const html = "<div><a id='bar' href='#test' data-heading-type='section'>Foo</a></div>";
	render(
		<div>
			<ProcessDocumentHtml content={html} onNewCommentClick={clickFunction} url={url} allowComments={true}/>
		</div>,
	);
	fireEvent.click(screen.getByRole("button"));
	expect(clickFunction).toHaveBeenCalledWith(expect.anything(), {
		sourceURI: "/1/1/guidance",
		commentText: "",
		commentOn: "section",
		htmlElementID: "bar",
		sectionNumber: undefined,
		quote: "Foo",
	});
});

test("button fires passed function with expected object for a subsection", () => {
	const url = "/1/1/guidance";
	const clickFunction = jest.fn();
	const html = "<p class='numbered-paragraph annotator-numbered-paragraph' id='np-1-3-1' data-heading-type='numbered-paragraph' title='Comment on numbered paragraph'><span class='paragraph-number'>1.3.1 </span>Healthcare providers should:</p>";
	render(
		<div>
			<ProcessDocumentHtml content={html} onNewCommentClick={clickFunction} url={url} allowComments={true}/>
		</div>,
	);
	fireEvent.click(screen.getByRole("button"));
	expect(clickFunction).toHaveBeenCalledWith(expect.anything(), {
		sourceURI: "/1/1/guidance",
		commentText: "",
		commentOn: "subsection",
		htmlElementID: "np-1-3-1",
		sectionNumber: "1.3.1",
		quote: "1.3.1 ",
	});
});

test("does not render a button when allowComments is false, even if the html contains an anchor with a type of 'section'", () => {
	const url = "/1/1/guidance";
	const clickFunction = jest.fn();
	const html = "<div><a id='bar' href='#test' data-heading-type='section'>Foo</a></div>";
	render(
		<div>
			<ProcessDocumentHtml content={html} onNewCommentClick={clickFunction} url={url} allowComments={false}/>
		</div>,
	);
	expect(screen.queryAllByRole("button").length).toEqual(0);
});

test("does not render a button when allowComments is false, even if the html contains an anchor with a type of 'chapter'", () => {
	const url = "/1/1/guidance";
	const clickFunction = jest.fn();
	const html = "<div><a id='bar' href='#test' data-heading-type='chapter'>Bar</a></div>";
	render(
		<div>
			<ProcessDocumentHtml content={html} onNewCommentClick={clickFunction} url={url} allowComments={false}/>
		</div>,
	);
	expect(screen.queryAllByRole("button").length).toEqual(0);
});
