Feature: Comment on a Consultation
  As a user of consultations
  I want to be able to login to make a comment
	I want to be able to comment at the consultation level

Background:
    Given I open the url "1/1/introduction"
    And I refresh
		And I wait on element ".page-header" to exist
		And I click on the button "body #js-drawer-toggleopen"
		When I click on the button "#sidebar-panel > div > div.panel.panel--inverse.mt--0.mb--0 > div > div > div > p > a"
		And I wait on element "body #Email" to exist
		And I add "martingmeta10@gmail.com" to the inputfield "body #Email"
		And I add "abc123" to the inputfield "body #Password"
		And I click on the button "body > div.container > div > div.span6.pull-right > div > div.panel-body > form:nth-child(1) > div:nth-child(10) > div > button"
		Then I wait on element "body .page-header" for 10000ms to exist

Scenario: User makes a comment at consultation level
    When I click on the button "#root > div > div > div > main > div.page-header > p:nth-child(1) > button"
    Then I expect that element "body #sidebar-panel" contains the text "Comment on: consultation"
		When I add "This is a Consultation comment" to the inputfield ".form__input form__input--textarea"
		And I click on the button "#sidebar-panel > div > ul > li > section > form > input"
		Then I expect that element "#sidebar-panel > div > ul > li > section > form > input" contains the text "Saved"
