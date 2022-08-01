/* eslint-env jest */
import React from "react";
import { mount } from "enzyme";

import browserHistory from "../../../helpers/history";

import { HistoryContext, Provider, Consumer, withHistory } from "../HistoryContext";

function mockReact() {
	const original = jest.requireActual("react");
	return {
		...original,
		// Mock react's create context because Enzyme doesn't support context in mount
		createContext: jest.fn(defaultValue => {
			var value = defaultValue;
			const Provider = (props) => {
				value = props.value;
				return props.children;
			};
			const Consumer = (props) => props.children(value);

			return {
				Provider: Provider,
				Consumer: Consumer,
			};
		}),
	};
}
jest.mock("react", () => mockReact());

describe("[Consultations]", () => {
	describe("HistoryContext", () => {

		describe("withHistory", () => {
			it("returns function", () => {
				const wrapped = withHistory(null);
				expect(typeof wrapped).toEqual("function");
			});

			it("passes props and default browser history to wrapped component when not wrapped in provider", () => {

				const TestComponent = () => <div />;
				const WrappedTestComponent = withHistory(TestComponent);
				const historyApplied = mount(<WrappedTestComponent testProp="TestValue" />);

				const expectedProps = {
					history: browserHistory,
					testProp: "TestValue",
				};

				const actualProps = historyApplied.find(TestComponent).props();

				expect(actualProps).toEqual(expectedProps);
			});

			it("wraps component and passes provided history when wrapped in provider", () => {

				const TestComponent = () => <div />;
				const WrappedTestComponent = withHistory(TestComponent);
				const historyApplied = mount(
					<HistoryContext.Provider value={{history: "test"}}>
						<WrappedTestComponent testProp="TestValue" />
					</HistoryContext.Provider>,
				);

				const expectedProps = {
					history: "test",
					testProp: "TestValue",
				};
				const actualProps = historyApplied.find(TestComponent).props();

				expect(actualProps).toEqual(expectedProps);
			});
		});

		describe("HistoryContext", () => {
			it("is react context", () => {
				expect(HistoryContext.Provider).toBeTruthy();
				expect(HistoryContext.Consumer).toBeTruthy();
			});

			it("named provider and consumer exports are from HistoryContext", () => {
				expect(HistoryContext.Provider).toBe(Provider);
				expect(HistoryContext.Consumer).toBe(Consumer);
			});
		});

	});
});
