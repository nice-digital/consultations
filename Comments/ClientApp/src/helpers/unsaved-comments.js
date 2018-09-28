// @flow

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
