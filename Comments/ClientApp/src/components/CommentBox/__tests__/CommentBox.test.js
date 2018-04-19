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
			const wrapper = shallow(<CommentBox {...fakeProps} />);
			expect(wrapper.find("textarea").length).toEqual(1);
			expect(wrapper.find("textarea").props().value).toEqual("a comment");

		});

		it("unsavedChanges state is updated correctly on text area change", () => {
			const wrapper = mount(<CommentBox {...fakeProps} />);
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

		it("marks state as unchanged after props are updated", () => {
			const wrapper = mount(<CommentBox {...fakeProps} />);
			wrapper.setState({ui: {unsavedChanges: true}});
			wrapper.setProps({});
			expect(wrapper.state().ui.unsavedChanges).toEqual(false);
		});

		it("updated comment text in state after new props received", () => {
			const wrapper = mount(<CommentBox {...fakeProps} />);
			const updatedProps = {
				comment: {
					commentId: sampleComment.commentId,
					commentText: "an updated comment"
				}
			};
			wrapper.setProps(updatedProps);
			expect(wrapper.state().comment.commentText).toEqual("an updated comment");
		});

		// it.only("updated comment text in state after comment is saved", () => {
		// 	const fakeProps = {
		// 		comment: {
		// 			commentId: 0,
		// 			commentText: "a comment"
		// 		}
		// 	};
		//
		// 	const wrapper = mount(<CommentBox {...fakeProps} />);
		// 	var updatedProps = {
		// 		comment: {
		// 			commentId: sampleComments.commentId,
		// 			commentText: "an updated comment"
		// 		}
		// 	};
		// 	wrapper.setProps(updatedProps);
		// 	expect(wrapper.state().comment.commentText).toEqual("an updated comment");
		// });




	});
});
