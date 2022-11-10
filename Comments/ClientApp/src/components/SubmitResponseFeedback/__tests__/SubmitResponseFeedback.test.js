import { SubmitResponseFeedback } from "../SubmitResponseFeedback";
import questionsWithAnswer from "./questionsWithAnswer.json";
import questionsWithMultipleAnswers from "./questionsWithMultipleAnswers.json";

describe("[ClientApp", function () {
	describe("Submit response feedback alert box", () => {

		const fakeProps = {
			validToSubmit: false,
			unsavedIdsQty: 1,
			organisationName: "",
			tobaccoDisclosure: "",
			respondingAsOrganisation: false,
			hasTobaccoLinks: false,
			questions: questionsWithMultipleAnswers,
		};

		test("should match the snapshot when almost everything that can be wrong, is", function () {
			let props = Object.assign({}, fakeProps);
			const wrapper = SubmitResponseFeedback(props);
			expect(wrapper).toMatchSnapshot();
		});

		test("should match the snapshot when all is good except for no saved comments", function () {
			let props = {
				validToSubmit: true,
				unsavedIdsQty: 0,
				organisationName: "",
				tobaccoDisclosure: "",
				respondingAsOrganisation: "no",
				hasTobaccoLinks: "no",
				questions: questionsWithAnswer,
			};
			const wrapper = SubmitResponseFeedback(props);
			expect(wrapper).toMatchSnapshot();
		});

		test("should match the snapshot when comments are saved", function () {
			let props = Object.assign({}, fakeProps);
			props.validToSubmit = true;
			const wrapper = SubmitResponseFeedback(props);
			expect(wrapper).toMatchSnapshot();
		});

		test("should match the snapshot when responding as organisation is yes but there is no name", function () {
			let props = Object.assign({}, fakeProps);
			props.respondingAsOrganisation = "yes";
			const wrapper = SubmitResponseFeedback(props);
			expect(wrapper).toMatchSnapshot();
		});

		test("should match the snapshot when tobacco disclosure is yes but there is no further detail", function () {
			let props = Object.assign({}, fakeProps);
			props.hasTobaccoLinks = "yes";
			const wrapper = SubmitResponseFeedback(props);
			expect(wrapper).toMatchSnapshot();
		});

		test("should match the snapshot when everything is good apart from 2 unsaved changes", function () {
			let props = {
				validToSubmit: true,
				unsavedIdsQty: 2,
				organisationName: "",
				tobaccoDisclosure: "",
				respondingAsOrganisation: true,
				hasTobaccoLinks: true,
				questions: questionsWithAnswer,
			};
			const wrapper = SubmitResponseFeedback(props);
			expect(wrapper).toMatchSnapshot();
		});

		test("should match the snapshot when everything is good apart from 2 unsaved changes", function () {
			let props = {
				validToSubmit: true,
				unsavedIdsQty: 2,
				organisationName: "",
				tobaccoDisclosure: "",
				respondingAsOrganisation: true,
				hasTobaccoLinks: true,
				questions: questionsWithAnswer,
			};
			const wrapper = SubmitResponseFeedback(props);
			expect(wrapper).toMatchSnapshot();
		});

		test("should match the snapshot when everything is good apart from too many answers to one question", function () {
			let props = {
				validToSubmit: true,
				unsavedIdsQty: 0,
				organisationName: "",
				tobaccoDisclosure: "",
				respondingAsOrganisation: true,
				hasTobaccoLinks: true,
				questions: questionsWithMultipleAnswers,
			};
			const wrapper = SubmitResponseFeedback(props);
			expect(wrapper).toMatchSnapshot();
		});

		test("should return null if everything is fine", function () {
			let props = {
				validToSubmit: true,
				unsavedIdsQty: 0,
				organisationName: "Org",
				tobaccoDisclosure: "Heavy smoker",
				respondingAsOrganisation: "yes",
				hasTobaccoLinks: "yes",
				questions: questionsWithAnswer,
			};
			const wrapper = SubmitResponseFeedback(props);
			expect(wrapper).toEqual(null);
		});

	});
});
