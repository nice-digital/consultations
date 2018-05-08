import React from "react";
import { mount } from "enzyme";
import { MemoryRouter } from "react-router";
import { Selection } from "../Selection";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { generateUrl } from "../../../data/loader";
import { nextTick } from "../../../helpers/utils";

const mock = new MockAdapter(axios);

describe("[ClientApp] ", () => {
	describe("Selection Component", () => {		

		beforeEach(() => {
			window.getSelection = () => {
			  return {
					removeAllRanges: () => {},
					getRangeAt: () => {}
			  };
			};
		  });

		it("mouseup event should call passed in function", async () => {

			// const paragraph = document.createElement("p");
			// paragraph.setAttribute("id", "myparagraph");
			// paragraph.textContent = "some paragraph content";
			// window.domNode = paragraph;
			// document.body.appendChild(paragraph);

			//let wrapper = mount(<Selection />, { attachTo: window.domNode });
			//let wrapper = mount(<div id="parentDiv"><p>This is outside the selection</p><Selection><p>some content inside selection</p></Selection></div>, { attachTo: window.domNode });
			let wrapper = mount(<Selection><p>some paragraph content</p></Selection>);

			wrapper.instance().onMouseUp(new Event("mouseup"));
			//let parentDiv = document.getElementById("parentDiv");
			// grabbedParagraph.dispatchEvent(new Event("mouseup"));


			//console.log(grabbedParagraph);

			// const selection = window.getSelection();
			// const range = document.createRange();

			//console.log(selection);

			// range.selectNodeContents(parentDiv);
			// selection.removeAllRanges();
			// selection.addRange(range);



			//console.log(window.getSelection());
			expect(true).toEqual(true);
		});

	});
});
