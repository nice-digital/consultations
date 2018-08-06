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

type StatusType = {
	statusId: number,
	name: string
};

declare type CommentType = {
	commentId: number,
	lastModifiedDate: Date,
	lastModifiedByUserId: string,
	commentText: string,
	locationId: number,
	sourceURI: string,
	htmlElementID: string,
	rangeStart: string,
	rangeStartOffset: string,
	rangeEnd: string,
	rangeEndOffset: string,
	quote: string,
	commentOn: string,
	show: boolean,
	status: StatusType
};

declare type QuestionTypeType = {
	description: string,
	hasTextAnswer: boolean,
	hasBooleanAnswer: boolean
};

declare type AnswerType = {
	answerId: number,
	answerText: string,
	answerBoolean: boolean,
	questionId: number,
	lastModifiedDate: Date,
	lastModifiedByUserId: string,
	statusId: number,
	status: StatusType,
};

declare type QuestionType = {
	questionId: number,
	questionText: string,
	questionTypeId: number,
	questionOrder: number,
	lastModifiedDate: Date,
	lastModifiedByUserId: string,
	questionType: QuestionTypeType,
	answers: Array<AnswerType>,
	show: boolean,
	commentOn: string,
	sourceURI: string,
};

declare type CommentsAndQuestionsType = {
	comments: Array<CommentType>,
	questions: Array<QuestionType>,
	isAuthorised: boolean,
	signInURL: string, 
	consultationState: ConsultationStateType,
};

declare type ReviewPageViewModelType = {
	commentsAndQuestions: CommentsAndQuestionsType,
	filters: Array<TopicListFilterGroupType>,
	//QuestionsOrComments[] and Documents[] are only for sending up to the backend. not to use in react. 
};