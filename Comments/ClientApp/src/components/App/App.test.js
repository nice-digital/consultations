import React from "react";
import { shallow } from "enzyme";
import { MemoryRouter } from "react-router-dom";
import App from "./App";

describe("[ClientApp] ", () => {
	describe("App ", () => {
		it("renders without crashing", () => {
			shallow(
				<MemoryRouter>
					<App />
				</MemoryRouter>);
		});
	});
});
