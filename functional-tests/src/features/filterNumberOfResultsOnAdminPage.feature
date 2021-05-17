Feature: The number or results in the list of consultations can be controlled by the user
	As a user of consultations
	We want to be able to control the number of results are displayed on each page

	Background:
		Given I open the url "admin"
		And I refresh
		When I log into the admin page with username "IDAM_EMAIL1" and password "IDAM_PASSWORD"
		Given I open the url "admin"

	Scenario: User can change the number of results displayed on each page
		Given I expect the result list count contains "Showing 1 to 25"
		When I change the number of results on the page by selecting index "1"
		Then I expect the result list count contains "Showing 1 to 50"
		When I change the number of results on the page by selecting index "2"
		Then I expect all results are displayed
