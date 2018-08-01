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

			serverRenderer({ data: { viewModel: 99  } }).then((result) => {
				expect(result.statusCode).toEqual(404);
				console.log(result.html);
				var pos = result.html.search("<main role=\"main\" data-reactroot=\"\"><div class=\"container\"><div class=\"panel page-header\"><h1 class=\"heading mt--c\">Something&#x27;s gone wrong</h1><p class=\"lead\">We&#x27;ll look into it right away. Please try again in a few minutes. And if it&#x27;s still not fixed, <a href=\"/get-involved/contact-us\">contact us</a>.</p><p><a href=\"~/guidance/inconsultation\">Back to consultations</a></p><div class=\"hide\">");
				expect(pos).toEqual(0);
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
