/* global jest */

import React from "react";
import { mount, shallow } from "enzyme";
import { MemoryRouter } from "react-router-dom";
import { CommentList } from "../CommentList";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { LiveAnnouncer } from "react-aria-live";
import toJson from "enzyme-to-json";

import { generateUrl } from "../../../data/loader";
import sampleComments from "./sampleComments.json";
import sampleQuestions from "./sampleQuestions.json";
import sampleQuestionsWithNoAnswers from "./sampleQuestionsWithNoAnswers.json";
import sampleConsultation from "./sampleconsultation.json";
import emptyCommentsResponse from "./EmptyCommentsResponse.json";
import { nextTick } from "../../../helpers/utils";

import { createQuestionPdf } from "../../QuestionView/QuestionViewDocument";
import { UserContext } from "../../../context/UserContext";

const mock = new MockAdapter(axios);

jest.mock("../../QuestionView/QuestionViewDocument");

describe("[ClientApp] ", () => {
	describe("CommentList Component", () => {
		const fakeProps = {
			match: {
				url: "/1/1/introduction",
				params: {
					consultationId: 1,
					documentId: 1,
					chapterSlug: "introduction",
				},
			},
			location: {
				pathname: "",
			},
			comment: {
				commentId: 1,
			},
		};

		let contextWrapper = null;

		beforeEach(() => {
			contextWrapper = {
				wrappingComponent: UserContext.Provider,
				wrappingComponentProps: {
					value: { isAuthorised: true, isOrganisationCommenter: false },
				},
			};
		});

		afterEach(() => {
			mock.reset();
		});

		describe("Handlers", () => {
			it("save handler put's to the api with updated comment", async done => {
				const commentToUpdate = sampleComments.comments[0];

				mock.onGet(generateUrl("comments", undefined, [], { sourceURI: fakeProps.match.url }))
					.reply(200, sampleComments);

				mock.onPut("/consultations/api/Comment/" + commentToUpdate.commentId)
					.reply(config => {
						expect(JSON.parse(config.data)).toEqual(commentToUpdate);
						done();
						return [200, commentToUpdate];
					});

				const wrapper = shallow(<CommentList {...fakeProps} />);
				wrapper.instance().saveCommentHandler(new Event("click"), commentToUpdate);
			});

			it("save handler updates the correct item in the comments array once the api has returned new data", async () => {
				const commentToUpdate = sampleComments.comments[1];

				mock.onGet()
					.reply(200, sampleComments);

				mock.onPut("/consultations/api/Comment/" + commentToUpdate.commentId)
					.reply(() => {
						const sampleCommentsUpdated = {...sampleComments.comments[1]};
						sampleCommentsUpdated.commentText = "New updated text";
						return [200, sampleCommentsUpdated];
					});

				const wrapper = shallow(<CommentList {...fakeProps} />);
				wrapper.instance().saveCommentHandler(new Event("click"), commentToUpdate);

				await nextTick();
				wrapper.update();

				expect(wrapper.state().comments[1].commentText).toEqual("New updated text");
			});

			it("new comment should add an entry in the array with negative id", () => {
				mock.onGet()
					.reply(() => {
						return [200, { comments: [] }];
					});

				const wrapper = shallow(<CommentList {...fakeProps} />);

				expect(wrapper.state().comments.length).toEqual(0);

				wrapper.instance().newComment(null, {order: "0"});

				expect(wrapper.state().comments.length).toEqual(1);
				expect(wrapper.state().comments[0].commentId).toEqual(-1);
			});

			it("2 new comments should decrement the negative commentId without conflicting", () => {
				mock.onGet()
					.reply(() => {
						return [200, { comments: [] }];
					});

				const wrapper = shallow(<CommentList {...fakeProps} />);

				expect(wrapper.state().comments.length).toEqual(0);

				wrapper.instance().newComment(null, {order: "0"});
				wrapper.instance().newComment(null, {order: "0"});

				expect(wrapper.state().comments.length).toEqual(2);
				expect(wrapper.state().comments[0].commentId).toEqual(-2);
				expect(wrapper.state().comments[1].commentId).toEqual(-1);
			});

			it("where there are existing comments in the array, inserting 2 new comments should still decrement the negative commentId without conflicting", () => {
				mock.onGet(generateUrl("comments", undefined, [], {	sourceURI: fakeProps.match.url }))
					.reply(200, sampleComments);

				const wrapper = shallow(<CommentList {...fakeProps} />);

				expect(wrapper.state().comments.length).toEqual(0);

				wrapper.instance().newComment(null, {order: "0"});
				wrapper.instance().newComment(null, {order: "0"});

				expect(wrapper.state().comments.length).toEqual(2);
				expect(wrapper.state().comments[0].commentId).toEqual(-2);
				expect(wrapper.state().comments[1].commentId).toEqual(-1);
			});

			it("save handler posts to the api with new comment with correct path from generateUrl", async done => {
				const commentToInsert = {
					commentId: -1,
					commentText: "a newly created comment",
				};

				mock.onGet(generateUrl("comments", undefined, [], {	sourceURI: fakeProps.match.url }))
					.reply(200, emptyCommentsResponse);

				mock.onPost(generateUrl("newcomment"))
					.reply(config => {
						expect(JSON.parse(config.data)).toEqual(commentToInsert);
						expect(config.url).toEqual("/consultations/api/Comment");
						done();
						return [200, commentToInsert];
					});

				const wrapper = shallow(<CommentList {...fakeProps} />);
				wrapper.instance().saveCommentHandler(new Event("click"), commentToInsert);
			});

			it("delete handler called with negative number removes item from array and calls aria announcer with correct message", async () => {
				mock.onGet()
					.reply(200, sampleComments);

				fakeProps.announceAssertive = jest.fn();

				const wrapper = shallow(<CommentList {...fakeProps} />);

				await nextTick();
				wrapper.update();

				expect(wrapper.state().comments.length).toEqual(5);

				wrapper.instance().newComment(null, {
					sourceURI: "/1/1/introduction",
					commentText: "",
					order: "0",
				});

				expect(wrapper.state().comments.length).toEqual(6);

				wrapper.instance().deleteCommentHandler(new Event("click"), -1);

				expect(wrapper.state().comments.length).toEqual(5);
				expect(fakeProps.announceAssertive).toHaveBeenCalledWith("Comment deleted", expect.any(String));
			});

			it("delete handler called with positive number hits the correct delete endpoint", async () => {
				mock.onGet()
					.reply(200, sampleComments);

				mock.onDelete(generateUrl("editcomment", undefined, [1004]))
					.reply(config => {
						expect(config.url).toEqual("/consultations/api/Comment/1004");
						return [200, {}];
					});

				fakeProps.announceAssertive = jest.fn();

				const wrapper = shallow(<CommentList {...fakeProps} />);

				await nextTick();
				wrapper.update();

				expect(wrapper.state().comments.length).toEqual(5);

				wrapper.instance().deleteCommentHandler(new Event("click"), 1004);

				await nextTick();
				wrapper.update();

				expect(wrapper.state().comments.length).toEqual(4);
			});
		});

		describe("Authorisation", () => {
			it("should show login banner and not render any comment boxes without authorisation", async () => {
				mock.onGet()
					.reply(200, sampleComments);

				contextWrapper.wrappingComponentProps.value.isAuthorised = false;

				const wrapper = mount(
					<MemoryRouter>
						<LiveAnnouncer>
							<CommentList {...fakeProps} />
						</LiveAnnouncer>
					</MemoryRouter>,
					contextWrapper,
				);

				await nextTick();
				wrapper.update();

				expect(wrapper.find("LoginBanner").length).toEqual(1);
				expect(wrapper.find("CommentBox").length).toEqual(0);
			});

			it("should show 5 (current user) comment boxes with authorisation and sample data", async () => {
				mock.onGet()
					.reply(200, sampleComments);

				const wrapper = mount(
					<MemoryRouter>
						<LiveAnnouncer>
							<CommentList {...fakeProps} />
						</LiveAnnouncer>
					</MemoryRouter>,
					contextWrapper,
				);

				await nextTick();
				wrapper.update();

				expect(wrapper.find("textarea").length).toEqual(5);
			});

			it("should match snapshot post login, with 5 personal comments and 5 read only comments from others", async () => {
				mock.onGet(generateUrl("comments", undefined, [], {	sourceURI: fakeProps.match.url }))
					.reply(200, sampleComments);

				mock.onGet(generateUrl("commentsForOtherOrgCommenters", undefined, [], { sourceURI: fakeProps.match.url }))
					.reply(200, sampleComments);

				contextWrapper.wrappingComponentProps.value.isOrganisationCommenter = true;

				const wrapper = mount(
					<MemoryRouter>
						<LiveAnnouncer>
							<CommentList {...fakeProps} />
						</LiveAnnouncer>
					</MemoryRouter>,
					contextWrapper,
				);

				await nextTick();
				wrapper.update();

				expect(
					toJson(wrapper, {
						noKey: true,
						mode: "deep",
					}),
				).toMatchSnapshot();
			});
		});

		describe("Organisational Commenting", () => {
			it("should hit the commentsForOtherOrgCommenters endpoint (when using the code) and populate comment list with read only comments", async () => {
				mock.onGet(generateUrl("comments", undefined, [], {	sourceURI: fakeProps.match.url }))
					.reply(200, emptyCommentsResponse);

				mock.onGet(generateUrl("commentsForOtherOrgCommenters", undefined, [], { sourceURI: fakeProps.match.url }))
					.reply(200, sampleComments);

				contextWrapper.wrappingComponentProps.value.isOrganisationCommenter = true;

				const wrapper = mount(
					<MemoryRouter>
						<LiveAnnouncer>
							<CommentList {...fakeProps} />
						</LiveAnnouncer>
					</MemoryRouter>,
					contextWrapper,
				);

				await nextTick();
				wrapper.update();

				expect(wrapper.find("textarea").filter({ disabled: false }).length).toEqual(0);
				expect(wrapper.find("textarea").filter({ disabled: true }).length).toEqual(5);
				expect(wrapper.find("button").filter({ children: "Delete" }).length).toEqual(0);
			});

			it("should hit the commentsForOtherOrgCommenters endpoint (when org commenter) and populate question list with read only comments", async () => {
				mock.onGet(generateUrl("comments", undefined, [], {	sourceURI: fakeProps.match.url }))
					.reply(200, sampleQuestionsWithNoAnswers);

				mock.onGet(generateUrl("commentsForOtherOrgCommenters", undefined, [], { sourceURI: fakeProps.match.url }))
					.reply(200, sampleQuestions);

				contextWrapper.wrappingComponentProps.value.isOrganisationCommenter = true;

				const wrapper = mount(
					<MemoryRouter>
						<LiveAnnouncer>
							<CommentList {...fakeProps} />
						</LiveAnnouncer>
					</MemoryRouter>,
					contextWrapper,
				);

				await nextTick();
				wrapper.update();

				// new answer boxes appear as the user hasn't added any themselves
				expect(wrapper.find("textarea").filter({ disabled: false }).length).toEqual(2);

				// readonly / disabled answer boxe for three existing questions
				expect(wrapper.find("textarea").filter({ disabled: true }).length).toEqual(3);
			});
		});

		describe("API", () => {
			it("should make get api call for comments with correct path and update state with empty array", async done => {
				mock.onGet(generateUrl("comments", undefined, [], {	sourceURI: fakeProps.match.url }))
					.reply(config => {
						expect(config.url).toEqual(
							"/consultations/api/Comments?sourceURI=%2F1%2F1%2Fintroduction",
						);
						done();
						return [200, emptyCommentsResponse];
					});

				const wrapper = shallow(<CommentList {...fakeProps} />);

				expect(Array.isArray(wrapper.state().comments)).toEqual(true);
				expect(wrapper.state().comments.length).toEqual(0);
			});
		});

		describe("Download responses", () => {
			it("should call createQuestionPDF with title, end date, and questions when the download questions button is clicked", async () => {
				mock.onGet("/consultations/api/Comments?sourceURI=%2F1%2F1%2Fintroduction")
					.reply(200, sampleComments)
					.onGet("/consultations/api/Consultation?consultationId=1&isReview=false")
					.reply(200, sampleConsultation);

				const questionsForPDF = sampleComments.questions;
				const titleForPDF = sampleConsultation.title;
				const endDate = sampleComments.consultationState.endDate;
				const getTitleFunction = () => { return titleForPDF;};

				const wrapper = mount(
					<MemoryRouter>
						<LiveAnnouncer>
							<CommentList {...fakeProps} getTitleFunction={getTitleFunction} />
						</LiveAnnouncer>
					</MemoryRouter>,
				);

				await nextTick();
				wrapper.update();

				const button = wrapper.find("#js-create-question-pdf");

				button.simulate("click");

				expect(createQuestionPdf).toHaveBeenCalledTimes(1);
				expect(createQuestionPdf).toHaveBeenCalledWith(questionsForPDF, titleForPDF, endDate);
			});
		});
	});
});
