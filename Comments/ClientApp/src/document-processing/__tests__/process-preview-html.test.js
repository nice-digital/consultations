/* eslint-env jest */

import { processPreviewHtml } from "../process-preview-html";
import React from "react";
import { mount } from "enzyme";

describe("[ClientApp]", () => {
	describe("Render Preview HTML", () => {

		function setupHtml(html) {
			return {
				wrapper: mount(
					<div>{processPreviewHtml(html)}</div>
				),
			};
		}

		it("doesn't render a comment button if there's no comment in the markup", () => {
			const instance = setupHtml(
				"<p><a href='#'>Hey!</a> Ain't no comment anywhere <span>here</span></p>"
			);
			expect(instance.wrapper.find("div.ConversionError").length).toEqual(0);
		});

		it("renders a comment box if the markup contains a preview error comment", () => {
			const instance = setupHtml(
				"<div><p>Here is a paragraph <!--[I] - Information: Soft return used in paragraph--></p></div>"
			);
			expect(instance.wrapper.find("div.ConversionError").length).toEqual(1);
		});

		it("renders the correct number of error markers for the type of errors supplied", () => {
			const instance = setupHtml(
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

				</div>`
			);

			expect(instance.wrapper.find("div.ConversionError").length).toEqual(4);
			expect(instance.wrapper.find("div.ConversionError.ConversionError--W").length).toEqual(1);
			expect(instance.wrapper.find("div.ConversionError.ConversionError--E").length).toEqual(1);
			expect(instance.wrapper.find("div.ConversionError.ConversionError--I").length).toEqual(2);
		});

		it("renders a comment box of the appropriate styling depending on the type of error", () => {
			const instance = setupHtml(
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

				</div>`
			);
			expect(instance.wrapper.find("div.ConversionError.ConversionError--W ul li").text()).toEqual("You have been warned!");
			expect(instance.wrapper.find("div.ConversionError.ConversionError--E ul li").text()).toEqual("There's an error!");
			expect(instance.wrapper.find("div.ConversionError.ConversionError--I ul li").text()).toEqual("You have been informed!");
		});

		it("should show the detail of the error when clicked by adding a class to make it visible", () => {
			const instance = setupHtml(
				"<p>Here is a paragraph <!--[I] - Information: You have been informed! --></p>"
			);
			const wrapper = instance.wrapper;
			expect(wrapper).toMatchSnapshot();
			wrapper.find("button.ConversionError__Button").simulate("click");
			expect(wrapper).toMatchSnapshot();
		});

	});
});
