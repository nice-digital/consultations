/* global jest */

import React from "react";
import { mount } from "enzyme";
import { MemoryRouter } from "react-router";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import toJson from "enzyme-to-json";

import { nextTick, queryStringToObject } from "../../../helpers/utils";
import { ReviewListPage } from "../ReviewListPage";

import ConsultationData from "./Consultation.json";
import CommentsReviewData from "./CommentsReview.json";

const mock = new MockAdapter(axios);

jest.mock("../../../context/UserContext", () => {
	return {
		UserContext: {
			Consumer: (props) => {
				return props.children({
					isAuthorised: true,
				});
			},
		},
	};
});

function mockReact() {
	const original = require.requireActual("react");
	return {
		...original,
		// Mock react's create context because Enzyme doesn't support context in mount
		createContext: jest.fn(defaultValue => {
			var value = defaultValue;
			const Provider = (props) => {
				value = props.value;
				return props.children;
			};
			const Consumer = (props) => props.children(value);

			return {
				Provider: Provider,
				Consumer: Consumer,
			};
		}),
	};
}
jest.mock("react", () => mockReact());

describe("[ClientApp] ", () => {
	describe("ReviewPage Component", () => {
		const fakeProps = {
			match: {
				url: "/1/review",
				params: {
					consultationId: 1,
				},
			},
			location: {
				pathname: "/1/review",
				search: "?sourceURI=consultations%3A%2F%2F.%2Fconsultation%2F1%2Fdocument%2F1",
			},
			history:{
				location:{
					search: "",
				},
				listen: function(){},
			},
			basename: "/consultations",
		};

		afterEach(() => {
			mock.reset();
		});

		it("queryStringToObject should return an object", async () => {
			const returnValue = queryStringToObject("?search=foo&id=bar");
			expect(returnValue.search).toEqual("foo");
			expect(returnValue.id).toEqual("bar");
		});

		it("should hit the endpoints successfully", async () => {
			const mock = new MockAdapter(axios);

			mock
				.onGet("/consultations/api/Documents?consultationId=1")
				.reply(() => {
					return [200, []];
				});

			mock
				.onGet("/consultations/api/Consultation?consultationId=1")
				.reply(() => {
					return [200, ConsultationData];
				});
		});

		it("should hit the submit endpoint successfully", async done => {

			const mock = new MockAdapter(axios);

			const wrapper = mount(
				<MemoryRouter>
					<ReviewListPage {...fakeProps} />
				</MemoryRouter>
			);

			let commentsReviewPromise = new Promise(resolve => {
				mock
					.onGet("/consultations/api/CommentsForReview?relativeURL=%2F1%2Freview")
					.reply(() => {
						resolve();
						return [200, CommentsReviewData];
					});
			});

			let consultationPromise = new Promise(resolve => {
				mock
					.onGet("/consultations/api/Consultation?consultationId=1&isReview=true")
					.reply(() => {
						resolve();
						return [200, ConsultationData];
					});
			});

			mock
				.onPost("/consultations/api/Submit")
				.reply(() => {
					done();
				});

			return Promise.all([
				commentsReviewPromise,
				consultationPromise,
			]).then(async () => {
				await nextTick();
				wrapper.update();

				wrapper.find(ReviewListPage).instance().submitConsultation();
			});

		});

		it("should match snapshot with supplied data", () => {
			const mock = new MockAdapter(axios);

			const wrapper = mount(
				<MemoryRouter>
					<ReviewListPage {...fakeProps} />
				</MemoryRouter>
			);

			let commentsReviewPromise = new Promise(resolve => {
				mock
					.onGet("/consultations/api/CommentsForReview?relativeURL=%2F1%2Freview")
					.reply(() => {
						resolve();
						return [200, CommentsReviewData];
					});
			});

			let consultationPromise = new Promise(resolve => {
				mock
					.onGet("/consultations/api/Consultation?consultationId=1&isReview=true")
					.reply(() => {
						resolve();
						return [200, ConsultationData];
					});
			});

			return Promise.all([
				commentsReviewPromise,
				consultationPromise,
			]).then(async () => {
				await nextTick();
				wrapper.update();
				expect(
					toJson(wrapper, {
						noKey: true,
						mode: "deep",
					})
				).toMatchSnapshot();
			});
		});

	});
});
