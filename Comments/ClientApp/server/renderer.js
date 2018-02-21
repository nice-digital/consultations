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

const DevScriptUrl = "/static/js/bundle.js",
    DevLayoutFolder = "public",
    ProdLayoutFolder = "build",
    IsProduction = process.env.NODE_ENV === "production";

// Returns a promise that resolves with the HTML layout file contents.
// In production, this is the HTML file generated into the *build* folder
// from create-react-app. In development mode this is the src HTML file
// from the *public* folder.
// This HTML file will need some processing e.g. adding script includes etc
function loadHtmlLayout() {
    let layoutFolder = IsProduction ? ProdLayoutFolder : DevLayoutFolder,
        fullLayoutFilePath = path.join(__dirname, "../", layoutFolder, "index.html");

    return new Promise((resolve, reject) => {
        fs.readFile(fullLayoutFilePath, "utf8", (err, data) => {
            if (err)
                reject(err);
            resolve(data);
        });
    })
}

// Replace placeholders and tokens in the static html layout file.
const prepHtml = (data, helmet, body) => {
    var fullDoc = data
        .replace(/%PUBLIC_URL%/g, "")
        .replace("<html lang=\"en\">", `<html ${helmet.htmlAttributes}`)
        .replace(/<title>(.*?)<\/title>/, helmet.title)
        .replace("</head>", `${helmet.meta}\r\n${helmet.link}</head>`)
        .replace("<div id=\"root\"></div>", `<div id="root">${body}</div>`);

    if (!IsProduction) {
        // In dev mode we run off react dev server so JS is served from that.
        // In production, the hashed URL is already in the build HTML file.
        fullDoc = fullDoc.replace("</body>", `<script type="text/javascript" src="${DevScriptUrl}"></script>\r\n</body>`);
    }

    return fullDoc;
};

export default createServerRenderer(params => {
    return new Promise((resolve, reject) => {
        loadHtmlLayout()
            .then((htmlData) => {

                let context = {};

                const body = renderToString(
                    <StaticRouter location={params.url} context={ context }>
                        <App />
                    </StaticRouter>
                );

                const helmet = Helmet.renderStatic();

                const html = prepHtml(htmlData, helmet, body);

                resolve({ html: html, statusCode: context.status || 200 });
            });
    });
});