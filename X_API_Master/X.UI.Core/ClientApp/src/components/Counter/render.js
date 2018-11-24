import React from 'react';
import { COUNTER } from '../../const/components';
/**
 * 函数组件
 * @param {*} props 
 */
export default function render(props) {
    const { state, context, target } = props;
    return <div>
        <h1>Counter</h1>
        <p>This is a simple example of a React component.</p>
        <p>Current count: <strong>{state.currentCount}</strong></p>
        <button onClick={
            e => {
                context.handleEvents.call(target, {
                    type: COUNTER.Actions.Add,
                });
            }}>Increment</button>
    </div>;
};