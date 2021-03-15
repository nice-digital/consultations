Feature: Code user unable to submit when they have not completed mandatory questions on Review page
   	As a code user of consultations
  	We want to be able to use generated code to add comment
	We want to be able to comment at the Document level
	We want to be warned if I attempt to submit response without answering mandatory questions


Background:
		Given I open the url "595/1/introduction"
		#When I accept all cookies
		When I log into consultation with Organisationcode "CONSULTATIONCODE1"

Scenario: User is unable to Submit when they have not answered the default response questions
		Given I comment on a Document
		When I add the comment "Test" and submit
		Then I expect the comment save button displays "Saved"
		When I click on the Review Page link
		When I click send your response to your organisation button
		Then I expect the feedback message "You have not entered an email address" to be displayed
		Then I enter code user emailaddress "CODE_EMAIL1"
		When I scroll the delete button into view
		Given I delete all comments on the page