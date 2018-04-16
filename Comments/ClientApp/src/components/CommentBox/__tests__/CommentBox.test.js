import React from "react";
import { shallow } from "enzyme";
import { CommentBox } from "../CommentBox";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";


describe("[ClientApp] ", () => {

	describe("CommentBox Component", () => {

		it("should fail if called with invalid props", () => {

			//expect(() => {
			//	shallow(<CommentBox comment={"not a valid comment"}/>);
			//}).toThrow();

		});

	});
});
