import React, { Component } from "react";
import { withRouter } from "react-router-dom";

import preload from "./../../data/pre-loader";
import { load } from "./../../data/loader";

class WeatherForecast extends Component {
	constructor(props) {
		super(props);

		this.state = {
			forecasts: [],
			loading: true,
			date: new Date()
		};

		const preloaded = preload(this.props.staticContext, "weather");

		if (preloaded) {
			this.state = {
				forecasts: preloaded,
				loading: false,
				date: new Date()
			};
		}
	}

	componentDidMount() {
		if (this.state.forecasts.length === 0) {
			load("weather").then(response => {
				this.setState({ forecasts: response.data, loading: false, date: new Date() });
			});
		}
	}

	handleReload = () => {
		load("weather").then(response => {
			this.setState({ forecasts: response.data, loading: false, date: new Date() });
		});
	};

	static renderForecastsTable(forecasts) {
		if (!forecasts) return null;
		return (
			<table className="table">
				<thead>
					<tr>
						<th>Date</th>
						<th>Temp. (C)</th>
						<th>Temp. (F)</th>
						<th>Summary</th>
					</tr>
				</thead>
				<tbody>
					{forecasts.map(forecast => (
						<tr key={forecast.dateFormatted}>
							<td>{forecast.dateFormatted}</td>
							<td>{forecast.temperatureC}</td>
							<td>{forecast.temperatureF}</td>
							<td>{forecast.summary}</td>
						</tr>
					))}
				</tbody>
			</table>
		);
	}

	render() {
		let contents = this.state.loading ? (
			<p>
				<em>Loading...</em>
			</p>
		) : (
			WeatherForecast.renderForecastsTable(this.state.forecasts)
		);

		console.log(`WeatherForecast: Render ${this.state.forecasts.length}`);

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

export default withRouter(WeatherForecast);
