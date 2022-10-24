import { PreviouslySetQuestions } from "../PreviouslySetQuestions";

const getFilteredQuestions = new PreviouslySetQuestions().getFilteredQuestions;

const previouslySetQuestions = [
	{
		questionText: "This is my first question",
		createdByRoles: ["Administrator", "SCSTeam"],
	},
	{
		questionText: "This is my second question",
		createdByRoles: ["Administrator", "MFITeam"],
	},
	{
		questionText: "This is my third question",
		createdByRoles: ["Administrator", "GBHTeam"],
	},
	{
		questionText: "This is my fourth question",
		createdByRoles: ["Administrator", "SCSTeam"],
	},
	{
		questionText: "This is my fifth question",
		createdByRoles: ["Administrator", "SCSTeam"],
	},
];

test("should not filter out any previous questions", () => {
	const result = getFilteredQuestions(previouslySetQuestions, "", false, ["Administrator", "SCSTeam"]);
	expect(result).toEqual(previouslySetQuestions);
});

test("should filter only questions matching the search string", () => {
	const result = getFilteredQuestions(previouslySetQuestions, "f", false, ["Administrator", "SCSTeam"]);
	expect(result).toEqual([
		{
			questionText: "This is my first question",
			createdByRoles: ["Administrator", "SCSTeam"],
		},
		{
			questionText: "This is my fourth question",
			createdByRoles: ["Administrator", "SCSTeam"],
		},
		{
			questionText: "This is my fifth question",
			createdByRoles: ["Administrator", "SCSTeam"],
		},
	]);
});

test("should filter only questions of the currently logged in user's role", () => {
	const result = getFilteredQuestions(previouslySetQuestions, "", true, ["MFITeam"]);
	expect(result).toEqual([
		{
			questionText: "This is my second question",
			createdByRoles: ["Administrator", "MFITeam"],
		},
	]);
});

test("should should filter based on both search string and currently logged in user's role", () => {
	const result = getFilteredQuestions(previouslySetQuestions, "f", true, ["MFITeam"]);
	expect(result).toEqual([]);
});

test("should should filter based on both search string and currently logged in user's role", () => {
	const result = getFilteredQuestions(previouslySetQuestions, "second", true, ["MFITeam"]);
	expect(result).toEqual([{
		questionText: "This is my second question",
		createdByRoles: ["Administrator", "MFITeam"],
	}]);
});
