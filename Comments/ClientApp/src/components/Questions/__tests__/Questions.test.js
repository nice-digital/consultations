/* global jest */

import React from "react";
import { shallow, mount } from "enzyme";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { MemoryRouter } from "react-router";
import { nextTick } from "../../../helpers/utils";
import toJson from "enzyme-to-json";
import QuestionsData from "./QuestionsData.json";
import { Questions } from "../Questions";
import { LiveAnnouncer } from "react-aria-live";

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

describe("[ClientApp] ", () => {
	describe("Question administration Component", () => {

		const fakeProps = {
			match: {
				params: {
					consultationId: 1,
				},
			},
			location: {
				pathname: "/admin/questions/1",
			},
		};

		it("should render without crashing", async () => {
			mock
				.onGet()
				.reply(
					(config) => {
						console.log(config);
						return [200, QuestionsData];
					}
				);

			const wrapper = mount(
				<MemoryRouter>
					<Questions {...fakeProps}/>
				</MemoryRouter>
			);

			await nextTick();
			wrapper.update();

			expect(
				toJson(wrapper, {
					noKey: true,
					mode: "deep",
				})
			).toMatchSnapshot();

		});

	});
});
