// See https://medium.com/@cereallarceny/server-side-rendering-with-create-react-app-fiber-react-router-v4-helmet-redux-and-thunk-275cb25ca972
// and https://github.com/cereallarceny/cra-ssr/blob/master/server/universal.js

import { createServerRenderer } from "aspnet-prerendering";
import React from "react";
import { renderToString  } from "react-dom/server";
import { StaticRouter } from "react-router";
import { Helmet } from "react-helmet";
import * as fs from "fs";

import { processHtml } from "./html-processor";

import App from "./../App";

const IsProduction: boolean = process.env.NODE_ENV === "production";

// Returns a promise that resolves to an object containing the HTML to be rendered.
// The params contains properties e.g.
// { location: { protocol, slashes, auth, host, port, hostname, hash, search, query: {}, pathname, path, href },
// origin, url, baseUrl, absoluteUrl, domainTasks: { }, data: { originalHtml: "", ... }
// The `params.data` property contains properties set in `SupplyData` in Startup.cs.
export const serverRenderer = (params): Promise => {
	return new Promise((resolve, reject) => {

		// Context object that Routes can use to pass properties 'out'. Primarily used for status code.
		// E.g. 
		//  <Route render={({ staticContext }) => {
		//      if (staticContext) staticContext.status = 404;
		//      return null; }} />
		let context = {};

		const rootContent = renderToString(
			<StaticRouter location={params.url} context={context}>
				<App />
			</StaticRouter>
		);

		const helmet = Helmet.renderStatic();

		let html = params.data.originalHtml;

		html = processHtml(html, {
			htmlAttributes: helmet.htmlAttributes.toString(),
			bodyAttributes: helmet.bodyAttributes.toString(),
			rootContent: rootContent,
			title: helmet.title.toString(),
			metas: helmet.meta.toString(),
			links: helmet.link.toString(),
			scripts: helmet.script.toString()
		});

		resolve({ html: html, statusCode: context.status || 200 });
	});
};

// `createServerRenderer` is what the DotNetCore SpaServices requires for SSR
export default createServerRenderer(serverRenderer);
