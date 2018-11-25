import React from 'react';
import render from './render';
import { COUNTER } from '../../const/components';

export default {
    state: { currentCount: 0},
    events: {
        [[COUNTER.Actions.LOAD]]: {
            fetchedOptions: {
                nextState(action, data, state, context) {
                    return { }
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
        },
        [[COUNTER.Actions.Add]]: {
            fetchedOptions: {
                nextState(action, data, state, context) {
                    return { currentCount: state.currentCount + 1 }
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