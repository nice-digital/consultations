import React from "react";
import { fireEvent, render, screen, waitForElementToBeRemoved } from "@testing-library/react";
import { MemoryRouter } from "react-router";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { LiveAnnouncer } from "react-aria-live";
import { createMemoryHistory } from "history";
import { queryStringToObject } from "../../../helpers/utils";
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
				});
			},
		},
	};
});

const windowAlertReset = window.alert;

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
	basename: "/consultations",
};

beforeEach(() => {
	window.alert = windowAlertReset;
});

afterEach(() => {
	mock.reset();
});

test("queryStringToObject should return an object", async () => {
	const returnValue = queryStringToObject("?search=foo&id=bar");
	expect(returnValue.search).toEqual("foo");
	expect(returnValue.id).toEqual("bar");
});

test("should hit the endpoints successfully", async () => {
	const mock = new MockAdapter(axios);
	mock
		.onGet("/consultations/api/Documents?consultationId=1")
		.reply(() => {
			return [200, []];
		});
	mock
		.onGet("/consultations/api/Consultation?consultationId=1")
		.reply(() => {
			return [200, ConsultationData];
		});
});

test("should hit the submit endpoint successfully", async () => {
	window.alert = jest.fn();
	const mock = new MockAdapter(axios);
	const history = createMemoryHistory("/1/review");
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
	mock
		.onPost("/consultations/api/Submit")
		.reply(200, CommentsReviewData);
	mock
		.onPost("/consultations/api/Logging?logLevel=Warning")
		.reply(200);
	render(
		<MemoryRouter>
			<LiveAnnouncer>
				<Review {...fakeProps} history={history} />
			</LiveAnnouncer>
		</MemoryRouter>,
	);
	return Promise.all([
		commentsReviewPromise,
		consultationPromise,
	]).then(async () => {
		const submitConsultation = await screen.findByRole("button", { name: "Submit my response" });
		fireEvent.click(submitConsultation);
		const submitConsultationSure = await screen.findByRole("button", { name: "Yes submit my response" });
		fireEvent.click(submitConsultationSure);
		expect(window.alert).toHaveBeenCalledTimes(0);
	});
});

test("should match snapshot with supplied data pre-submission", () => {
	const mock = new MockAdapter(axios);
	const history = createMemoryHistory("/1/review");
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
				<Review {...fakeProps} history={history} />
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

test("should match snapshot with supplied data post-submission", () => {
	const mock = new MockAdapter(axios);
	const localConsultationData = Object.assign({},ConsultationData);
	localConsultationData.consultationState.submittedDate = "2019-07-23T13:50:40.7043147";
	const history = createMemoryHistory("/1/review");
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
				<Review {...fakeProps} history={history} />
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
