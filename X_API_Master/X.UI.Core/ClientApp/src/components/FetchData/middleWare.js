import React from 'react';
import render from './render';
import { FETCHDATA } from '../../const/components';

export default {
    state: { forecasts: [] },
    events: {
        [[FETCHDATA.Actions.LOAD]]: {
            fetchOptions: {
                params: {},
                fetch(params, state, context) {
                    return fetch('api/SampleData/WeatherForecasts')
                        .then(response => response.json());
                }
            },
            fetchingOptions: {
                nextState(state, context) {
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
                nextState(data, state, context) {
                    return { forecasts: data}
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
    }
}