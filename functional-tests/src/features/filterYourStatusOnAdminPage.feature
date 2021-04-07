Feature: User can filter the admin page using status filter
	As a user of consultations
	We want to be able to view consultation lists using the open and closed status filter

	Background:
		Given I open the url "admin"
		When I log into the admin page with username "IDAM_EMAIL1" and password "IDAM_PASSWORD"

	Scenario: User can view consultation lists using the team and status filter
		Given I select open and closed status filter
		Then I expect the result list count contains "Showing 1 to 25"
