import React, { Component } from 'react';
import { COUNTER } from '../../const/components';
import { glabalEvents } from '../../const/const';
import ReactProxyProvider from '../../core/React-Proxy-Provider'
import middleWare from './middleWare';

export class Counter extends Component {
    displayName = Counter.name

    constructor(props) {
        super(props);

        this.state = {
            action: {
                type: COUNTER.Actions.LOAD,
                data: null
            },
            version: 1
        };
        this.execContext = this.props.context || {};
    }

    render() {
        return <ReactProxyProvider id={COUNTER.ID} parent={this} action={this.state.action} version={this.state.version}
            context={this.execContext} middleWare={middleWare} globalEvents={Object.values(glabalEvents)} />
    }
}
