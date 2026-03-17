require("@babel/register")({
  extensions: [".js", ".jsx"]
});

require("ignore-styles");

const express = require("express");

// IMPORTANT: load index.js so Babel + ignore-styles run
const rendererModule = require("./index");
const renderer = rendererModule.default || rendererModule;

const app = express();
app.use(express.json());

app.post("/render", async (req, res) => {
  try {
    const result = await renderer(req.body);
    res.json(result);
  } catch (err) {
    console.error(err);
    res.status(500).send(err.toString());
  }
});

app.listen(4000, () => {
  console.log("React SSR running on port 4000");
});