// See https://medium.com/@cereallarceny/server-side-rendering-with-create-react-app-fiber-react-router-v4-helmet-redux-and-thunk-275cb25ca972
// and https://github.com/cereallarceny/cra-ssr/blob/master/server/universal.js

import { createServerRenderer } from "aspnet-prerendering";
import React from "react";
import { renderToString  } from "react-dom/server";
import { StaticRouter } from "react-router";
import { Helmet } from "react-helmet";

import { processHtml } from "./html-processor";

import App from "./../components/App/App";

const BaseUrlRelative: string = "/consultations";

// Returns a promise that resolves to an object containing the HTML to be rendered.
// The params contains properties e.g.
// { location: { protocol, slashes, auth, host, port, hostname, hash, search, query: {}, pathname, path, href },
// origin, url, baseUrl, absoluteUrl, domainTasks: { }, data: { originalHtml: "", ... }
// The `params.data` property contains properties set in `SupplyData` in Startup.cs.
export const serverRenderer = (params): Promise => {
	return new Promise((resolve) => {

		// Context object that Routes can use to pass properties 'out'. Primarily used for status code. E.g.:
		//  <Route render={({ staticContext }) => {
		//      if (staticContext) staticContext.status = 404;
		//      return null; }} />
		let context = {
			preload: {
				data: {}, // Key value pairs of preloaded data sets
				loaders: [] // List of promises where we track preloading data
			},
			baseUrl: params.origin + BaseUrlRelative
		};

		var app = (
			<StaticRouter basename={BaseUrlRelative} location={params.url} context={context}>
				<App />
			</StaticRouter>);

		// First render: this trigger any data preloaders to fire
		let rootContent = renderToString(app);

		// Wait for all preloaders to have loaded before re-rendering the app
		Promise.all(context.preload.loaders).then(function () {

			// Second render now that all the data preloaders have finished so we can render with data on the server
			rootContent = renderToString(app);

			const helmet = Helmet.renderStatic();

			let clientPreloadedData = `<script>window.__PRELOADED__=${JSON.stringify(context.preload.data)};</script>`;

			if (process.env.NODE_ENV === "development") {
				clientPreloadedData = `\r\n\r\n${clientPreloadedData}\r\n\r\n`;
			}

			const html = processHtml(params.data.originalHtml, {
				htmlAttributes: helmet.htmlAttributes.toString(),
				bodyAttributes: helmet.bodyAttributes.toString(),
				rootContent: rootContent,
				title: helmet.title.toString(),
				metas: helmet.meta.toString(),
				links: helmet.link.toString(),
				scripts: clientPreloadedData + helmet.script.toString()
			});

			resolve({ html: html, statusCode: context.status || 200 });
		});
	});
};

// `createServerRenderer` is what the DotNetCore SpaServices requires for SSR
export default createServerRenderer(serverRenderer);
