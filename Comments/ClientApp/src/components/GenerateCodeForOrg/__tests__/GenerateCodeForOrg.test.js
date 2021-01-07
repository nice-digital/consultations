/* eslint-env jest */
import React from "react";
import { mount, shallow } from "enzyme";
import { Button } from "@nice-digital/nds-button";
import { Tag } from "@nice-digital/nds-tag";

import { GenerateCodeForOrg } from "../../GenerateCodeForOrg/GenerateCodeForOrg";

describe("GenerateCodeForOrg", () => {

	const fakePropsNoCode = {
		canGenerateCollationCode: true,
		collationCode: null,
		organisationAuthorisationId: 0,
		organisationId: 999,
		organisationName: "Super Magic Club",
		consultationId: 111,
	};

	let fakePropsCode = {...fakePropsNoCode, consultationId: 111};

	fakePropsCode.canGenerateCollationCode = false;
	fakePropsCode.collationCode = "1234 5678 9123";

	it("should show generate button when no code has been generated", () => {
		const wrapper = mount(<GenerateCodeForOrg {...fakePropsNoCode} />);
		expect(wrapper.find(Button).render().text()).toEqual("Generate");
	});

	it("should show the copy button when a code has been generated", () => {
		const wrapper = shallow(<GenerateCodeForOrg {...fakePropsCode} />);
		expect(wrapper.find('button').text()).toEqual("Copy code");
	});

	it("should show copied text after the code has been copied", () => {
		const wrapper = mount(<GenerateCodeForOrg {...fakePropsCode} />);
		const instance = wrapper.instance();
		instance.showCopiedLabel();
		expect(wrapper.find('button').text()).toEqual("Copied");
	});

});
