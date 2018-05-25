Feature: Consultations homepage feature
  As a user of consultations
  I want to be able to use navigation

  #Background:
    #Given I open the page ""

  #Scenario: Load home page
    #When I click on text "Document View" in ".stacked-nav"
    #Then I expect that element "h1" matches the text "For consultation comments"

  Background:
    Given I open the url "1/1/introduction"
    And I refresh

  Scenario: Load home page
    #When I click on the link "Patient-centred care"
    When I wait on element ".page-header" to exist
    Then I expect that element "#root > div > div > div > main > div.page-header > h1" matches the text "Unstable angina and NSTEMI: early management"
    And I expect that element ".page-header h2" matches the text "Unstable angina and NSTEMI"

  Scenario: Comment Panel can be rolled down
    When I wait on element ".page-header" to exist
    Given I expect that element "body #sidebar-panel" is not within the viewport
    When I click on the element "body #js-drawer-toggleopen"
    Then I expect that element "body #sidebar-panel" is visible



