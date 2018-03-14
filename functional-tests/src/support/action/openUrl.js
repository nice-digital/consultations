/**
 * Open the given URL
 * @param  {String}   page The URL to navigate to
 */
module.exports = (page) => {
    browser.url(page || "");
};