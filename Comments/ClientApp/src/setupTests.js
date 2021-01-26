import Enzyme from "enzyme";
import EnzymeAdapter from "enzyme-adapter-react-16";

// Setup enzyme's react adapter
Enzyme.configure({ adapter: new EnzymeAdapter() });

// Set up the virtual DOM cos we're using "node" for the test environment
const { JSDOM } = require("jsdom");

const jsdom = new JSDOM("<!doctype html><html><body/></html>");
const { window } = jsdom;

function copyProps(src, target) {
	const props = Object.getOwnPropertyNames(src)
		.filter(prop => typeof target[prop] === "undefined")
		.reduce((result, prop) => ({
			...result,
			[prop]: Object.getOwnPropertyDescriptor(src, prop),
		}), {});
	Object.defineProperties(target, props);
}

global.window = window;
global.document = window.document;
global.navigator = {
	userAgent: "node.js",
};

copyProps(window, global);
