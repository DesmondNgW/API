import React from 'react';
import render from './render';
import { FETCHDATA } from '../../const/components';

export default {
    state: { stockDatas: [], tab: 0, filter: "" },
    fetchOptions: {
        params(action, state, context) {
            return {};
        },
        fetch(params, action, state, context) {
            return fetch(`api/Proxy/GetStockData?dt=${this.dt}&tab=${action.data.tab}`)
                .then(response => response.json());
        }
    },
    fetchingOptions: {
        nextState(action, state, context) {
            return {}
        },
        render(action, isLoading, data, state, context) {
            const Component = this.props.render || render;
            return <Component
                action={action}
                isLoading={isLoading}
                data={data}
                state={state}
                context={context}
                target={this} />
        }
    },
    fetchedOptions: {
        nextState(action, data, state, context) {
            switch (action.type) {
                case FETCHDATA.Actions.LOAD:
                case FETCHDATA.Actions.CHANGE:
                    return { stockDatas: data, tab: action.data.tab }
                case FETCHDATA.Actions.FILTER:
                    return { filter: action.data.filter}
                default:
                    return {}
            }
        },
        render(action, isLoading, data, state, context) {
            const Component = this.props.render || render;
            return <Component
                action={action}
                isLoading={isLoading}
                data={data}
                state={state}
                context={context}
                target={this} />
        }
    }
}