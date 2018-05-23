/* global jest */

import React from "react";
import { mount, shallow } from "enzyme";
import { MemoryRouter } from "react-router";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { generateUrl } from "../../../data/loader";
import { nextTick, queryStringToObject } from "../../../helpers/utils";
import ReviewPageWithRouter, {ReviewPage} from "../ReviewPage";

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
			location: {
				pathname: "",
				search: ""
			}
		};

		afterEach(() => {
			mock.reset();
		});

		it("generateDocumentList doesn't filter out documents supporting comments", async () => {
			
			const docTypesIn = [
				{title : "first doc title", sourceURI: "first source uri", supportsComments: true},
				{title : "second doc title", sourceURI: "second source uri", supportsComments: true}];

			const reviewPage = new ReviewPage(fakeProps);

			const returnValue = reviewPage.generateDocumentList(docTypesIn);

			expect(returnValue.links.length).toEqual(2);				
		});

		it("generateDocumentList filters out documents not supporting comments", async () => {
			
			const docTypesIn = [
				{title : "first doc title", sourceURI: "first source uri", supportsComments: true},
				{title : "second doc title", sourceURI: "second source uri", supportsComments: false}];

			const reviewPage = new ReviewPage(fakeProps);

			const returnValue = reviewPage.generateDocumentList(docTypesIn);

			expect(returnValue.links.length).toEqual(1);				
		});
	});
});
