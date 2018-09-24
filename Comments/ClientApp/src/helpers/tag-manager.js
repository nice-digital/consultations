export const tagManager = (props) => {
	if (window && window.dataLayer) {
		console.log("GTM", props);
		window.dataLayer.push(props);
	}
};
