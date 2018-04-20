import React from "react";
import { shallow, mount } from "enzyme";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import { MemoryRouter } from "react-router";

import { Document } from "../Document";
import ChapterData from "./Chapter";
import ConsultationData from "./Consultation";
import DocumentsData from "./Documents";
import { nextTick } from "../../../helpers/utils";
import toJson from "enzyme-to-json";

// import { generateUrl } from "./../../../data/loader";

describe("[ClientApp] ", () => {

	describe("Document View Component", () => {


		it("newComment handler is called when click li", () => {

		});

	});
});
