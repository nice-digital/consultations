// @flow
/**
 * Maintain a list of unsaved IDs for comments and answer boxes
 * @param  {string} commentReference string Some unique identifier - in our case a123 (Answer 123) or c-1 (Comment -1)
 * @param {boolean} dirty Whether we're marking the entry as unsaved or saved
 * @param {function} self Class reference for calling setState from the originating component
 */
export const updateUnsavedIds = (commentReference: string, dirty: boolean, self: any) => {
	const unsavedIds = self.state.unsavedIds;
	if (dirty) {
		if (!unsavedIds.includes(commentReference)) {
			unsavedIds.push(commentReference);
			self.setState({
				unsavedIds,
			});
		}
	} else {
		self.setState({
			unsavedIds: unsavedIds.filter(id => id !== commentReference),
		});
	}
};
