import React from "react";
import { shallow, mount } from "enzyme";
import { MemoryRouter } from "react-router";
import CommentList from "../CommentList";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";

// import toJson from "enzyme-to-json";

describe("[ClientApp] ", () => {
	
	describe("CommentList Component", () => {
		
		const fakeProps = {
			match: {
				url: "/1/1/introduction"
			}
		};

		it("should render a li tag", () => {
			const wrapper = mount(<CommentList {...fakeProps}/>);
			expect(wrapper.find("li").text()).toEqual("1002");
		});
		
		it("state has a comments array", () => {
			const wrapper = shallow(<CommentList {...fakeProps}/>);
			var state = wrapper.state();
			expect(Array.isArray(state.comments)).toEqual(true);
		});
		
		it("log", ()=>{

			// const mock = new MockAdapter(axios);

			

			// mock.onGet("/consultations/api/Comments").reply((config)=> {
			// 	console.log(config);
			// 	return [200, {}];
			// });

			const wrapper = mount(
				<MemoryRouter>
					<CommentList {...fakeProps}/>
				</MemoryRouter>);

			

		});
		
	});
});
