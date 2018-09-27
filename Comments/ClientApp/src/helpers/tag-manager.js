export const tagManager = (props) => {
	if (!props) return;

	if (window && window.dataLayer) {
		window.dataLayer.push(props);
	}
};
