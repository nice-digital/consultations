/* global jest */

import React from "react";
import { shallow } from "enzyme";
import { SubmittedContent } from "../SubmittedContent";
import toJson from "enzyme-to-json";

describe("[ClientApp] ", () => {
	describe("Submitted page", () => {

		const fakeProps = {
			organisationName: "My Org",
			isOrganisationCommenter: false,
			isLead: false,
			consultationState:{
				supportsDownload: true,
				leadHasBeenSentResponse: false,
			},
			consultationId: 1,
			basename: "something",
			isSubmitted: true,
			linkToReviewPage: false,
		};

        it("should match snapshot for individual commenters on the review page", () => {
			const wrapper = shallow(<SubmittedContent {...fakeProps} />);

			expect(
				toJson(wrapper, {
					noKey: true,
					mode: "deep",
				})).toMatchSnapshot();
		});

		it("should match snapshot for individual commenters on the submitted page", () => {
			const localProps = fakeProps;
			localProps.linkToReviewPage = true;
			const wrapper = shallow(<SubmittedContent {...localProps} />);

			expect(
				toJson(wrapper, {
					noKey: true,
					mode: "deep",
				})).toMatchSnapshot();
		});

		it("should match snapshot for organisation commenter on the review page", () => {
			const localProps = fakeProps;
			localProps.isOrganisationCommenter = true;
            localProps.linkToReviewPage = false;
			const wrapper = shallow(<SubmittedContent {...localProps} />);

			expect(
				toJson(wrapper, {
					noKey: true,
					mode: "deep",
				})).toMatchSnapshot();
		});

		it("should match snapshot for organisation commenter on the submitted page", () => {
			const localProps = fakeProps;
			localProps.isOrganisationCommenter = true;
			localProps.linkToReviewPage = true;
			const wrapper = shallow(<SubmittedContent {...localProps} />);

			expect(
				toJson(wrapper, {
					noKey: true,
					mode: "deep",
				})).toMatchSnapshot();
		});

		it("should match snapshot for organisation lead on the review page without any responses from their org", () => {
			const localProps = fakeProps;
			localProps.isLead = true;
            localProps.linkToReviewPage = false;
			const wrapper = shallow(<SubmittedContent {...localProps} />);

			expect(
				toJson(wrapper, {
					noKey: true,
					mode: "deep",
				})).toMatchSnapshot();
		});

		it("should match snapshot for organisation lead on the review page with responses from their org", () => {
			const localProps = fakeProps;
			localProps.isLead = true;
			localProps.consultationState.leadHasBeenSentResponse = true;
            localProps.linkToReviewPage = false;
			const wrapper = shallow(<SubmittedContent {...localProps} />);

			expect(
				toJson(wrapper, {
					noKey: true,
					mode: "deep",
				})).toMatchSnapshot();
		});

		it("should match snapshot for organisation lead on the submitted page", () => {
			const localProps = fakeProps;
			localProps.isLead = true;
			localProps.linkToReviewPage = true;
			localProps.consultationState.leadHasBeenSentResponse = true;
			const wrapper = shallow(<SubmittedContent {...localProps} />);

			expect(
				toJson(wrapper, {
					noKey: true,
					mode: "deep",
				})).toMatchSnapshot();
		});

	});
});
