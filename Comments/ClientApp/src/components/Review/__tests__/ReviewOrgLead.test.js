import React from "react";
import { render, screen, waitForElementToBeRemoved } from "@testing-library/react";
import { MemoryRouter } from "react-router";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { LiveAnnouncer } from "react-aria-live";
import { Review } from "../Review";
import ConsultationData from "./Consultation.json";
import CommentsReviewData from "./CommentsReview.json";

const mock = new MockAdapter(axios);

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

afterEach(() => {
	mock.reset();
});

test("should match snapshot when viewing as an organisation lead (pre-submission)", () => {
	const mock = new MockAdapter(axios);
	let commentsReviewPromise = new Promise(resolve => {
		mock
			.onGet("/consultations/api/CommentsForReview?relativeURL=%2F1%2Freview")
			.reply(() => {
				resolve();
				return [200, CommentsReviewData];
			});
	});
	let consultationPromise = new Promise(resolve => {
		mock
			.onGet("/consultations/api/Consultation?consultationId=1&isReview=true")
			.reply(() => {
				resolve();
				return [200, ConsultationData];
			});
	});
	const {container} = render(
		<MemoryRouter>
			<LiveAnnouncer>
				<Review {...fakeProps} />
			</LiveAnnouncer>
		</MemoryRouter>,
	);
	return Promise.all([
		commentsReviewPromise,
		consultationPromise,
	]).then(async () => {
		await waitForElementToBeRemoved(() => screen.getByText("Loading...", { selector: "h1" }));
		expect(container).toMatchSnapshot();
	});
});

test("should match snapshot when viewing as an organisation lead (post-submission)", () => {
	const mock = new MockAdapter(axios);
	const localConsultationData = Object.assign({},ConsultationData);
	localConsultationData.consultationState.submittedDate = "2019-07-23T13:50:40.7043147";
	let commentsReviewPromise = new Promise(resolve => {
		mock
			.onGet("/consultations/api/CommentsForReview?relativeURL=%2F1%2Freview")
			.reply(() => {
				resolve();
				return [200, CommentsReviewData];
			});
	});
	let consultationPromise = new Promise(resolve => {
		mock
			.onGet("/consultations/api/Consultation?consultationId=1&isReview=true")
			.reply(() => {
				resolve();
				return [200, localConsultationData];
			});
	});
	const {container} = render(
		<MemoryRouter>
			<LiveAnnouncer>
				<Review {...fakeProps} />
			</LiveAnnouncer>
		</MemoryRouter>,
	);
	return Promise.all([
		commentsReviewPromise,
		consultationPromise,
	]).then(async () => {
		await waitForElementToBeRemoved(() => screen.getByText("Loading...", { selector: "h1" }));
		expect(container).toMatchSnapshot();
	});
});
