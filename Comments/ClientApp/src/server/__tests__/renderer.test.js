import { Helmet } from "react-helmet";
import { serverRenderer } from "../renderer";

// To avoid "Error: You may ony call rewind() on the server. Call peek() to read the current state."
Helmet.canUseDOM = false;

describe("Server renderer", () => {
	describe("serverRenderer", () => {

		it("rejects promise when app rendering fails in production", (done) => {
			process.env.NODE_ENV = "production";

			serverRenderer(null).catch((e) => {
				expect(e.message).toEqual("Cannot read property 'data' of null");
				done();
			});
		});

		it("resolves promise with error component and 500 status when app rendering fails in development", (done) => {
			process.env.NODE_ENV = "development";

			serverRenderer({ data: { viewModel: 99 } }).then((result) => {
				const expectedResult = {
					html: "<div data-reactroot=\"\">error</div>",
					statusCode: 500,
					globals: {
						viewModel: 99
					}
				};
				expect(result).toEqual(expectedResult);
				done();
			});
		});


		it("returns a promise that resolves with correct html", () => {
			var result = serverRenderer({ url: "/",
				data: { originalHtml: "<div>test-html</div>" } });
			return expect(result).resolves.toHaveProperty("html", "<div>test-html</div>");
		});
	});
});
