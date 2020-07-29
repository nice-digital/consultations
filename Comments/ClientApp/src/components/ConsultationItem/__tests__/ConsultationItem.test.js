/* global jest */

import React from "react";
import { mount } from "enzyme";
import { MemoryRouter } from "react-router";
import toJson from "enzyme-to-json";

import { ConsultationItem } from "../ConsultationItem";

describe("[ClientApp] ", () => {
	describe("Download Component", () => {
		const fakeProps = {
			title:"Consultation Title",
			startDate : new Date("1995-12-17T03:24:00"),
			endDate : new Date("2059-12-17T03:24:00"),
			submissionCount:1,
			consultationId :123,
			documentId:1,
			chapterSlug :"introduction",
			gidReference :"TA-123",
			productTypeName :"TA",
			isOpen: true,
			isClosed: false,
			isUpcoming: false,
			show: true,
			basename: "fdfd",
		};

		it("does not render link if document id or chapter slug is null", () => {
			window.__PRELOADED__ = { isAuthorised: true};
			const wrapper = mount(
				<MemoryRouter>
					<ConsultationItem {...fakeProps} chapterSlug={null} documentId={null} />
				</MemoryRouter>,
			);
			
			expect(wrapper.find(".card__heading a").length).toEqual(0);
		});

		it("should match snapshot with supplied data", () => {
			window.__PRELOADED__ = { isAuthorised: true};
			const wrapper = mount(
				<MemoryRouter>
					<ConsultationItem {...fakeProps} />
				</MemoryRouter>,
			);
			
			expect(
				toJson(wrapper, {
					noKey: true,
					mode: "deep",
				}),
			).toMatchSnapshot();
		});

	});
});