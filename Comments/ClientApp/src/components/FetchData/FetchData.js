import React, { Component } from "react";
import { connect } from "react-redux";
import { Helmet } from "react-helmet";

import { fetchForecastData } from "./forecastActions";

class FetchData extends Component {
	componentWillMount() {
		this.props.fetchForecastData();
	}

	renderForecastsTable = () => {
		return (
			<table className="table">
				<Helmet>
					<title>Weather Forecast</title>
				</Helmet>
				<thead>
					<tr>
						<th>Date</th>
						<th>Temp. (C)</th>
						<th>Temp. (F)</th>
						<th>Summary</th>
					</tr>
				</thead>
				<tbody>
					{this.props.forecastData.map(forecast => (
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
	};

	render() {
		const { forecastData, forecastStatus } = this.props;
		if (!forecastStatus) return null;
		if (forecastStatus === "failed")
			return (
				<div>
					<h1>Request Failed</h1>
					<pre>{forecastData.toString()}</pre>
				</div>
			);
		if (forecastStatus === "complete")
			return (
				<div>
					<h1>Weather forecast</h1>
					<p>This component demonstrates fetching data from the server.</p>
					{this.renderForecastsTable()}
				</div>
			);
		return <h1><i>Loading...</i></h1>;
	}
}

function mapDispatchToProps(dispatch) {
	return {
		fetchForecastData: () => dispatch(fetchForecastData())
	};
}

function mapStateToProps(state) {
	return {
		forecastStatus: state.forecast.status,
		forecastData: state.forecast.data
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(FetchData);
