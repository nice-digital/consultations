import React from "react";
import { shallow, mount } from "enzyme";
// import {MemoryRouter} from "react-router";
import { CommentBox } from "../CommentBox";
import sampleComment from "./sample";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { nextTick } from "../../../helpers/utils";

const mock = new MockAdapter(axios);

describe("[ClientApp] ", () => {
	describe("CommentBox Component", () => {
		const fakeProps = {
			comment: {
				commentId: sampleComment.commentId,
				commentText: "a comment",
				lastModifiedDate: new Date("01/04/2018").toISOString()
			}
		};

		it("sets text area with comment text correctly", () => {
			const wrapper = shallow(<CommentBox {...fakeProps} />);
			expect(wrapper.find("textarea").length).toEqual(1);
			expect(wrapper.find("textarea").props().defaultValue).toEqual("a comment");
		});

		it("unsavedChanges state is updated correctly on text area change", () => {
			const wrapper = mount(<CommentBox {...fakeProps} />);
			expect(wrapper.state().unsavedChanges).toEqual(false);
			const textArea = wrapper.find("textarea");
			textArea.simulate("input", {
				target: {
					value: "an updated comment"
				}
			});
			expect(wrapper.state().comment.commentText).toEqual("an updated comment");
			expect(wrapper.state().unsavedChanges).toEqual(true);
		});

		it("should update UnsavedChanges if lastupdateddate has changed", () => {
			const wrapper = mount(<CommentBox {...fakeProps} />);
			wrapper.setState({unsavedChanges: true});
			const updatedProps = {
				comment: {
					commentId: sampleComment.commentId,
					commentText: "an updated comment",
					lastModifiedDate: new Date("02/04/2018").toISOString()
				}
			};
			wrapper.setProps(updatedProps);
			expect(wrapper.state().unsavedChanges).toEqual(false);
		});

		it("should not update UnsavedChanges if lastupdateddate has not changed", () => {
			const wrapper = mount(<CommentBox {...fakeProps} />);
			wrapper.setState({unsavedChanges: true});
			wrapper.setProps(fakeProps);
			expect(wrapper.state().unsavedChanges).toEqual(true);
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
	});
});
