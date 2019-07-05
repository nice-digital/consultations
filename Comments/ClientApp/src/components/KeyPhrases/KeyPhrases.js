// @flow

import React, { Component, } from "react";

import { TagCloud } from "react-tagcloud";

type PropsType = {
    keyPhrases: Array<KeyPhrase>,
};

type StateType = {};

export class KeyPhrases extends Component<PropsType, StateType> {
	constructor() {
		super();
		this.state = {};
    }
    
	render() {

        const data = this.props.keyPhrases.map(keyPhrase => { return {value: keyPhrase.text, count: keyPhrase.score * 10000000}});        
          
		return (
            <TagCloud minSize={12}
                maxSize={45}
                tags={data}
                onClick={tag => console.log(`'${tag.value}' was selected!`)} />
		);
	}
}

export default KeyPhrases;
