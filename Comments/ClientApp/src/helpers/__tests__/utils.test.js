import { objectToQueryString, nextTick, replaceFormat, isExternalLink } from "../utils";

describe("[ClientApp] ", () => {
	describe("Utils ", () => {
		describe("objectToQueryString", () => {
			it("should take an object and create a query string", () => {
				expect(objectToQueryString({
					value1: "aValue",
					value2: "anotherValue"
				})).toEqual("?value1=aValue&value2=anotherValue");
				expect(objectToQueryString({
					value1: "aValue"
				})).toEqual("?value1=aValue");
				expect(objectToQueryString()).toEqual("");
			});
		});

		describe("nextTick", () => {
			it("should return a resolved promise object", (done) => {
				expect(nextTick().then( () => done() ).catch( () => done.fail()));
			});
		});

		describe("replaceFormat", () => {
			const stringWithNoReplaceableValues = "/consultations/api/comment";
			const stringWithOneReplaceableValue = "/consultations/api/comment/{0}";
			const stringWithTwoReplaceableValues = "/consultations/api/comment/{0}/{1}";

			it("should return string unchanged with null passed", () => {
				expect(replaceFormat(stringWithNoReplaceableValues, null)).toEqual(stringWithNoReplaceableValues);
			});
			it("should return string unchanged with undefined passed", () => {
				expect(replaceFormat(stringWithNoReplaceableValues, undefined)).toEqual(stringWithNoReplaceableValues);
			});
			it("should return string unchanged with object passed", () => {
				expect(replaceFormat(stringWithNoReplaceableValues, {notvalid: true})).toEqual(stringWithNoReplaceableValues);
			});
			it("should return replaced string with one value passed", () => {
				expect(replaceFormat(stringWithOneReplaceableValue, [100])).toEqual("/consultations/api/comment/100");
			});
			it("should return replaced string with two values passed", () => {
				expect(replaceFormat(stringWithTwoReplaceableValues, [1001, 2002])).toEqual("/consultations/api/comment/1001/2002");
			});
		});

		describe("isExternalLink", () => {
			it("should return true if the string supplied begins with 'http'", () => {
				expect(isExternalLink("/an/internal/link")).toEqual(false);
				expect(isExternalLink("http://external.com")).toEqual(true);
				expect(isExternalLink("https://external.com")).toEqual(true);
				expect(isExternalLink("www.http.horriblelink.com")).toEqual(false);
			});
		});
	});
});
