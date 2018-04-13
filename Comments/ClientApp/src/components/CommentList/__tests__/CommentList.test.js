import React from "react";
import { mount } from "enzyme";
import { CommentList } from "../CommentList";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { generateUrl } from "../../../data/loader";
import sampleComment from "./sample";
import { nextTick } from "../../../helpers/utils";

const mock = new MockAdapter(axios);

describe("[ClientApp] ", () => {

	describe("CommentList Component", () => {

		const fakeProps = {
			match: {
				url: "/1/1/introduction"
			}
		};

		afterEach(()=>{
			mock.reset();
		});

		it("should render a li tag with sample data ID", async () => {
			mock.onGet(generateUrl("comments", undefined, { sourceURI: fakeProps.match.url })).reply(200, sampleComment);
			const wrapper = mount(<CommentList {...fakeProps}/>);
			await nextTick();
			wrapper.update();
			expect(wrapper.find("li").text()).toEqual("1002");
		});

		it("renders the 'no comments' message if the comments array is empty", async () => {
			mock.onAny().reply(200, {comments: []});
			const wrapper = mount(<CommentList {...fakeProps}/>);
			await nextTick();
			wrapper.update();
			expect(wrapper.find("p").text()).toEqual("No comments");
		});

		it("has state with an empty array of comments", () => {
			mock.onAny().reply(200, {comments: []});
			const wrapper = mount(<CommentList {...fakeProps}/>);
			const state = wrapper.state();
			expect(Array.isArray(state.comments)).toEqual(true);
		});

		it("should make an api call with the correct path and query string", ()=>{
			mock.reset();
			mock.onGet(generateUrl("comments", undefined, { sourceURI: fakeProps.match.url })).reply((config)=>{
				expect(config.url).toEqual("/consultations/api/Comments?sourceURI=%2F1%2F1%2Fintroduction");
				return [200, {comments: []}];
			});
			mount(<CommentList {...fakeProps}/>);
		});

	});
});
