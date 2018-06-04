Feature: Consultations Login
  As a user of consultations
  I want to be able to login to make a comment

Background:
    Given I open the url "1/1/introduction"
    And I refresh

Scenario: User not signed in signs in
    When I wait on element ".page-header" to exist
    When I click on the button "body #js-drawer-toggleopen"
    Then I expect that element "body #sidebar-panel" contains the text "Sign in to your NICE account to comment on this consultation. Don't have an account?"
		When I click on the button "body .btn btn--inverse"
		And I wait on element "body .control-group" to exist
		And I add "martingmeta10@gmail.com" to the inputfield "body #Email"
		And I add "abc123" to the inputfield "body #Password"
		And I click on the button "body > div.container > div > div.span6.pull-right > div > div.panel-body > form:nth-child(1) > div:nth-child(10) > div > button"
		Then I wait on element "body .page-header" for 10000ms to exist
		And I click on the element "body #js-drawer-toggleopen"
		Then I expect that element "body #sidebar-panel" contains the text "No comments yet"

Scenario: User makes a comment at consultation level
    When I click on the button "#root > div > div > div > main > div.page-header > p:nth-child(1) > button"
    Then I expect that element "#CommentBox" contains the text "Comment on: consultation"
		When I add "This is a Consultation comment" to the inputfield ".form__input form__input--textarea"
		And I click on the button "#sidebar-panel > div > ul > li > section > form > input"
		Then I expect that element "#sidebar-panel > div > ul > li > section > form > input" contains the text "Saved"
