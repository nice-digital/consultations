Feature: User unable to submit when they have unsaved comments on Review page
   	As a user of consultations
  	I want to be able to login to make a comment
 		I want to be able to comment at the Document level
		I want to be warned if I attempt to navigate away while having unsaved Comments

 Background:
    Given I open the url "234/1/recommendations"
		When I log into accounts with username "ACCOUNTS_EMAIL2" and password "ACCOUNTS_PASSWORD"

 Scenario: User is unable to Submit when there are unsaved comments
		Given I comment on a Document
		Then I expect the comment box title contains "document"
		When I add the comment "This comment" and submit
		When I navigate to the Review Page
		And I add the comment ". must be saved" to the first in the list on the review page
		And I complete the mandatory response submission questions
		Then I expect the Submit Response button is inactive




