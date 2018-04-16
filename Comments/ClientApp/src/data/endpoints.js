export const BaseUrl  = "/consultations";

export const Endpoints = {
	consultation: "/api/Consultation", // details for a single consultation
	consultations: "/api/Consultations", // list of consultations
	documents: "/api/Documents", // documents contained within consultation
	chapter: "/api/Chapter", // chapter content for a document
	comments: "/api/Comments", // list of comments for a given URI, restricted by current user
	comment: "/api/Comment/{0}" // gets a single comment, restricted by current user
};
