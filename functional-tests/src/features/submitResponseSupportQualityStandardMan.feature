Feature: User unable to submit when they have not completed mandatory questions on Quality Standard Review page
	As a user of consultations
	We want to be able to login to make a comment
	We want to be able to comment at the Document level
	We want to be warned if I attempt to submit response without answering mandatory questions for a QS

	Background:
		Given I open the url "404/1/overview"
		When I log into accounts with username "IDAM_EMAIL1" and password "IDAM_PASSWORD"

	Scenario: User is unable to Submit when they have not answered the default response questions when answering on behalf of an Organisation on a QS Consultation
		Given I comment on a Document
		Then I expect the comment box title contains "document"
		When I add the comment "This comment" and submit
		When I navigate to the Review Page
		And I answer Yes to Organisation question and complete the organisation name
		And I click submit my response button
		Then I expect the feedback message "You have not disclosed whether your organisation would like to express an interest in formally supporting this quality standard" to be displayed
		When I scroll the delete button into view
		Given I delete all comments on the page
