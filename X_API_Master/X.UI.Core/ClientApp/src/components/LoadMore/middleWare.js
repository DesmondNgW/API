import React from 'react';
import render from './render';
import { LOADMORE } from '../../const/components';

export default {
    state: {
        list: [],
        noMore: true,
        currentIndex: 0
    },
    events: {
        [[LOADMORE.Actions.LOAD]]: {
            fetchOptions: {
                params: {},
                fetch(params, state, context) {
                    return this.load(Object.assign({}, params, this.loadParams, { currentIndex: state.currentIndex + 1 }));
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
                    return {
                        list: state.list.concat(data.list),
                        noMore: data.noMore,
                        currentIndex: state.currentIndex + 1
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
        },
        [[LOADMORE.Actions.LOADMORE]]: {
            fetchOptions: {
                params: {},
                fetch(params, state, context) {
                    return this.load(Object.assign({}, params, this.loadParams, { currentIndex: state.currentIndex + 1 }));
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
                    return {
                        list: state.list.concat(data.list),
                        noMore: data.noMore,
                        currentIndex: state.currentIndex + 1
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
    }
}