Feature: Consultation Closed for Commenting
	As a user of consultations
	We want to be able to login to be told when a COnsultation is closed for commenting
	We will not be able to comment on any part of the Consultation
	Background:
		Given I open the url "596/1/recommendations"
		When I log into accounts with username "IDAM_EMAIL2" and password "IDAM_PASSWORD"
	# Given I delete all comments on the page

	Scenario: The Consultation is closed and the user cannot make a comment
		Given I appear on a closed consultation
		And I expect the not current guidance banner to appear
		Then I expect the Comment on document button to not appear
		And I expect the Comment on Chapter bubbles to not appear
		And I expect the Comment on Section bubbles to not appear
		And I expect the Comment on Subsection bubbles to not appear

