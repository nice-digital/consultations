/* global jest */

import React from "react";
import { mount } from "enzyme";
import { MemoryRouter } from "react-router";
import CommentListWithRouter, { CommentList } from "../CommentList";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { generateUrl } from "../../../data/loader";
import sampleComments from "./sample";
import reviewComments from "./reviewComments";
import EmptyCommentsResponse from "./EmptyCommentsResponse";
import { nextTick, queryStringToObject } from "../../../helpers/utils";

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
				url: "/1/1/introduction"
			},
			location: {
				pathname: ""
			},
			comment: {
				commentId: 1
			}
		};

		afterEach(() => {
			mock.reset();
		});

		it("should render a li tag with sample data ID", async () => {
			mock
				.onGet(
					generateUrl("comments", undefined, [], {
						sourceURI: fakeProps.match.url
					})
				)
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
			mock.onAny().reply(200, { comments: [] });
			const wrapper = mount(<CommentList {...fakeProps} />);
			await nextTick();
			wrapper.update();
			expect(wrapper.find("p").text()).toEqual("No comments yet");
		});

		it("has state with an empty array of comments", () => {
			mock.onAny().reply(200, { comments: [] });
			const wrapper = mount(<CommentList {...fakeProps} />);
			const state = wrapper.state();
			expect(Array.isArray(state.comments)).toEqual(true);
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
			mount(<CommentList {...fakeProps} />);
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
			const wrapper = mount(<CommentList {...fakeProps} />);
			wrapper.instance().saveCommentHandler(new Event("click"), commentToUpdate);
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
				.onGet(
					generateUrl("comments", undefined, [], {
						sourceURI: fakeProps.match.url
					})
				)
				.reply(200, sampleComments);
			const wrapper = mount(<CommentList {...fakeProps} />);
			wrapper.instance().saveCommentHandler(new Event("click"), commentToUpdate);
			await nextTick();
			wrapper.update();
			expect(wrapper.state().comments[1].commentText).toEqual(
				"New updated text"
			);
		});

		it("new comment should add an entry in the array with negative id", () => {
			mock.reset();
			mock.onAny().reply(() => {
				return [200, { comments: [] }];
			});
			const wrapper = mount(<CommentList {...fakeProps} />);
			expect(wrapper.state().comments.length).toEqual(0);
			wrapper.instance().newComment({});
			expect(wrapper.state().comments.length).toEqual(1);
			expect(wrapper.state().comments[0].commentId).toEqual(-1);
		});

		it("2 new comments should decrement the negative commentId without conflicting", () => {
			mock.reset();
			mock.onAny().reply(() => {
				return [200, { comments: [] }];
			});
			const wrapper = mount(<CommentList {...fakeProps} />);
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
			const wrapper = mount(<CommentList {...fakeProps} />);
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

			const wrapper = mount(<CommentList {...fakeProps} />);
			wrapper.instance().saveCommentHandler(new Event("click"), commentToInsert);
		});

		it("delete handler called with negative number removes item from array", async () => {
			mock.reset();
			mock
				.onGet(
					generateUrl("comments", undefined, [], {
						sourceURI: fakeProps.match.url
					})
				)
				.reply(200, sampleComments);

			const wrapper = mount(<CommentList {...fakeProps} />);

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

			expect(wrapper.state().comments.length).toEqual(5);
		});

		it("delete handler called with positive number hits the correct delete endpoint", async () => {
			mock.reset();
			mock
				.onGet(
					generateUrl("comments", undefined, [], {
						sourceURI: fakeProps.match.url
					})
				)
				.reply(200, sampleComments);
			mock
				.onDelete(
					generateUrl("editcomment", undefined, [1004])
				)
				.reply(config => {
					expect(config.url).toEqual("/consultations/api/Comment/1004");
					return [200, {}];
				});

			const wrapper = mount(<CommentList {...fakeProps} />);
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
					generateUrl("review", undefined, [1], {})
				)
				.reply(config => {
					expect(config.url).toEqual(
						"/consultations/api/Review/1"
					);
					return [200, { comments: [] }];
				});
			mount(<CommentList {...fakeProps} isReviewPage={true}  />);
		});

		it("when mounted with review property then the review endpoint is hit", async done  => {
			 mock.reset();
			 console.log(generateUrl("review", undefined, [1], {}));
			 mock
			 	.onGet(
			 		generateUrl("review", undefined, [1], {})
			 	)
			 	.reply(config => {
			 		expect(config.url).toEqual(
			 			"/consultations/api/Review/1"
					 );
					 done();
			 		return [200, { comments: [] }];
				 });
			 mock.onAny().reply(config => {
			 	console.log(`config is: ${config}`);

			 });

			 mount(<CommentList {...fakeProps} isReviewPage={true} />);
		});

		const firstProps = {
			match: {
				url: "/1/1/introduction"
			},
			location: {
				pathname: "1/review",
				search: "?sourceURI=/consultations/1/1/introduction"
			},
			comment: {
				commentId: 1
			}				
		};

		it("componentDidUpdate filters comments for review page", async () => {
					
			mock.reset();
			mock
				.onGet(
					generateUrl("review", undefined, [1], {
					})
				)
				.reply(200, reviewComments);

			let wrapper = mount(
				<CommentList {...firstProps}  isReviewPage={true} />
			);
			
			await nextTick();
			wrapper.update();

			expect(wrapper.state().comments.length).toEqual(6);
			expect(wrapper.find("p").text()).toEqual("No comments yet"); //Fake props has sourceURI but filteredComments == 0

			wrapper.instance().filterComments("?sourceURI=/consultations/1/1/guidance");

			await nextTick();
			wrapper.update();

			expect(wrapper.state().filteredComments.length).toEqual(1);
			expect(wrapper.find("li").length).toEqual(1);
		});

		it("componentDidUpdate filters by substring comments for review page", async () => {
					
			mock.reset();
			mock
				.onGet(
					generateUrl("review", undefined, [1], {
					})
				)
				.reply(200, reviewComments);

			let wrapper = mount(
				<CommentList {...firstProps}  isReviewPage={true} />
			);
			
			await nextTick();
			wrapper.update();

			expect(wrapper.state().comments.length).toEqual(6);
			expect(wrapper.find("p").text()).toEqual("No comments yet"); //Fake props has sourceURI but filteredComments == 0

			wrapper.instance().filterComments("?sourceURI=/consultations/1/1");

			await nextTick();
			wrapper.update();

			expect(wrapper.state().filteredComments.length).toEqual(2);
			expect(wrapper.find("li").length).toEqual(2);
		});

		it("renders 'no comments yet' if URI is supplied and filteredComments array is empty", async () => {

			mock.reset();
			mock
				.onGet(
					generateUrl("review", undefined, [1], {
					})
				)
				.reply(200, reviewComments);

			let wrapper = mount(
				<CommentList {...firstProps}  isReviewPage={true} />
			);
			
			await nextTick();
			wrapper.update();

			expect(wrapper.state().comments.length).toEqual(6);
			expect(wrapper.find("p").text()).toEqual("No comments yet"); //Fake props has sourceURI but filteredComments == 0

			wrapper.instance().filterComments("?sourceURI=/consultations/1/0");

			await nextTick();
			wrapper.update();

			expect(wrapper.find("p").text()).toEqual("No comments yet");
		});

		it("list all comments if no sourceURI is given", async () => {
			const firstProps = {
				match: {
					url: "/1/1/introduction"
				},
				location: {
					pathname: "1/review",
					search: ""
				},
				comment: {
					commentId: 1
				}				
			};
			
			mock.reset();
			mock
				.onGet(
					generateUrl("review", undefined, [1], {
					})
				)
				.reply(200, reviewComments);

			let wrapper = mount(
				<CommentList {...firstProps}  isReviewPage={true} />
			);
			
			await nextTick();
			wrapper.update();

			expect(wrapper.state().comments.length).toEqual(6);

			wrapper.instance().filterComments(firstProps.location.search);

			await nextTick();
			wrapper.update();

			expect(wrapper.find("li").length).toEqual(6);
		});
	});
});
