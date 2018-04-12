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
		
		it("the api url call hits the correct endpoint", ()=>{
			mock.reset();

			const url = generateUrl("comments", undefined, { sourceURI: fakeProps.match.url });
			
			mock.onGet(url).reply((config)=>{
				expect(config.url).toEqual("/consultations/api/Comments?sourceURI=%2F1%2F1%2Fintroduction");
				return [200, {}];
			});

			mount(<CommentList {...fakeProps}/>);
		});
		
	});
});
