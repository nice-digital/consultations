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

		it("mouseup event should call passed in function", async () => {

			let wrapper = mount(<Selection><p>Some content within the selection</p></Selection>);

			console.log(wrapper);
			expect(true).toEqual(false);
		});

	});
});
