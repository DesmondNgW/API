import React, { Component } from 'react';
import { FETCHDATA } from '../../const/components';
import { glabalEvents } from '../../const/const';
import ReactProxyProvider from '../../core/React-Proxy-Provider'
import middleWare from './middleWare';
import './fetch.css';


export class FetchData extends Component {
    displayName = FetchData.name

    constructor(props) {
        super(props);
        this.dt = this.props.match.params.dt || "";
        this.state = {
            action: {
                type: FETCHDATA.Actions.LOAD,
                data: {
                    tab: 0
                }
            },
            version: 1
        };
        this.execContext = this.props.context || {};
    }

    render() {
        return <ReactProxyProvider id={FETCHDATA.ID} parent={this} action={this.state.action} version={this.state.version}
            context={this.execContext} middleWare={middleWare} globalEvents={Object.values(glabalEvents)} />
    }
}
