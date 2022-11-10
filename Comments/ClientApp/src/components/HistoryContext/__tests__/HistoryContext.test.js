import React from "react";
import { render } from "@testing-library/react";
import browserHistory from "../../../helpers/history";
import { HistoryContext, Provider, Consumer, withHistory } from "../HistoryContext";

test("returns function", () => {
	const wrapped = withHistory(null);
	expect(typeof wrapped).toEqual("function");
});

test("passes props and default browser history to wrapped component when not wrapped in provider", () => {
	const TestComponent = jest.fn(() => <div />);
	const WrappedTestComponent = withHistory(TestComponent);
	render(<WrappedTestComponent testProp="TestValue" />);
	const expectedProps = {
		history: browserHistory,
		testProp: "TestValue",
	};
	expect(TestComponent).toHaveBeenCalledWith(
		expect.objectContaining(expectedProps),
    	expect.anything(),
	);
});

test("wraps component and passes provided history when wrapped in provider", () => {
	const TestComponent = jest.fn(() => <div />);
	const WrappedTestComponent = withHistory(TestComponent);
	render(
		<HistoryContext.Provider value={{history: "test"}}>
			<WrappedTestComponent testProp="TestValue" />
		</HistoryContext.Provider>,
	);
	const expectedProps = {
		history: "test",
		testProp: "TestValue",
	};
	expect(TestComponent).toHaveBeenCalledWith(
		expect.objectContaining(expectedProps),
    	expect.anything(),
	);
});

test("is react context", () => {
	expect(HistoryContext.Provider).toBeTruthy();
	expect(HistoryContext.Consumer).toBeTruthy();
});

test("named provider and consumer exports are from HistoryContext", () => {
	expect(HistoryContext.Provider).toBe(Provider);
	expect(HistoryContext.Consumer).toBe(Consumer);
});
