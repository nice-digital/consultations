import React, { Component } from "react";
import { connect } from "react-redux";
import { fetchForecastData } from "./forecastActions";

// import fetch from "isomorphic-fetch";
// todo: move to the forecast actions

class FetchData extends Component {
	displayName = FetchData.name;

	componentWillMount() {
		this.props.fetchForecastData();
	}

	renderForecastsTable = () => {
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
		const { forecastData } = this.props;
		if (!forecastData) return null;
		return (
			<div>
				<h1>Weather forecast</h1>
				<p>This component demonstrates fetching data from the server.</p>
				{this.renderForecastsTable()}
			</div>
		);
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
