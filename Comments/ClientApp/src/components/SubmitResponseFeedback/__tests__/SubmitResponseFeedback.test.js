import { SubmitResponseFeedback } from "../SubmitResponseFeedback";

describe("[ClientApp", function () {
	describe("Submit response feedback alert box", () => {

		const fakeProps = {
			validToSubmit: false,
			unsavedIdsQty: 1,
			organisationName: "",
			tobaccoDisclosure: "",
			respondingAsOrganisation: false,
			hasTobaccoLinks: false,
		};

		it("should match the snapshot when almost everything that can be wrong, is", function () {
			let props = Object.assign({}, fakeProps);
			const wrapper = SubmitResponseFeedback(props);
			expect(wrapper).toMatchSnapshot();
		});

		it("should match the snapshot when all is good except for no saved comments", function () {
			let props = {
				validToSubmit: true,
				unsavedIdsQty: 0,
				organisationName: "",
				tobaccoDisclosure: "",
				respondingAsOrganisation: "no",
				hasTobaccoLinks: "no",
			};
			const wrapper = SubmitResponseFeedback(props);
			expect(wrapper).toMatchSnapshot();
		});

		it("should match the snapshot when comments are saved", function () {
			let props = Object.assign({}, fakeProps);
			props.validToSubmit = true;
			const wrapper = SubmitResponseFeedback(props);
			expect(wrapper).toMatchSnapshot();
		});

		it("should match the snapshot when responding as organisation is yes but there is no name", function () {
			let props = Object.assign({}, fakeProps);
			props.respondingAsOrganisation = "yes";
			const wrapper = SubmitResponseFeedback(props);
			expect(wrapper).toMatchSnapshot();
		});

		it("should match the snapshot when tobacco disclosure is yes but there is no further detail", function () {
			let props = Object.assign({}, fakeProps);
			props.hasTobaccoLinks = "yes";
			const wrapper = SubmitResponseFeedback(props);
			expect(wrapper).toMatchSnapshot();
		});

		it("should match the snapshot when everything is good apart from 2 unsaved changes", function () {
			let props = {
				validToSubmit: true,
				unsavedIdsQty: 2,
				organisationName: "",
				tobaccoDisclosure: "",
				respondingAsOrganisation: true,
				hasTobaccoLinks: true,
			};
			const wrapper = SubmitResponseFeedback(props);
			expect(wrapper).toMatchSnapshot();
		});

		it("should match the snapshot when everything is good apart from 2 unsaved changes", function () {
			let props = {
				validToSubmit: true,
				unsavedIdsQty: 2,
				organisationName: "",
				tobaccoDisclosure: "",
				respondingAsOrganisation: true,
				hasTobaccoLinks: true,
			};
			const wrapper = SubmitResponseFeedback(props);
			expect(wrapper).toMatchSnapshot();
		});

		it("should return null if everything is fine", function () {
			let props = {
				validToSubmit: true,
				unsavedIdsQty: 0,
				organisationName: "Org",
				tobaccoDisclosure: "Heavy smoker",
				respondingAsOrganisation: "yes",
				hasTobaccoLinks: "yes",
			};
			const wrapper = SubmitResponseFeedback(props);
			expect(wrapper).toEqual(null);
		});

	});
});
