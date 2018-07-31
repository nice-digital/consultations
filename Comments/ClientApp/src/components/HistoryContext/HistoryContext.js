import React from "react";

import browserHistory from "./../../../helpers/history";

export const HistoryContext = React.createContext({ history: browserHistory });

export const Provider = HistoryContext.Provider;
export const Consumer = HistoryContext.Consumer;

export const withHistory = (Component) =>
	(function HistoryComponent(props) {
		return (
			<Consumer>
				{({history}) => <Component {...props} history={history} />}
			</Consumer>
		);
	});

export default withHistory;
