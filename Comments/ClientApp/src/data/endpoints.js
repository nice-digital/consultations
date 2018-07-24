export const BaseUrl  = "/consultations";

export const Endpoints = {
	consultation: "/api/Consultation", // details for a single consultation
	consultations: "/api/Consultations", // list of consultations
	documents: "/api/Documents", // documents contained within consultation
	chapter: "/api/Chapter", // chapter content for a document
	comments: "/api/Comments", // list of comments for a given URI, restricted by current user
	editcomment: "/api/Comment/{0}", // edits an existing comment. id must be positive int, restricted by current user
	newcomment: "/api/Comment", // creates a new comment. body of message contains comment in json, restricted by current user
	editanswer: "/api/Answer/{0}", // edits an existing answer. id must be positive int, restricted by current user
	newanswer: "/api/Answer", // creates a new answer. body of message contains answer in json, restricted by current user
	review: "/api/Review/{0}", //list of all comments for a consultation (inc Consultation, Document, Chapter and Section levels) given a URI, restricted by current user
	user: "/api/User", // get details of currently logged in user
	submit: "/api/Submit", // submit on the review page.
	footer: "https://alpha.nice.org.uk/Media/Default/html/HtmlWidget/Footer.html"
};
