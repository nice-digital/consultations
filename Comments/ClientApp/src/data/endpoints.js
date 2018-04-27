export const BaseUrl  = "/consultations";

export const Endpoints = {
	consultation: "/api/Consultation", // details for a single consultation
	consultations: "/api/Consultations", // list of consultations
	documents: "/api/Documents", // documents contained within consultation
	chapter: "/api/Chapter", // chapter content for a document
	comments: "/api/Comments", // list of comments for a given URI, restricted by current user
	editcomment: "/api/Comment/{0}", // edits an existing comment. id must be positive int, restricted by current user
	newcomment: "/api/Comment" // creates a new comment. body of message contains comment in json, restricted by current user
};
