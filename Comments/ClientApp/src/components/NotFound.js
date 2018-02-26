import React, { Component } from 'react';
import { Route } from 'react-router'

export class NotFound extends Component {
    render() {
        return (
            <div>
                <Route render={({ staticContext }) => {
                    if (staticContext)
                        staticContext.status = 404
                    return null;
                }} />
                <h1>Not found</h1>

                <p>Sorry that page could not be found.</p>
            </div>
        );
    }
}
