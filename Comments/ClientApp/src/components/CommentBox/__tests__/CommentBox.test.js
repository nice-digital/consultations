import React from "react";
import {shallow, mount} from "enzyme";
// import {MemoryRouter} from "react-router";
import {CommentBox} from "../CommentBox";
import sampleComment from "./sample";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import {nextTick} from "../../../helpers/utils";

const mock = new MockAdapter(axios);

describe("[ClientApp] ", () => {

	describe("CommentBox Component", () => {

		const fakeProps = {
			comment: {
				commentId: sampleComment.commentId,
				commentText: "a comment"
			}
		};

		it("sets text area with comment text correctly", () => {
			var wrapper = shallow(<CommentBox {...fakeProps} />);

			expect(wrapper.find("textarea").length).toEqual(1);
			expect(wrapper.find("textarea").props().value).toEqual("a comment");

		});

		it("unsaved changes state is updated correctly on text area change", () => {

			var wrapper = mount(<CommentBox {...fakeProps} />);

			expect(wrapper.state().ui.unsavedChanges).toEqual(false);
			const textArea = wrapper.find("textarea");
			textArea.simulate("change", {
				target: {
					value: "an updated comment"
				}
			});
			expect(wrapper.state().comment.commentText).toEqual("an updated comment");
			expect(wrapper.state().ui.unsavedChanges).toEqual(true);

		});



		it("marks state as unchanged after comment is saved", () => {
			const wrapper = mount(<CommentBox {...fakeProps} />);
			wrapper.setState({ui: {unsavedChanges: true}});
			// var updatedProps = fakeProps;
			// updatedProps.comment.commentText = "an updated comment";
			wrapper.setProps({});

			// expect(wrapper.state().comment.commentText).toEqual("an updated comment");
			expect(wrapper.state().ui.unsavedChanges).toEqual(false);
		});




		// it("save handler posts to the api with valid message", async (done) => {
		// 	mock.reset();
		// 	const fakeProps = {
		// 		comment: {
		// 			commentId: sampleComment.commentId
		// 		}
		// 	};
		// 	mock.onGet("/consultations/api/Comment/" + sampleComment.commentId).reply(200, sampleComment);
		//
		// 	mock.onPut("/consultations/api/Comment/" + sampleComment.commentId).reply(config => {
		// 		expect(JSON.parse(config.data)).toEqual(sampleComment);
		// 		done();
		// 		return [200, sampleComment];
		// 	});
		//
		// 	const wrapper = shallow(<CommentBox {...fakeProps} />);
		// 	await nextTick();
		// 	wrapper.update();
		// 	const commentBoxClass = wrapper.instance();
		// 	commentBoxClass.formSubmitHandler(new Event("click"), sampleComment);
		// });

	});
});
