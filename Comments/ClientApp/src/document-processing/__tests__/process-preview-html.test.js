import React from "react";
import { render } from "@testing-library/react";
import { processPreviewHtml } from "../process-preview-html";

const consoleErrorReset = console.error;

beforeEach(() => {
	console.error = consoleErrorReset;
});

test("doesn't render a comment button if there's no comment in the markup", () => {
	const {container} = render(
		<div>
			{processPreviewHtml("<p><a href='#'>Hey!</a> Ain't no comment anywhere <span>here</span></p>")}
		</div>,
	);
	expect(container.getElementsByClassName("ConversionError").length).toEqual(0);
});

test("renders a comment box if the markup contains a preview error comment", () => {
	console.error = jest.fn();
	const {container} = render(
		<div>
			{processPreviewHtml("<div><p>Here is a paragraph <!--[I] - Information: Soft return used in paragraph--></p></div>")}
		</div>,
	);
	expect(container.getElementsByClassName("ConversionError").length).toEqual(1);
});

test("renders the correct number of error markers for the type of errors supplied", () => {
	console.error = jest.fn();
	const {container} = render(
		<div>
			{processPreviewHtml(
				`<div>
					<p>Here is a paragraph <!--[I] - Information: You have been informed! --></p>
					<div>
						<ul>
							<li>
								<a href="#">Here is an anchor! <!--[E] - Error: There's an error! --></a>
							</li>
						</ul>
					</div>
					<table>
						<thead>
						<tr>
							<th>Heading <!--[I] - Information: You have been informed! --></th>
						</tr>
						</thead>
						<tbody>
						<tr>
							<td>This is a cell inside a table <!--[W] - Warning: You have been warned! --></td>
						</tr>
						</tbody>
					</table>
				</div>`,
			)}
		</div>,
	);
	expect(container.getElementsByClassName("ConversionError").length).toEqual(4);
	expect(container.querySelectorAll(".ConversionError.ConversionError--W").length).toEqual(1);
	expect(container.querySelectorAll(".ConversionError.ConversionError--E").length).toEqual(1);
	expect(container.querySelectorAll(".ConversionError.ConversionError--I").length).toEqual(2);
});

test("renders a comment box of the appropriate styling depending on the type of error", () => {
	console.error = jest.fn();
	const {container} = render(
		<div>
			{processPreviewHtml(
				`<div>
					<p>Here is a paragraph <!--[I] - Information: You have been informed! --></p>
					<div>
						<ul>
							<li>
								<a href="#">Here is an anchor! <!--[E] - Error: There's an error! --></a>
							</li>
						</ul>
					</div>
					<table>
						<thead>
						<tr>
							<th>Heading</th>
						</tr>
						</thead>
						<tbody>
						<tr>
							<td>This is a cell inside a table <!--[W] - Warning: You have been warned! --></td>
						</tr>
						</tbody>
					</table>

				</div>`,
			)}
		</div>,
	);
	expect(container.querySelectorAll(".ConversionError.ConversionError--W ul li")[0].textContent).toEqual("You have been warned!");
	expect(container.querySelectorAll(".ConversionError.ConversionError--E ul li")[0].textContent).toEqual("There's an error!");
	expect(container.querySelectorAll(".ConversionError.ConversionError--I ul li")[0].textContent).toEqual("You have been informed!");
});
