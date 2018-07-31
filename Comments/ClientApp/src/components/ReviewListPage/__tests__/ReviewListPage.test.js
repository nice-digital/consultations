/* global jest */

import React from "react";
import { mount, shallow } from "enzyme";
import { MemoryRouter } from "react-router";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import toJson from "enzyme-to-json";

import { generateUrl } from "../../../data/loader";
import { nextTick, queryStringToObject } from "../../../helpers/utils";
import ReviewListPageWithRouter, {ReviewListPage} from "../ReviewListPage";
import ConsultationData from "./Consultation";
import DocumentsData from "./Documents";
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
	describe("ReviewPage Component", () => {
		const fakeProps = {
			match: {
				url: "/1/review",
				params: {
					consultationId: 1,
				}
			},
			location: {
				pathname: "/1/review",
				search: "?sourceURI=consultations%3A%2F%2F.%2Fconsultation%2F1%2Fdocument%2F1"
			}
		};

		afterEach(() => {
			mock.reset();
		});

		// it("generateDocumentList doesn't filter out documents where convertedDocument is true", async () => {

		// 	const docTypesIn = [
		// 		{ title: "first doc title", sourceURI: "first source uri", convertedDocument : true},
		// 		{ title: "second doc title", sourceURI: "second source uri", convertedDocument : true}];

		// 	const reviewPage = new ReviewListPage(fakeProps);

		// 	const returnValue = reviewPage.generateDocumentList(docTypesIn);

		// 	expect(returnValue.links.length).toEqual(2);
		// });

		// it("generateDocumentList filters out documents where convertedDocument is false", async () => {

		// 	const docTypesIn = [
		// 		{ title: "first doc title", sourceURI: "first source uri", convertedDocument : true},
		// 		{ title: "second doc title", sourceURI: "second source uri", convertedDocument : false}];

		// 	const reviewPage = new ReviewListPage(fakeProps);

		// 	const returnValue = reviewPage.generateDocumentList(docTypesIn);

		// 	expect(returnValue.links.length).toEqual(1);
		// });

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
					return [200, DocumentsData];
				});

			mock
				.onGet("/consultations/api/Consultation?consultationId=1")
				.reply(() => {
					return [200, ConsultationData];
				});
		});

		// it.only("should hit the submit endpoint successfully", async done => {

		// 	const mock = new MockAdapter(axios);

		// 	const wrapper = mount(
		// 		<MemoryRouter>
		// 			<ReviewPage {...fakeProps} />
		// 		</MemoryRouter>
		// 	);

		// 	let documentsPromise = new Promise(resolve => {
		// 		mock
		// 			.onGet("/consultations/api/Documents?consultationId=1")
		// 			.reply(() => {
		// 				resolve();
		// 				return [200, DocumentsData];
		// 			});
		// 	});

		// 	let consultationPromise = new Promise(resolve => {
		// 		mock
		// 			.onGet("/consultations/api/Consultation?consultationId=1")
		// 			.reply(() => {
		// 				resolve();
		// 				return [200, ConsultationData];
		// 			});
		// 	});

		// 	mock
		// 		.onPost("/consultations/api/Submit")
		// 		.reply(() => {
		// 			done();

		// 		});

		// 	return Promise.all([
		// 		documentsPromise,
		// 		consultationPromise
		// 	]).then(async () => {
		// 		await nextTick();
		// 		wrapper.update();

		// 		//expect(wrapper.find(ReviewPage).instance().state.isSubmitted).toEqual(false);

		// 		wrapper.find(ReviewPage).instance().submitConsultation();
		// 	});

		// });

		it("should match snapshot with supplied data", () => {
			const mock = new MockAdapter(axios);

			const wrapper = mount(
				<MemoryRouter>
					<ReviewListPage {...fakeProps} />
				</MemoryRouter>
			);

			let documentsPromise = new Promise(resolve => {
				mock
					.onGet("/consultations/api/Documents?consultationId=1")
					.reply(() => {
						resolve();
						return [200, DocumentsData];
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
				documentsPromise,
				consultationPromise
			]).then(async () => {
				await nextTick();
				wrapper.update();
				expect(
					toJson(wrapper, {
						noKey: true,
						mode: "deep"
					})
				).toMatchSnapshot();
			});
		});

	});
});
