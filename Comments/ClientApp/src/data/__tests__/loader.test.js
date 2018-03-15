import { load, generateUrl } from "../loader";
import * as api from "../loader";
import axios from "axios";
import MockAdapater from "axios-mock-adapter";



describe("[ClientApp] ", () => {
	describe("loader.js", () => {
		describe("generateUrl function", () => {

			it("should produce a string that only passes endpoint if it doesn't match a shortcut", () => {
				const url  = generateUrl(
					"myEndpoint",
					"baseUrl",
					{
						value1: "value1",
						value2: 2,
						value3: ""
					}
				);
				expect(url)
					.toEqual("myEndpoint");
			});

			it("should produce a string that ends on a query of the supplied parameters", () => {
				const url  = generateUrl(
					"chapter",
					"/testing",
					{
						value1: "value1",
						value2: 2,
						value3: ""
					}
				);
				expect(url)
					.toEqual("/testing/api/Chapter?value1=value1&value2=2&value3=");
			});

			it("should only return an appended query string if the parameters are supplied", () => {
				const url  = generateUrl(
					"chapter"
				);
				expect(url)
					.toEqual("/consultations/api/Chapter");
			});

		});

		describe("axios function should be called with the correct url", () => {

			// const mock = new MockAdapater(axios);

			const options = [
				"chapter",
				"myBaseUrl",
				{
					value1: "value1",
					value2: 2
				}
			];

			it("should work", async () => {
				// arrange

				// const thing = load(options.endpoint, options.baseUrl, options.params);

				// const spy = jest.spyOn(api, "load");
				// expect(spy).toHaveBeenCalled();
				// const spy = jest.spyOn(api, load);
				const makeRequest = await load(...options);
				// console.log(spy);
				// expect(spy).toHaveBeenCalled();
				// console.log(makeRequest);
				// act
				// assert
			});
		});
	});
});
