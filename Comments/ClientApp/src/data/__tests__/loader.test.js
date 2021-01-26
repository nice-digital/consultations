import * as loader from "../loader";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { BaseUrl } from "../endpoints";

const mock = new MockAdapter(axios);

describe("[ClientApp] ", () => {
	describe("loader.js", () => {
		describe("generateUrl function", () => {

			it("should produce a string that only passes endpoint if it doesn't match a shortcut", () => {
				const url = loader.generateUrl(
					"myEndpoint",
					"baseUrl",
					null,
					{
						value1: "value1",
						value2: 2,
						value3: "",
					},
				);
				expect(url)
					.toEqual("myEndpoint");
			});

			it("should produce a string that contains replaced value in endpoint", () => {
				const url = loader.generateUrl(
					"editcomment",
					"consultations",
					[ 1 ],
				);
				expect(url)
					.toEqual("consultations/api/Comment/1");
			});

			it("should produce a string that ends on a query of the supplied parameters", () => {
				const url  = loader.generateUrl(
					"chapter",
					"testing",
					[],
					{
						value1: "value1",
						value2: 2,
						value3: "",
					},
				);
				expect(url)
					.toEqual("testing/api/Chapter?value1=value1&value2=2&value3=");
			});

			it("should only return an appended query string if the parameters are supplied", () => {
				const url  = loader.generateUrl(
					"chapter",
				);
				expect(url)
					.toEqual(BaseUrl + "/api/Chapter");
			});
		});

		describe("load function", () => {
			const options = [
				"chapter",
				"myBaseUrl",
				[],
				{
					value1: "value1",
					value2: 2,
				},
			];

			it("axios should be called with the generated url", () => {
				expect.assertions(1);
				mock.onAny().reply(200, {});
				return loader.load(...options).then(response => {
					expect(response.config.url)
						.toBe("myBaseUrl/api/Chapter?value1=value1&value2=2");
				});
			});
		});
	});
});
