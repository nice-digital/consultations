Feature: Answer Question on a Consultation
   	As a code user of consultations
	We want to be able to login with generated organisation code to make answer a question
	We want to be able to answer a question at the consultation or document level

Background:
		Given I open the url "593/1/introduction"
		When I accept all cookies
		When I log into consultation with Organisationcode "CONSULTATIONCODE2"

Scenario: I answer a Consultation and Document level question
 		When I open question panel
		When I add the question answer "If this is a question then this is the answer" to the first in the list and submit
		Then I expect the first comment box contains "If this is a question then this is the answer"
		Then I expect the comment save button displays "Saved"
		When I add the question answer "If this is a question then this is the answer" to the second in the list and submit
		Then I expect the second comment box contains "If this is a question then this is the answer"
		Then I expect the comment save button displays "Saved"
		Then I delete all comments on the page

