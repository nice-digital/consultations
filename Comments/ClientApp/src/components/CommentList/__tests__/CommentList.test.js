import React from "react";
import { mount } from "enzyme";
import { MemoryRouter } from "react-router";
import { CommentList } from "../CommentList";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { generateUrl } from "../../../data/loader";
import sampleComments from "./sample";
import { nextTick } from "../../../helpers/utils";

const mock = new MockAdapter(axios);

describe("[ClientApp] ", () => {

	describe("CommentList Component", () => {

		const fakeProps = {
			match: {
				url: "/1/1/introduction"
			},
			location: {
				pathname: ""
			},
			comment: {
				commentId: 1
			}
			//staticContext: {}
		};

		afterEach(()=>{
			mock.reset();
		});

		it("should render a li tag with sample data ID", async () => {
			 mock.onGet(generateUrl("comments", undefined, [], { sourceURI: fakeProps.match.url })).reply(200, sampleComments);
			 const wrapper = mount(
			 	<MemoryRouter>
			 		<CommentList {...fakeProps}/>
				</MemoryRouter>
			 );
			 await nextTick();
			 wrapper.update();
			 expect(wrapper.find("li").length).toEqual(2);
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
			mock.onGet(generateUrl("comments", undefined, [], { sourceURI: fakeProps.match.url })).reply((config)=>{
				expect(config.url).toEqual("/consultations/api/Comments?sourceURI=%2F1%2F1%2Fintroduction");
				return [200, {comments: []}];
			});
			mount(<CommentList {...fakeProps}/>);
		});

		it("save handler posts to the api with updated comment from state", async (done) => {
			mock.reset();
			const commentToUpdate = sampleComments.comments[0];

			mock.onPut("/consultations/api/Comment/" + commentToUpdate.commentId).reply(config => {
				expect(JSON.parse(config.data)).toEqual(commentToUpdate);
				done();
				return [200, commentToUpdate];
			});

			mock.onGet(generateUrl("comments", undefined, [], { sourceURI: fakeProps.match.url })).reply(200, sampleComments);
			//console.log(sampleComments);
			const wrapper = mount(<CommentList {...fakeProps}/>);
			wrapper.instance().saveComment(
				new Event("click"),
				commentToUpdate);
		});

		it("save handler updates the correct item in the comments array once the api has returned new data", async ()=>{
			mock.reset();
			const commentToUpdate = sampleComments.comments[1];
			mock.onPut("/consultations/api/Comment/" + commentToUpdate.commentId).reply(() => {
				const sampleCommentsUpdated = sampleComments;
				sampleCommentsUpdated.comments[1].commentText = "New updated text";
				return [200, sampleCommentsUpdated.comments[1]];
			});
			mock.onGet(generateUrl("comments", undefined, [], { sourceURI: fakeProps.match.url })).reply(200, sampleComments);
			const wrapper = mount(<CommentList {...fakeProps}/>);
			wrapper.instance().saveComment(
				new Event("click"),
				commentToUpdate);
			await nextTick();
			wrapper.update();
			expect(wrapper.state().comments[1].commentText).toEqual("New updated text");
		});

		it("new comment should add an entry in the array with negative id", ()=>{
			mock.reset();
			mock.onAny().reply((config)=>{
				return [200, {comments: []}];
			});
			const wrapper = mount(<CommentList {...fakeProps}/>);
			expect(wrapper.state().comments.length).toEqual(0);
			wrapper.instance().newComment({});
			expect(wrapper.state().comments.length).toEqual(1);
			expect(wrapper.state().comments[0].commentId).toEqual(-1);
		});

		it("2 new comments should decrement the negative commentId without conflicting", ()=>{
			mock.reset();
			mock.onAny().reply((config)=>{
				return [200, {comments: []}];
			});
			const wrapper = mount(<CommentList {...fakeProps}/>);
			expect(wrapper.state().comments.length).toEqual(0);
			wrapper.instance().newComment({});
			wrapper.instance().newComment({});
			expect(wrapper.state().comments.length).toEqual(2);
			expect(wrapper.state().comments[0].commentId).toEqual(-2);
			expect(wrapper.state().comments[1].commentId).toEqual(-1);
		});

		it("where there are existing comments in the array, inserting 2 new comments should still decrement the negative commentId without conflicting", ()=>{
			mock.reset();
			mock.onGet(generateUrl("comments", undefined, [], { sourceURI: fakeProps.match.url })).reply(200, sampleComments);
			const wrapper = mount(<CommentList {...fakeProps}/>);
			expect(wrapper.state().comments.length).toEqual(0);
			wrapper.instance().newComment({});
			wrapper.instance().newComment({});
			expect(wrapper.state().comments.length).toEqual(2);
			expect(wrapper.state().comments[0].commentId).toEqual(-2);
			expect(wrapper.state().comments[1].commentId).toEqual(-1);
		});

	});
});
