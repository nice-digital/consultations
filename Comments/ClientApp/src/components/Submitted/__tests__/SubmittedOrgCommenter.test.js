/* global jest */

import React from "react";
import { mount } from "enzyme";
import { Submitted } from "../Submitted";
import MockAdapter from "axios-mock-adapter";
import axios from "axios";
import { MemoryRouter } from "react-router";
// import { LiveAnnouncer } from "react-aria-live";
import ConsultationData from "./Consultation.json";
import { nextTick } from "../../../helpers/utils";
import toJson from "enzyme-to-json";

jest.mock("../../../context/UserContext", () => {
	return {
		UserContext: {
			Consumer: (props) => {
				return props.children({
					isAuthorised: true,
					isOrganisationCommenter: true,
					organisationName: "Really Cool Org",
				});
			},
		},
	};
});

describe("[ClientApp] ", () => {
	describe("Submitted page", () => {

		const fakeProps = {
			match: {
				params: {
					consultationId: 1,
				},
			},
		};

		it("should match snapshot with supplied data", () => {
			const mock = new MockAdapter(axios);

			const wrapper = mount(
				<MemoryRouter>
					<Submitted {...fakeProps} />
				</MemoryRouter>,
			);

			let consultationPromise = new Promise(resolve => {
				mock
					.onAny()
					.reply(() => {
						resolve();
						return [200, ConsultationData];
					});
			});

			return Promise.all([
				consultationPromise,
			]).then(async () => {
				await nextTick();
				wrapper.update();

				expect(
					toJson(wrapper, {
						noKey: true,
						mode: "deep",
					})).toMatchSnapshot();
			});
		});

	});
});
