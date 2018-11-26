import React from 'react';
import render from './render';
import { COUNTER } from '../../const/components';

export default {
    state: { currentCount: 0 },
    fetchedOptions: {
        nextState(action, data, state, context) {
            switch (action.type) {
                //case COUNTER.Actions.LOAD:
                case COUNTER.Actions.Add:
                    return { currentCount: state.currentCount + 1 }
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