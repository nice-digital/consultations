/* global jest */

import React from "react";
import { mount, shallow } from "enzyme";
import { MemoryRouter } from "react-router";
import CommentListWithRouter, { CommentList } from "../CommentList";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { generateUrl } from "../../../data/loader";
import sampleComments from "./sample";
import reviewComments from "./reviewComments";
import EmptyCommentsResponse from "./EmptyCommentsResponse";
import { nextTick, queryStringToObject } from "../../../helpers/utils";
//import stringifyObject from "stringify-object";

const mock = new MockAdapter(axios);

jest.mock("../../../context/UserContext", () => {
	return {
		UserContext: {
			Consumer: (props) => {
				return props.children({
					isAuthorised: true
				});
			}
		}
	};
});

describe("[ClientApp] ", () => {
	describe("CommentList Component", () => {
		const fakeProps = {
			match: {
				url: "/1/1/introduction",
				params: {
					consultationId: 1,
					documentId: 1,
					chapterSlug: "introduction"
				}
			},
			location: {
				pathname: ""
			},
			comment: {
				commentId: 1
			},
			viewComments: true
		};

		afterEach(() => {
			mock.reset();
		});

		it("should render a li tag with sample data ID", async () => {
			mock
				.onGet()
				.reply(200, sampleComments);

			const wrapper = mount(
				<MemoryRouter>
					<CommentList {...fakeProps} />
				</MemoryRouter>
			);
			await nextTick();
			wrapper.update();
			expect(wrapper.find("li").length).toEqual(5);
		});

		it("renders the 'no comments' message if the comments array is empty", async () => {
			mock.onGet().reply(200, EmptyCommentsResponse);
			const wrapper = mount(<MemoryRouter><CommentList {...fakeProps} /></MemoryRouter>);
			await nextTick();
			wrapper.update();
			expect(wrapper.find("p").last().text()).toEqual("No comments yet");
		});

		it("has state with an empty array of comments", () => {
			mock.onGet().reply(200, EmptyCommentsResponse);
			const wrapper = shallow(<MemoryRouter><CommentList {...fakeProps} /></MemoryRouter>).find("CommentList").dive();
			expect(Array.isArray(wrapper.state().comments)).toEqual(true);
		});

		it("should make an api call with the correct path and query string", () => {
			mock.reset();
			mock
				.onGet(
					generateUrl("comments", undefined, [], {
						sourceURI: fakeProps.match.url
					})
				)
				.reply(config => {
					expect(config.url).toEqual(
						"/consultations/api/Comments?sourceURI=%2F1%2F1%2Fintroduction"
					);
					return [200, { comments: [] }];
				});
			mount(<MemoryRouter><CommentList {...fakeProps} /></MemoryRouter>);
		});

		it("save handler put's to the api with updated comment from state", async done => {
			mock.reset();
			const commentToUpdate = sampleComments.comments[0];
			mock
				.onPut("/consultations/api/Comment/" + commentToUpdate.commentId)
				.reply(config => {
					expect(JSON.parse(config.data)).toEqual(commentToUpdate);
					done();
					return [200, commentToUpdate];
				});

			mock
				.onGet(
					generateUrl("comments", undefined, [], {
						sourceURI: fakeProps.match.url
					})
				)
				.reply(200, sampleComments);
			//console.log(sampleComments);
			const wrapper = mount(<MemoryRouter><CommentList {...fakeProps} /></MemoryRouter>);
			wrapper.find("CommentList").instance().saveCommentHandler(new Event("click"), commentToUpdate);
		});

		it("save handler updates the correct item in the comments array once the api has returned new data", async () => {
			mock.reset();
			const commentToUpdate = sampleComments.comments[1];
			mock
				.onPut("/consultations/api/Comment/" + commentToUpdate.commentId)
				.reply(() => {
					const sampleCommentsUpdated = sampleComments;
					sampleCommentsUpdated.comments[1].commentText = "New updated text";
					return [200, sampleCommentsUpdated.comments[1]];
				});
			mock
				.onGet()
				.reply(200, sampleComments);
			const wrapper = shallow(<MemoryRouter><CommentList {...fakeProps} /></MemoryRouter>).find("CommentList").dive();
			wrapper.instance().saveCommentHandler(new Event("click"), commentToUpdate);
			await nextTick();
			wrapper.update();
			expect(wrapper.state().comments[1].commentText).toEqual(
				"New updated text"
			);
		});

		it("new comment should add an entry in the array with negative id", () => {
			mock.reset();
			mock.onGet().reply(() => {
				return [200, { comments: [] }];
			});
			const wrapper = shallow(<MemoryRouter><CommentList {...fakeProps} /></MemoryRouter>).find("CommentList").dive();
			expect(wrapper.state().comments.length).toEqual(0);
			wrapper.instance().newComment({});
			expect(wrapper.state().comments.length).toEqual(1);
			expect(wrapper.state().comments[0].commentId).toEqual(-1);
		});

		it("2 new comments should decrement the negative commentId without conflicting", () => {
			mock.reset();
			mock.onGet().reply(() => {
				return [200, { comments: [] }];
			});
			const wrapper = shallow(<MemoryRouter><CommentList {...fakeProps} /></MemoryRouter>).find("CommentList").dive();
			expect(wrapper.state().comments.length).toEqual(0);
			wrapper.instance().newComment({});
			wrapper.instance().newComment({});
			expect(wrapper.state().comments.length).toEqual(2);
			expect(wrapper.state().comments[0].commentId).toEqual(-2);
			expect(wrapper.state().comments[1].commentId).toEqual(-1);
		});

		it("where there are existing comments in the array, inserting 2 new comments should still decrement the negative commentId without conflicting", () => {
			mock.reset();
			mock
				.onGet(
					generateUrl("comments", undefined, [], {
						sourceURI: fakeProps.match.url
					})
				)
				.reply(200, sampleComments);
			const wrapper = shallow(<MemoryRouter><CommentList {...fakeProps} /></MemoryRouter>).find("CommentList").dive();
			expect(wrapper.state().comments.length).toEqual(0);
			wrapper.instance().newComment({});
			wrapper.instance().newComment({});
			expect(wrapper.state().comments.length).toEqual(2);
			expect(wrapper.state().comments[0].commentId).toEqual(-2);
			expect(wrapper.state().comments[1].commentId).toEqual(-1);
		});

		it("save handler posts to the api with new comment", async done => {
			mock.reset();
			const commentToInsert = {
				commentId: -1,
				commentText: "a newly created comment"
			};
			mock
				.onGet(
					generateUrl("comments", undefined, [], {
						sourceURI: fakeProps.match.url
					})
				)
				.reply(200, EmptyCommentsResponse);
			mock
				.onPost(
					generateUrl("newcomment")
				)
				.reply(config => {
					expect(config.url).toEqual("/consultations/api/Comment");
					done();
					return [200, commentToInsert];
				});

			const wrapper = mount(<MemoryRouter><CommentList {...fakeProps} /></MemoryRouter>);
			wrapper.find("CommentList").instance().saveCommentHandler(new Event("click"), commentToInsert);
		});

		it("delete handler called with negative number removes item from array", async () => {
			mock.reset();
			mock
				.onGet()
				.reply(200, sampleComments);

			const wrapper = shallow(<MemoryRouter><CommentList {...fakeProps} /></MemoryRouter>).find("CommentList").dive();

			await nextTick();
			wrapper.update();

			const state = wrapper.state();

			expect(state.comments.length).toEqual(5);

			wrapper.instance().newComment({
				sourceURI: "/1/1/introduction",
				commentText: ""
			});

			expect(state.comments.length).toEqual(6);

			wrapper.instance().deleteCommentHandler(new Event("click"), -1);

			const updatedState = wrapper.state();

			expect(updatedState.comments.length).toEqual(5);
		});

		it("delete handler called with positive number hits the correct delete endpoint", async () => {
			mock.reset();
			mock
				.onGet()
				.reply(200, sampleComments);
			mock
				.onDelete(
					generateUrl("editcomment", undefined, [1004])
				)
				.reply(config => {
					expect(config.url).toEqual("/consultations/api/Comment/1004");
					return [200, {}];
				});

			const wrapper = shallow(<MemoryRouter><CommentList {...fakeProps} /></MemoryRouter>).find("CommentList").dive();
			await nextTick();
			wrapper.update();

			expect(wrapper.state().comments.length).toEqual(5);
			wrapper.instance().deleteCommentHandler(new Event("click"), 1004);

			await nextTick();
			wrapper.update();
			expect(wrapper.state().comments.length).toEqual(4);
		});

		it("should make an api call to review endpoint with the correct path and query string", () => {
			mock.reset();
			mock
				.onGet(
					generateUrl("comments", undefined, [], {})
				)
				.reply(config => {
					expect(config.url).toEqual(
						"/consultations/api/Comments"
					);
					return [200, { comments: [] }];
				});
			mount(<CommentList {...fakeProps} isReviewPage={true}  />);
		});

		it("when mounted with review property then the review endpoint is hit", ()  => {
			 mock.reset();
			 mock
			 	.onGet(
					generateUrl("comments", undefined, [], {sourceURI: "/1/0/Review", isReview: true})
			 	)
			 	.reply(config => {	 
			 		expect(config.url).toEqual(
			 			"/consultations/api/Comments?sourceURI=%2F1%2F0%2FReview&isReview=true"
					 );
			 		return [200, { comments: [] }];
				 });

			 mount(<CommentList {...fakeProps} isReviewPage={true} />);
		});
		
		//sourceURI: "/1/0/Review", isReview: true

		const firstProps = {
			match: {
				url: "/1/1/introduction",
				params: {
					consultationId: 1,
					documentId: 1,
					chapterSlug: "introduction"
				}
			},
			location: {
				pathname: "1/review",
				search: "?sourceURI=/consultations/1/1/introduction"
			},
			comment: {
				commentId: 1
			},
			viewComments: true				
		};
	});
});
