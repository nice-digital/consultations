import React, { Component } from 'react';
import { withRouter } from 'react-router';

import preload from "./../../data/pre-loader";
import load from "./../../data/loader";

export class FetchData extends Component {
	displayName = FetchData.name

	constructor(props) {
		super(props);

		this.state = { forecasts: [], loading: true, date: new Date() };

		const preloaded = preload(this.props.staticContext, "weather");

		if (preloaded) {
			this.state = { forecasts: preloaded, loading: false, date: new Date() };
		}

		this.handleReload = this.handleReload.bind(this);
	}

	componentDidMount() {
		if (this.state.forecasts.length === 0) {
			load("weather")
				.then((data) => {
					this.setState({ forecasts: data, loading: false, date: new Date() });
				});
		}
	}

	handleReload() {
		load("weather")
			.then((data) => {
				this.setState({ forecasts: data, loading: false, date: new Date() });
			});
	}

	static renderForecastsTable(forecasts) {
		return (
			<table className='table'>
				<thead>
				<tr>
					<th>Date</th>
					<th>Temp. (C)</th>
					<th>Temp. (F)</th>
					<th>Summary</th>
				</tr>
				</thead>
				<tbody>
				{forecasts.map(forecast =>
					<tr key={forecast.dateFormatted}>
						<td>{forecast.dateFormatted}</td>
						<td>{forecast.temperatureC}</td>
						<td>{forecast.temperatureF}</td>
						<td>{forecast.summary}</td>
					</tr>
				)}
				</tbody>
			</table>
		);
	}

	render() {
		let contents = this.state.loading
			? <p><em>Loading...</em></p>
			: FetchData.renderForecastsTable(this.state.forecasts);

		console.log(`FetchData: Render ${this.state.forecasts.length}`);

		return (
			<div>
				<h1>Weather forecast</h1>
				<p>{this.state.date.toString()}</p>
				<p>This component demonstrates fetching data from the server.</p>
				{contents}
				<button onClick={this.handleReload}>Reload</button>
			</div>
		);
	}
}

export default withRouter(FetchData);
