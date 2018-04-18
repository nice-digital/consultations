import React from "react";
import {shallow} from "enzyme";
// import {MemoryRouter} from "react-router";
import {CommentBox} from "../CommentBox";
import sampleComment from "./sample";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import {nextTick} from "../../../helpers/utils";

const mock = new MockAdapter(axios);

describe("[ClientApp] ", () => {

	describe("CommentBox Component", () => {

		it("makes correct request to the api based on comment ID", (done) => {
			mock.reset();
			const fakeProps = {
				commentId: sampleComment.commentId
			};
			mock.onAny().reply(config => {
				expect(config.url).toEqual("/consultations/api/Comment/" + sampleComment.commentId);
				done();
				return [200, sampleComment];
			});

			shallow(<CommentBox {...fakeProps} />);

		});

		it("save handler posts to the api with valid message", async (done) => {
			mock.reset();
			const fakeProps = {
				commentId: sampleComment.commentId
			};
			mock.onGet("/consultations/api/Comment/" + sampleComment.commentId).reply(200, sampleComment);

			mock.onPut("/consultations/api/Comment/" + sampleComment.commentId).reply(config => {
				expect(JSON.parse(config.data)).toEqual(sampleComment);
				done();
				return [200, sampleComment];
			});

			const wrapper = shallow(<CommentBox {...fakeProps} />);
			await nextTick();
			wrapper.update();
			const commentBoxClass = wrapper.instance();
			commentBoxClass.formSubmitHandler(new Event("click"), sampleComment);
		});

	});
});
