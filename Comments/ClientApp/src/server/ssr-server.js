require("@babel/register")({
  extensions: [".js", ".jsx"]
});

require("ignore-styles");

const path = require('path');
const fs = require('fs');

require('dotenv').config({
  path: process.env.DOTENV_CONFIG_PATH
    ? path.resolve(__dirname, '../../', process.env.DOTENV_CONFIG_PATH)
    : path.resolve(__dirname, '../../.env')
});

const express = require("express");

// IMPORTANT: load the named serverRenderer export, NOT the default createServerRenderer wrapper
// which is designed for ASP.NET SpaServices' callback-based calling convention.
const { serverRenderer } = require("./renderer");

// Read the built index.html to use as the HTML template for SSR
const indexHtmlPath = path.resolve(__dirname, '../../build/index.html');
let originalHtml = '';
try {
  originalHtml = fs.readFileSync(indexHtmlPath, 'utf8');
} catch (err) {
  console.error(`Warning: Could not read ${indexHtmlPath}. SSR will not have an HTML template.`, err.message);
}

const app = express();
app.use(express.json());

app.post("/render", async (req, res) => {
  try {
    // Build the params object that serverRenderer expects
    const params = {
      url: req.body.url,
      origin: req.body.origin,
      data: {
        ...req.body.data,
        originalHtml: originalHtml
      }
    };

    const result = await serverRenderer(params);
    res.json(result);
  } catch (err) {
    console.error(err);
    res.status(500).send(err.toString());
  }
});

const port = process.env.PORT || 5000;

app.listen(port, () => {
  console.log(`React SSR running on port ${port}`);
});
