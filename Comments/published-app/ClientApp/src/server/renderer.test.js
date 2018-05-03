import { Helmet } from "react-helmet";
import { serverRenderer } from "./renderer";

// To avoid "Error: You may ony call rewind() on the server. Call peek() to read the current state."
Helmet.canUseDOM = false;

describe("Server renderer", () => {
	describe("serverRenderer", () => {
		it("returns a promise that resolves with correct html", () => {
			var result = serverRenderer({ url: "/",
				data: { originalHtml: "<div>test-html</div>" } });
			return expect(result).resolves.toHaveProperty("html", "<div>test-html</div>");
		});
	});
});
