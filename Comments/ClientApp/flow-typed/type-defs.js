declare type TopicListFilterGroupType = {
	id: string,
	title: string,
	options: TopicListFilterOptionType[]
};

declare type TopicListFilterOptionType = {
	filteredResultCount: number,
	id: string,
	isSelected: boolean,
	label: string,
	unfilteredResultCount: number
};

declare type TopicListFilterOptionType = {
	filteredResultCount: number,
	id: string,
	isSelected: boolean,
	label: string,
	unfilteredResultCount: number
};

// See https://github.com/ReactTraining/history#listening
declare type HistoryLocationType = {
	pathname: string,
	search: string,
	hash: string,
	state: ?any,
	key: ?string
};

declare type HistoryType = {
	push: (url: string) => void,
	listen: ((location: HistoryLocationType, action: "PUSH" | "REPLACE" | "POP" | null) => void) => void
};

declare type DocumentType = {
	title: string,
	sourceURI: string,
	convertedDocument: boolean
};

declare type ConsultationStateType = {
	consultationIsOpen: boolean,
	hasQuestions: boolean,
	consultationHasEnded: boolean,
	hasUserSuppliedAnswers: boolean,
	hasUserSuppliedComments: boolean
};

declare type ConsultationDataType = {
	consultationState: ConsultationStateType,
	supportsComments: boolean,
	supportsQuestions: boolean
};