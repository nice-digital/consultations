import React from 'react';
import ReactDOM from 'react-dom';
import App from './App';
import { MemoryRouter } from 'react-router-dom';

describe('Client Unit Tests', ()=>{
  describe('App', ()=>{
    it('Renders without crashing', () => {
      const div = document.createElement('div');
      ReactDOM.render(<MemoryRouter><App /></MemoryRouter>, div);
    });
    it('Really does render without crashing', () => {
      const div = document.createElement('div');
      ReactDOM.render(<MemoryRouter><App /></MemoryRouter>, div);
    });
  })  
})
