// See https://medium.com/@cereallarceny/server-side-rendering-with-create-react-app-fiber-react-router-v4-helmet-redux-and-thunk-275cb25ca972
// and https://github.com/cereallarceny/cra-ssr/blob/master/server/universal.js

import { createServerRenderer } from "aspnet-prerendering";
import React from "react";
import { renderToString  } from "react-dom/server";
import { StaticRouter } from "react-router";
import { Helmet } from "react-helmet";
import * as fs from "fs";
import * as path from "path";

import App from "./../src/App";

const OpeningHtmlTagRegex: RegExp = /<html(.*?[^?])?>/,
    OpeningBodyTagRegex: RegExp = /<body(.*?[^?])?>/,
    ClassAttributeRegex: RegExp = /class=\"([^"]*)\"/,
    IsProduction: boolean = process.env.NODE_ENV === "production";

// Replaces the html tag with the given html attributes
const replaceHtmlTag = (html: string, htmlAttributes): string => {
    // Always add a no-js class to support Modernizr
    let className: string = htmlAttributes.toComponent().className || "",
        attrs: string = htmlAttributes.toString().replace(ClassAttributeRegex, "");

    return html.replace(OpeningHtmlTagRegex, `<html class="no-js ${className}" ${attrs}>`);
};

// Replaces the react root div with the given content html
const replaceBodyTag = (html: string, bodyAttributes): string => {
    return html.replace(OpeningBodyTagRegex, `<body ${bodyAttributes}>`);
};

// Replaces the react root div with the given content html
const replaceRootContent = (html: string, rootContent: string): string => {
    return html.replace("<div id=\"root\"></div>", `<div id="root">${rootContent}</div>`);
};

// Replace placeholders and tokens in the static html layout file.
const prepHtml = (html: string, helmet): string => {
    return html
        .replace("<!-- title -->", helmet.title.toString())
        .replace("<!-- meta -->", helmet.meta.toString())
        .replace("<!-- link -->", helmet.link.toString())
        .replace("<!-- script -->", helmet.script.toString());
};

// Returns a promise that resolves to an object containing the HTML to be rendered.
// The params contains properties e.g.
// { location: { protocol, slashes, auth, host, port, hostname, hash, search, query: {}, pathname, path, href },
// origin, url, baseUrl, absoluteUrl, domainTasks: { }, data: { originalHtml: "", ... }
// The `params.data` property contains properties set in `SupplyData` in Startup.cs.
export default createServerRenderer(params => {
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
        html = replaceHtmlTag(html, helmet.htmlAttributes);
        html = replaceBodyTag(html, helmet.bodyAttributes);
        html = replaceRootContent(html, rootContent);
        html = prepHtml(html, helmet);

        resolve({ html: html, statusCode: context.status || 200 });

    });
});