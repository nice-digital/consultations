// @flow

import React, {Fragment} from "react";
import { Pager } from "../Pager/Pager";

type PaginationProps = {
	onChangePage: Function,
	itemsPerPage: number,
	consultationCount: number,
	currentPage: number
};

export const Pagination = (props: PaginationProps) => {
	let generatePageList = (pageCount, currentPage) => {
		let pageListArray = [{pageNumber: currentPage - 1, active: false}, {pageNumber: currentPage, active: true}, {pageNumber: currentPage + 1, active: false}];

		if (currentPage === 1) {
			pageListArray = [{pageNumber: currentPage, active: true}, {pageNumber: currentPage + 1, active: false}, {pageNumber: currentPage + 2, active: false}];
		}

		if (currentPage === pageCount) {
			pageListArray = [{pageNumber: currentPage - 2, active: false}, {pageNumber: currentPage - 1, active: false}, {pageNumber: currentPage, active: true}];
		}

		return pageListArray;
	};

	const {
		onChangePage,
		itemsPerPage,
		consultationCount,
		currentPage,
	} = props;

	const paginationNeeded = consultationCount > itemsPerPage,
		pageCount = Math.ceil(consultationCount / itemsPerPage);

	// should this set empty or function
	const pageListArray = generatePageList(pageCount, currentPage);

	return (
		<nav>
			{paginationNeeded &&
				<ul className="pagination">
					{currentPage > 1 &&
						<Pager active={false} label="previous" onChangePage={onChangePage} />
					}

					{pageListArray.map((page, index) => {
						let showFirstLink = (index === 0) && (page.pageNumber > 1),
							showLastLink = (index === (pageListArray.length - 1)) && (page.pageNumber < pageCount);

						return (
							<Fragment key={page.pageNumber}>
								{showFirstLink &&
									<Pager active={false} label="1" type="first" onChangePage={onChangePage} />
								}

								<Pager active={page.active} label={page.pageNumber} type="normal" onChangePage={onChangePage} />

								{showLastLink &&
									<Pager active={false} label={pageCount} type="last" onChangePage={onChangePage} />
								}
							</Fragment>
						);
					})}

					{currentPage < pageCount &&
						<Pager active={false} label="next" type="normal" onChangePage={onChangePage} />
					}
				</ul>
			}
		</nav>
	);
};
