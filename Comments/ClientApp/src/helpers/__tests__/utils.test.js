import { objectToQueryString, nextTick, replaceFormat, isHttpLink } from "../utils";

describe("[ClientApp] ", () => {
	describe("Utils ", () => {
		describe("objectToQueryString", () => {
			test("should take an object and create a query string", () => {
				expect(objectToQueryString({
					value1: "aValue",
					value2: "anotherValue",
				})).toEqual("?value1=aValue&value2=anotherValue");
				expect(objectToQueryString({
					value1: "aValue",
				})).toEqual("?value1=aValue");
				expect(objectToQueryString()).toEqual("");
				expect(objectToQueryString({
					value1: ["first item in array", "second item in array"],
					value2: "anotherValue",
				})).toEqual("?value1=first%20item%20in%20array&value1=second%20item%20in%20array&value2=anotherValue");
			});
		});

		describe("nextTick", () => {
			test("should return a resolved promise object", (done) => {
				expect(nextTick().then( () => done() ).catch( () => done.fail()));
			});
		});

		describe("replaceFormat", () => {
			const stringWithNoReplaceableValues = "/consultations/api/comment";
			const stringWithOneReplaceableValue = "/consultations/api/comment/{0}";
			const stringWithTwoReplaceableValues = "/consultations/api/comment/{0}/{1}";

			test("should return string unchanged with null passed", () => {
				expect(replaceFormat(stringWithNoReplaceableValues, null)).toEqual(stringWithNoReplaceableValues);
			});
			test("should return string unchanged with undefined passed", () => {
				expect(replaceFormat(stringWithNoReplaceableValues, undefined)).toEqual(stringWithNoReplaceableValues);
			});
			test("should return string unchanged with object passed", () => {
				expect(replaceFormat(stringWithNoReplaceableValues, {notvalid: true})).toEqual(stringWithNoReplaceableValues);
			});
			test("should return replaced string with one value passed", () => {
				expect(replaceFormat(stringWithOneReplaceableValue, [100])).toEqual("/consultations/api/comment/100");
			});
			test("should return replaced string with two values passed", () => {
				expect(replaceFormat(stringWithTwoReplaceableValues, [1001, 2002])).toEqual("/consultations/api/comment/1001/2002");
			});
		});

		describe("isHttpLink", () => {
			test("should return true if the string supplied begins with 'http'", () => {
				expect(isHttpLink("/an/internal/link")).toEqual(false);
				expect(isHttpLink("http://external.com")).toEqual(true);
				expect(isHttpLink("https://external.com")).toEqual(true);
				expect(isHttpLink("www.http.horriblelink.com")).toEqual(false);
			});
		});
	});
});
