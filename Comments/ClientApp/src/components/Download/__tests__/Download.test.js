import React from "react";
import { render } from "@testing-library/react";
import { MemoryRouter } from "react-router";
import DownloadWithRouter from "../Download";

const fakeProps = {
	match: {
		url: "/1/review",
		params: {
			consultationId: 1,
		},
	},
	location: {
		pathname: "/1/review",
		search: "?sourceURI=consultations%3A%2F%2F.%2Fconsultation%2F1%2Fdocument%2F1",
	},
	history:{
		location:{
			search: "",
		},
		listen: function(){},
	},
	basename: "/consultations",
};

test("should match snapshot with supplied data", () => {
	window.__PRELOADED__ = { isAuthorised: true};
	const {container} = render(
		<MemoryRouter>
			<DownloadWithRouter {...fakeProps} />
		</MemoryRouter>,
	);
	expect(container).toMatchSnapshot();
});
