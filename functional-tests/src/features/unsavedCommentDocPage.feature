Feature: User is alerted when there are unsaved comments on Document page
	As a user of consultations
	We want to be able to login to make a comment
	We want to be able to comment at the Document level
	We want to be warned if I attempt to navigate away while having unsaved Comments

	Background:
		Given I open the url "233/1/recommendations"
		When I log into accounts with username "IDAM_EMAIL15" and password "IDAM_PASSWORD"

	Scenario: User is prompted to Save unsaved Comments
		Given I comment on a Document
		Then I expect the comment box title contains "document"
		When I add the comment "This comment must be saved"
		When I click on the Review Page link
		Then I expect that a alertbox contains the text "You have 1 unsaved change. Continue without saving?"
		When I dismiss the confirmbox
		Given I comment on a Document again
		When I add the comment "This comment must also be saved" to the first in the list
		When I click on the Review Page link
		Then I expect that a alertbox contains the text "You have 2 unsaved changes. Continue without saving?"




