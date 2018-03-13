import React from "react";

import { shallow } from "enzyme";
import axios from "axios";
import MockAdaptor from "axios-mock-adapter";
import Document from "./Document";
import SampleDataJson from "./../../../public/sample";
describe("[ClientApp] ", () => {
	describe("Document ", () => {
		it("Renders", () => {

			const instance = axios.create();
			const mock = new MockAdaptor(instance);

			mock
				.onGet()
				.reply(function() {
					return new Promise(function(resolve, reject) {
						resolve([200, { SampleDataJson }]);
					});
				});
			const wrapper = shallow(<Document/>);
			expect(wrapper.find("h1").length).toEqual(1);
		});
	});
});


// wrapper.instance().handleClick();
