import React, { Component } from 'react';
import { Route, Switch } from 'react-router';
import { Helmet } from "react-helmet";

import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { Counter } from './components/Counter';
import { NotFound } from './components/NotFound';

export default class App extends Component {
  displayName = App.name

  render() {
    return (
        <Layout>
            <Helmet titleTemplate="%s | Consultations | NICE" />
            <Switch>
                <Route exact path='/' component={Home} />
                <Route path='/counter' component={Counter} />
                <Route path='/fetchdata' component={FetchData} />
                <Route component={NotFound} />
            </Switch>
      </Layout>
    );
  }
}
