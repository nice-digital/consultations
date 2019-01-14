Feature: Answer Question on a Consultation
   	As a user of consultations
  	I want to be able to login to make answer a question
 		I want to be able to answer a question at the consultation or document level

Background:
		Given I open the url "154/1/overview"
		When I log into accounts with username "ACCOUNTS_EMAIL4" and password "ACCOUNTS_PASSWORD"
		Given I open the url "admin/InsertQuestionsForDocument1And2InConsultation?consultationId=154"
		And I open the url "154/1/overview"
		When I wait on element ".page-header" to exist
 		And I pause for 1000ms

Scenario: I answer a Consultation and Document level question
 		When I wait on element "[data-qa-sel='open-questions-panel']" to be visible
		When I click on the button "[data-qa-sel='open-questions-panel']"
		Then I expect that element "[data-qa-sel='comment-panel']" is within the viewport
		When I wait on element "[data-qa-sel='Comment-text-area']" to be visible
		#And I debug
		When I add "If this is a question then this is the answer" to the inputfield ".CommentBox:first-child [data-qa-sel='Comment-text-area']"
 		And I click on the button ".CommentBox:first-child [data-qa-sel='submit-button']"
 		And I pause for 1000ms
 		Then I expect that element ".CommentBox:first-child [data-qa-sel='Comment-text-area']" contains the text "If this is a question then this is the answer"
		Then I expect that element ".CommentBox:first-child .CommentBox__savedIndicator" contains the text "Saved"
		Then I click on the button "body .CommentBox:first-child [data-qa-sel='delete-comment-button']"
		And I pause for 1000ms
		When I add "If this is a question then this is the answer" to the inputfield ".CommentBox:nth-child(2) [data-qa-sel='Comment-text-area']"
 		And I click on the button ".CommentBox:nth-child(2) [data-qa-sel='submit-button']"
 		And I pause for 1000ms
 		Then I expect that element ".CommentBox:nth-child(2) [data-qa-sel='Comment-text-area']" contains the text "If this is a question then this is the answer"
 		Then I expect that element ".CommentBox:nth-child(2) .CommentBox__savedIndicator" contains the text "Saved"
 		Then I click on the button "body .CommentBox:nth-child(2) [data-qa-sel='delete-comment-button']"

