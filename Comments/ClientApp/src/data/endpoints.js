export const BaseUrl  = "/consultations";

export const Endpoints = {
	consultation: "/api/Consultation", // details for a single consultation
	consultations: "/api/Consultations", // list of consultations
	consultationList: "/api/ConsultationList", // list of consultations for the download page
	draftconsultation: "/api/DraftConsultation", // details for a single consultation that has never been published
	documents: "/api/Documents", // documents contained within consultation
	previewdraftdocuments: "/api/PreviewDraftDocuments", // documents contained within consultation
	previewpublisheddocuments: "/api/PreviewpublishedDocuments", // documents contained within consultation
	chapter: "/api/Chapter", // chapter content for a document
	previewchapter: "/api/PreviewChapter", // chapter content for a document in preview mode
	comments: "/api/Comments", // list of comments for a given URI, restricted by current user
	commentsreview: "/api/CommentsForReview", // list of comments for a given URI, restricted by current user
	editcomment: "/api/Comment/{0}", // edits an existing comment. id must be positive int, restricted by current user
	newcomment: "/api/Comment", // creates a new comment. body of message contains comment in json, restricted by current user
	editanswer: "/api/Answer/{0}", // edits an existing answer. id must be positive int, restricted by current user
	newanswer: "/api/Answer", // creates a new answer. body of message contains answer in json, restricted by current user
	review: "/api/Review/{0}", //list of all comments for a consultation (inc Consultation, Document, Chapter and Section levels) given a URI, restricted by current user
	user: "/api/User", // get details of currently logged in user
	submit: "/api/Submit", // submit on the review page.
	submitToOrgLead: "/api/SubmitToOrgLead", // submit to lead on the review page
	exportExternal: "/api/ExportExternal/{0}", //Creates an excel spread sheet for external users of the system that contains only their responses
	logging: "/api/Logging", // sends a message to the server for logging. expects the body to be the message and the loglevel to be in the querystring eg ?logLevel=Error
	questions: "/api/Questions", // get complete page structure for the admin of questions for a given consultation. /api/Questions?consultationId=22
	question: "/api/Question/{0}", // GET PUT for individual questions
	newquestion: "/api/Question", // POST for new questions
	organisation: "/api/Organisation", //GET checks for valid collation code and consultation id in querystring. returns OrganisationCode object
	organisationsession: "/api/Organisation/CreateOrganisationUserSession", //POST. checks for valid collation code and organisation authorisation id in querystring. returns a guid if successful. error otherwise.
	checkorganisationusersession: "/api/Organisation/CheckOrganisationUserSession", //GET. checks for valid session id for a given consultation id. returns a boolean for valid or not.
};
