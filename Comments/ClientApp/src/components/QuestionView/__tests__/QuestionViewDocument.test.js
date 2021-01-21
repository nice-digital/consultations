/* global jest */

import { createQuestionPdf , createDocumentDefinition } from "../QuestionViewDocument";
import questionsData from "./questionsData.json";

jest.mock("pdfmake/build/pdfmake", ()=> {
	return {
		createPdf: jest.fn(() => {
			return {
				open: jest.fn(),
			};
		}),
	};
});
import pdfMake from "pdfmake/build/pdfmake";

describe("[ClientApp] ", () => {
	describe("Question View Document", () => {

		const questionsForPDF = questionsData.consultationQuestions;
		const titleForPDF = questionsData.consultationTitle;
		const endDate = questionsData.consultationState.endDate;

		it("createPdf should be called", () => {
			//act
			createQuestionPdf(questionsForPDF, titleForPDF, endDate);
			//assert
			expect(pdfMake.createPdf).toHaveBeenCalled();
		});

		it("document should be correctly formatted", () => {
			const definition = createDocumentDefinition(questionsForPDF, titleForPDF, endDate);

			expect(definition).toMatchSnapshot();
		});

	});
});
