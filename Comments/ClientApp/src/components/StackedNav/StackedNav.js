// @flow

import React, { PureComponent } from "react";
import { Link } from "react-router-dom";
import { tagManager } from "../../helpers/tag-manager";

type LinkType = {
	label: string,
	url: string,
	current?: boolean,
	isReactRoute: boolean,
	marker?: string,
};

type PropsType = {
	links: ?{
		title: string,
		links: Array<LinkType>
	},
};

export class StackedNav extends PureComponent<PropsType> {

	trackClick = (e: SyntheticEvent) => {
		tagManager({
			event: "generic",
			category: "Consultation comments page",
			action: "Opened",
			label: e.target.href,
		});
	};

	render() {
		if (!this.props.links || !this.props.links.links || this.props.links.links.length === 0) return null;
		const {title, links} = this.props.links;
		return (
			<nav className="stacked-nav" aria-label={title}>
				<h2 className="stacked-nav__root">{title}</h2>
				<ul className="stacked-nav__list">
					{links.map((item, index) => (
						<li key={index} data-qa-sel="nav-list-item" className="stacked-nav__list-item">
							{item.isReactRoute ?
								<Link
									to={item.url}
									aria-current={item.current ? "page" : null}
								>
									<span data-g="10" className="pl--0">{item.label}</span>
									{item.marker &&
										<span className="text-right" data-g="2">({item.marker})</span>
									}
								</Link>
								:
								// if !item.isReactRoute
								<a href={item.url}
									 target="_blank"
									 onClick={this.trackClick}
									 rel="noopener noreferrer">
									<span data-g="10" className="pl--0">
										{item.label}
									</span>
									</a>
							}
						</li>
					))}
				</ul>
			</nav>
		);
	}
}
