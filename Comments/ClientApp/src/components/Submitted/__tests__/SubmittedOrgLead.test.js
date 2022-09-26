import React from "react";
import { render, waitForElementToBeRemoved, screen } from "@testing-library/react";
import MockAdapter from "axios-mock-adapter";
import axios from "axios";
import { MemoryRouter } from "react-router";
import { Submitted } from "../Submitted";
import ConsultationData from "./Consultation.json";

jest.mock("../../../context/UserContext", () => {
	return {
		UserContext: {
			Consumer: (props) => {
				return props.children({
					isAuthorised: true,
					isOrganisationCommenter: true,
					isLead: true,
					organisationName: "Really Cool Org",
				});
			},
		},
	};
});

test("should match snapshot with supplied data", () => {
	const mock = new MockAdapter(axios);
	let consultationPromise = new Promise(resolve => {
		mock
			.onAny()
			.reply(() => {
				resolve();
				return [200, ConsultationData];
			});
	});
	const fakeProps = {
		match: {
			params: {
				consultationId: 1,
			},
		},
	};
	const {container} = render(<Submitted {...fakeProps} />, {wrapper: MemoryRouter});
	return Promise.all([consultationPromise]).then(async () => {
		await waitForElementToBeRemoved(() => screen.getByText("Loading...", { selector: "h1" }));
		expect(container).toMatchSnapshot();
	});
});
