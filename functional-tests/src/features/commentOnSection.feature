Feature: Comment on a Section
	As a user of consultations
	We want to be able to login to make a comment
	We want to be able to comment at the Section
	Background:
		Given I open the url "406/1/information-about-risdiplam"
		When I log into accounts with username "IDAM_EMAIL2" and password "IDAM_PASSWORD"
	# Given I delete all comments on the page

	Scenario: User makes a comment at section level
		Given I comment on a Section
		Then I expect the comment box title contains "section"
		When I add the comment "This is a Section comment" and submit
		Then I expect the comment box contains "This is a Section comment"
		Then I expect the comment save button displays "Saved"
		Then I click delete comment
