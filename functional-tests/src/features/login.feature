Feature: Consultations Login
  As a user of consultations
  I want to be able to login to make a comment

Background:
    Given I open the url "1/1/introduction"
    And I refresh

Scenario: User not signed in signs in
    When I wait on element ".page-header" to exist
    When I click on the button "body #js-drawer-toggleopen"
    #When I set a cookie "__nrpa_2.2" with the content "SJQxrQET32KYHUYHY6eOSLldyy87hHkQpuxX9Sh/Z3k+V2UENlYwe4T5i5CDTf4Ltn2I35Yen+unTSHs08TXcg=="
    Then I expect that element "body #sidebar-panel" contains the text "Sign in to your NICE account to comment on this consultation. Don't have an account? Register"
		When I click on the button "body #sidebar-panel > div > div > div > div > p > a"
		And I wait on element "body #Email" to exist
		And I add "martingmeta10@gmail.com" to the inputfield "body #Email"
		And I add "abc123" to the inputfield "body #Password"
		And I click on the button "body > div.container > div > div.span6.pull-right > div > div.panel-body > form:nth-child(1) > div:nth-child(10) > div > button"
		Then I wait on element "body .page-header" for 10000ms to exist
		And I click on the element "body #js-drawer-toggleopen"
		Then I expect that element "body #sidebar-panel" contains the text "No comments yet"
