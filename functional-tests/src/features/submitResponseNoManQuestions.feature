Feature: User unable to submit when they have not completed mandatory questions on Review page
	As a user of consultations
	I want to be able to login to make a comment
	I want to be able to comment at the Document level
	I want to be warned if I attempt to submit response without answering mandatory questions

	Background:
		Given I open the url "234/1/recommendations"
		When I log into accounts with username "ACCOUNTS_EMAIL" and password "ACCOUNTS_PASSWORD"

	Scenario: User is unable to Submit when they have not answered the default response questions
		Given I comment on a Document
		Then I expect the comment box title contains "document"
		When I add the comment "This comment" and submit
		When I navigate to the Review Page
		And I click submit my response button
		Then I expect the feedback message "You have not stated whether you are submitting the response on behalf of an organisation" to be displayed
		And I expect the feedback message "You have not disclosed whether you or the organisation you represent have links to the tobacco industry" to be displayed
		Given I delete all comments on the page
