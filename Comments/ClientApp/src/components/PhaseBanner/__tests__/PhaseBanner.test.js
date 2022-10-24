import React from "react";
import { render } from "@testing-library/react";
import { PhaseBanner } from "../PhaseBanner";

test("Matches snapshot", () => {
	const FakeProps = {
		name: "My Project",
		repo: "http://mygitrepo.com",
		phase: "delta",
	};
	const {container} = render(<PhaseBanner {...FakeProps}/>);
	expect(container).toMatchSnapshot();
});
