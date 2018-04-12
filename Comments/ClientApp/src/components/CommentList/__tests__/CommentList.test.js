import React from "react";
import { shallow, mount } from "enzyme";
import { MemoryRouter } from "react-router";
import CommentList from "../CommentList";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { generateUrl } from "../../../data/loader";

const mock = new MockAdapter(axios);

// import toJson from "enzyme-to-json";

describe("[ClientApp] ", () => {
	
	describe("CommentList Component", () => {
		
		const fakeProps = {
			match: {
				url: "/1/1/introduction"
			}
		};

		it("should render a li tag", () => {
			mock.onAny().reply(200);
			const wrapper = mount(<CommentList {...fakeProps}/>);
			expect(wrapper.find("li").text()).toEqual("1002");
		});
		
		it("state has a comments array", () => {
			mock.onAny().reply(200);
			const wrapper = shallow(<CommentList {...fakeProps}/>);
			var state = wrapper.state();
			expect(Array.isArray(state.comments)).toEqual(true);
		});
		
		it("log", ()=>{
			mock.reset();

			const url = generateUrl("comments", undefined, { monkey: true });
			
			mock.onGet(url).reply((config)=>{
				console.log(config);
				console.log("inside mock reply");
				return [ 200, {
					monkey: false,
					hello: true
				} ];
			});

			const wrapper = mount(				
				<CommentList {...fakeProps}/>
			);

			console.log(wrapper.html());
			// var url = generateUrl("comments");
			// console.log("mocking: " + url);
			
			// mock.onGet(/.*/).reply((config)=> {
			// 	console.log("hit our mock");
			// 	console.log(config);
			// 	return [200, {}];
			// });

			// axios.get('/users')
			// .then(function(response) {
			// 	console.log("hello");
			// });			
			
			

			

		});
		
	});
});
