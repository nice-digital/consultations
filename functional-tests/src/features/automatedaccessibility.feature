Feature: Automated accessibility on a Consultation
   	As a user of consultations
  	We want to be able to check consultations is accessible by keyboard and speech reader software users
 		We want to be able to check each element on the page for accessibility issues

Background:
		Given I open the url "233/1/recommendations"
		When I log into accounts with username "IDAM_EMAIL1" and password "IDAM_PASSWORD"

Scenario: User login to consultation and able to navigate, listen to each element on the page
	Then the page should have no AA accessibility issues
