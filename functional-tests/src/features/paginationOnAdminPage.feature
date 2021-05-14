Feature: Users can page through the list of consultations
	As a user of consultations
	We want to be able to page through the list of consultations

	Background:
		Given I open the url "admin"
		And I refresh
		When I log into the admin page with username "IDAM_EMAIL1" and password "IDAM_PASSWORD"

	Scenario: User can page through the list of consultations
		Given I expect the result list count contains "Showing 1 to 25"
		Then I expect the first pagination option is "1"
		When I click the second pagination option
		Then I expect the first pagination option is "Previous"
		When I click the next pagination option
		And I click the previous pagination option
		And I click the previous pagination option
		Then I expect the first pagination option is "1"

# When I click on the cancel filter
# Then I expect the result list count contains "Showing 1 to 25"
