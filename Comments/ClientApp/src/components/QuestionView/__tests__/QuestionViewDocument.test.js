import { createQuestionPdf , createDocumentDefinition } from "../QuestionViewDocument";
import questionsData from "./questionsData.json";
import pdfMake from "pdfmake/build/pdfmake";

jest.mock("pdfmake/build/pdfmake", ()=> {
	return {
		createPdf: jest.fn(() => {
			return {
				open: jest.fn(),
			};
		}),
	};
});

const questionsForPDF = questionsData.consultationQuestions;
const titleForPDF = questionsData.consultationTitle;
const endDate = questionsData.consultationState.endDate;

test("createPdf should be called", () => {
	createQuestionPdf(questionsForPDF, titleForPDF, endDate);
	expect(pdfMake.createPdf).toHaveBeenCalled();
});

test("document should be correctly formatted", () => {
	const definition = createDocumentDefinition(questionsForPDF, titleForPDF, endDate);
	expect(definition).toMatchSnapshot();
});
