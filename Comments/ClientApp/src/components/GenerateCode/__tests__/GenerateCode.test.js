/* eslint-env jest */
import React from "react";
import { mount, shallow } from "enzyme";
import toJson from "enzyme-to-json";
import { Button } from "@nice-digital/nds-button";

import { GenerateCode } from "../GenerateCode";

describe("GenerateCode", () => {

	const fakePropsNoCode = {
		organisationCodes: [{
			canGenerateCollationCode: true,
			collationCode: null,
			organisationAuthorisationId: 0,
			organisationId: 999,
			organisationName: "Super Magic Club"
		}],
		consultationId: 111
	};

	let fakePropsCode = {
		organisationCodes: [{...fakePropsNoCode.organisationCodes[0]}],
		consultationId: 111
	};

	fakePropsCode.organisationCodes[0].canGenerateCollationCode = false;
	fakePropsCode.organisationCodes[0].collationCode = "1234 5678 9123";

	const shareOrganisationSelector = `button#share-organisation-${fakePropsNoCode.consultationId}`,
		organisationCodesSelector = `#organisation-codes-${fakePropsNoCode.consultationId}`;

	it("should match snapshot with no code having been generated", () => {
		const wrapper = mount(<GenerateCode {...fakePropsNoCode} />);
		wrapper.find(shareOrganisationSelector).simulate("click");
		expect(toJson(wrapper, { noKey: true, mode: "deep" })).toMatchSnapshot();
	});

	it("should match snapshot with code having been generated", () => {
		const wrapper = mount(<GenerateCode {...fakePropsCode} />);
		wrapper.find(shareOrganisationSelector).simulate("click");
		expect(toJson(wrapper, { noKey: true, mode: "deep" })).toMatchSnapshot();
	});

	it("shouldn't show the panel when first loaded", () => {
		const wrapper = shallow(<GenerateCode {...fakePropsNoCode} />);
		expect(wrapper.find(organisationCodesSelector).exists()).toBe(false);
	});

	it("should show panel when share button is clicked", () => {
		const wrapper = shallow(<GenerateCode {...fakePropsNoCode} />);
		wrapper.find(Button).simulate("click");
		expect(wrapper.find(organisationCodesSelector).exists()).toBe(true);
	});

});
