import React from "react";

export const QuestionsFilter = props => {
	const {handleFilter, filter} = props;
	return (
		<div className="panel mb--d">
			<div className="form__group form__group--text">
				<label htmlFor="textFilter" className="form__label"><b>Filter by question text</b></label>
				<input
					value={filter.string}
					className="form__input"
					onChange={e => handleFilter(e, null)}
					id="textFilter"
					tabIndex={0}/>
			</div>
			<div role="radiogroup">
				<label htmlFor="filterByRole" className="form__label"><b>Filter by directorate</b></label>
				<div className="form__group form__group--radio form__group--inline">
					<input
						className="form__radio"
						id="filterByRole--mine"
						type="radio"
						name="filterByRole"
						checked={filter.directorate === true}
						onChange={e => handleFilter(e, true)}
						value={true}
					/>
					<label
						className="form__label form__label--radio"
						htmlFor="filterByRole--mine">
						My Directorate
					</label>
				</div>
				<div className="form__group form__group--radio form__group--inline">
					<input
						className="form__radio"
						id="filterByRole--all"
						type="radio"
						name="filterByRole"
						checked={filter.directorate === false}
						onChange={e => handleFilter(e, false)}
						value={false}
					/>
					<label
						className="form__label form__label--radio"
						htmlFor="filterByRole--all">
						All Directorates
					</label>
				</div>
			</div>
		</div>
	);
};
