/**
 * Perform an click action on the given element
 * @param  {String}   action  The action to perform (click or doubleClick)
 * @param  {String}   type    Type of the element (text or element selector)
 * @param  {String}   element Element selector, or text of the element
 * @param  {String}   ancestorSelector Ancestor selector in which to look
 */
module.exports = (action, type, element, ancestorSelector) => {

    /**
     * Element to perform the action on
     * @type {String}
     */
    const elem = (type === 'text') ? `=${element}` : element;

    /**
     * The method to call on the browser object
     * @type {String}
     */
    const method = (action === 'click') ? 'click' : 'doubleClick';

    if(ancestorSelector) {
        browser.element(ancestorSelector)[method](elem);
    } else {
        browser[method](elem);
    }
    
};