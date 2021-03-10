Feature: Comment on a Sub-Section
	As a user of consultations
	We want to be able to login to make a comment
	We want to be able to comment at the Sub-section level

	Background:
		Given I open the url "158/3/guidance"
		And I refresh
		When I log into accounts with username "IDAM_EMAIL2" and password "IDAM_PASSWORD"
		Given I delete all comments on the page

	Scenario: User makes a comment at sub-section level
		Given I comment on a Sub-section
		Then I expect the comment box title contains "subsection"
		When I add the comment "This is a Sub-section comment" and submit
		Then I expect the comment box contains "This is a Sub-section comment"
		Then I expect the comment save button displays "Saved"
		Then I click delete comment
