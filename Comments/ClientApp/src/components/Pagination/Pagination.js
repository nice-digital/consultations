// @flow

import React, { Fragment } from "react";
import { Pager } from "../Pager/Pager";

type PaginationProps = {
	onChangePage: Function,
	onChangeAmount: Function,
	itemsPerPage: number | string,
	consultationCount: number,
	currentPage: number,
};

export const Pagination = (props: PaginationProps) => {
	let generatePageList = (pageCount, currentPage) => {
		let pageListArray = [currentPage - 1, currentPage, currentPage + 1];

		if (currentPage <= 2) {
			pageListArray = [1, 2, 3, 4];
		}

		if (currentPage >= pageCount - 1) {
			pageListArray = [pageCount - 3, pageCount - 2, pageCount - 1, pageCount];
		}

		if (pageCount <= 5) {
			pageListArray = Array.from({ length: pageCount }, (x, y) => y + 1);
		}

		return pageListArray;
	};

	const {
		onChangePage,
		onChangeAmount,
		itemsPerPage,
		consultationCount,
		currentPage,
	} = props;

	const paginationNeeded = typeof itemsPerPage === "string" ? false : consultationCount > itemsPerPage,
		pageCount = Math.ceil(consultationCount / itemsPerPage);

	const pageListArray = generatePageList(pageCount, currentPage);

	return (
		<div className="flex flex--align-center">
			<div className="mr--e" data-qa-sel="number-of-results-on-page">
				<label htmlFor="itemsPerPage" className="bold mr--c">Show</label>
				<select id="itemsPerPage" name="itemsPerPage" onChange={onChangeAmount} data-qa-sel="result-on-the-page-index" value={itemsPerPage}>
					<option value="25">25</option>
					<option value="50">50</option>
					<option value="all">All</option>
				</select>
			</div>

			{paginationNeeded &&
				<nav>
					<ul className="pagination" data-qa-sel="pagination-section">
						{currentPage > 1 && (
							<Pager
								active={false}
								label="previous"
								type="normal"
								onChangePage={onChangePage}
							/>
						)}

						{pageListArray.map((page, index) => {
							let showFirstLink = index === 0 && page > 1 && pageCount > 5,
								showLastLink =
									index === pageListArray.length - 1 &&
									page < pageCount &&
									pageCount > 5;

							return (
								<Fragment key={page}>
									{showFirstLink && (
										<Pager
											active={false}
											label="1"
											type="first"
											onChangePage={onChangePage}
										/>
									)}

									<Pager
										active={page === currentPage}
										label={page}
										type="normal"
										onChangePage={onChangePage}
									/>

									{showLastLink && (
										<Pager
											active={false}
											label={pageCount}
											type="last"
											onChangePage={onChangePage}
										/>
									)}
								</Fragment>
							);
						})}

						{currentPage < pageCount && (
							<Pager
								active={false}
								label="next"
								type="normal"
								onChangePage={onChangePage}
							/>
						)}
					</ul>
				</nav>
			}
		</div>
	);
};
