import React, { Fragment } from "react";

export const SingleQuestion = (props) => {
	const {
		questionText,
		newQuestion,
		currentDocumentId,
		currentConsultationId,
		questionTypeId,
		questionType,
		allRoles,
	} = props;
	const getQuestionType = (type) => {
		return type === "Text" ? "Open Question" : "Yes/No Question";
	};
	return (
		<li>
			<div className="card">
				<div className="grid">
					<div data-g="9">
						<div className="card__header">
							<p className="card__heading">
								{questionText}
							</p>
						</div>
						<dl className="card__metadata">
							<div className="card__metadatum">
								<dt>
									<span className="card__tag tag tag--flush tag--beta">
										{getQuestionType(questionType.type)}
									</span>
								</dt>
								<dd className="visually-hidden">{questionType.type}</dd>
							</div>
							{allRoles.map(role =>
								<Fragment key={role}>
									<div className="card__metadatum">
										<dt>
											<span className="card__tag tag tag--flush tag--alpha">
												{role}
											</span>
										</dt>
										<dd className="visually-hidden">{role}</dd>
									</div>
								</Fragment>,
							)}
						</dl>
					</div>
					<div data-g="3">
						<button
							onClick={e => {
								if (currentDocumentId === "consultation") {
									newQuestion(e, currentConsultationId, null, questionTypeId, questionText);
								} else {
									newQuestion(e, currentConsultationId, parseInt(currentDocumentId, 10), questionTypeId, questionText);
								}
							}}
							className="btn btn-small right mr--0">Insert
						</button>
					</div>
				</div>
			</div>
		</li>
	);
};

