Feature: Comment Ordering on the Review Page
	As a user of consultations
	We want to be able to login to make a comment
	We want to be able to comment at multiple levels
	We want them to be ordered and displayed on the review page

	Background:
		Given I open the url "1/1/introduction"
		And I refresh
		When I log into accounts with username "IDAM_EMAIL1" and password "IDAM_PASSWORD"
		Given I open the url "1/1/introduction"
		And I refresh
		Given I delete all comments on the page

	Scenario: User makes multiple comments and views them on the Review page
		Given I comment on a Document
		When I add the comment "1" to the first in the list and submit
		Then I expect the comment save button displays "Saved"
		Given I comment on a Document
		When I add the comment "2" to the first in the list and submit
		Then I expect the comment save button displays "Saved"
		Given I comment on a Document
		When I add the comment "3" to the first in the list and submit
		Then I expect the comment save button displays "Saved"
		When I navigate to the Review Page
		Then I expect the first comment box contains "3"
		And I expect the second comment box contains "2"
		And I expect the third comment box contains "1"
		Given I delete all comments on the page


