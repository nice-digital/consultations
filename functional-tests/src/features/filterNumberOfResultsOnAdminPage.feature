Feature: The number or results in the list of consultations can be controlled by the user
	As a user of consultations
	We want to be able to control the number of results are displayed on each page

	Background:
		Given I open the url "admin"
		When I log into accounts with username "IDAM_EMAIL1" and password "IDAM_PASSWORD"

	Scenario: User can change the number of results displayed on each page
		Given I expect the result list count contains "Showing 1 to 25 of 227 consultations"
		When I change the number of results on the page by selecting index "1"
		Then I expect the result list count contains "Showing 1 to 50 of 227 consultations"
		When I change the number of results on the page by selecting index "2"
		Then I expect the result list count contains "Showing 1 to 227 of 227 consultations"
