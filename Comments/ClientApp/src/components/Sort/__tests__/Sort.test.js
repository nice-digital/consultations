import React from "react";
import { render  } from "@testing-library/react";
import { Sort } from "./../Sort";

test("should match the snapshot with supplied data", () => {
	const {container} = render(<Sort sortOrder="TestOrder" path="test-path" />);
	expect(container).toMatchSnapshot();
});
