import React from 'react';
import { LOADMORE } from '../../const/components';


/**
 * 函数组件
 * @param {*} props 
 */
export default function render(props) {
    const { action, isLoading, data, state, context, target } = props;
    const actionType = action.type,
        parentProps = target.props;

    const onTouchEnd = (column, index, state, context) => {
        if (!state.noMore) {
            column = column || 1;
            if (Math.floor(index / column) >= Math.floor(state.list.length / column - parentProps.pageSize / 2 / column)) {
                context.handleEvents.call(target, {
                    type: LOADMORE.Actions.LOADMORE
                })
            }
        }
    }

    if (state.list.length > 0) {
        return <div className="main-data">
            <ul>
                {
                    state.list.map((item, index) => {
                        if ((actionType == LOADMORE.Actions.LOAD || actionType == LOADMORE.Actions.LOADMORE) && isLoading) {
                            return <li key={index} onTouchEnd={e => onTouchEnd(1, index, state, context)}>
                                <p>{item}</p>
                            </li>
                        }
                        return <li key={index} onTouchEnd={e => onTouchEnd(1, index, state, context)}>
                            <p>{item}</p>
                        </li>
                    })
                }
            </ul>
            {
                state.noMore ? <p>{parentProps.noMoreTip}</p> : null
            }
            {
                isLoading ? <div className="box">loading...</div> : null
            }
        </div>
    } else {
        if ((actionType == LOADMORE.Actions.LOAD || actionType == LOADMORE.Actions.LOADMORE) && isLoading) {
            return <div className="box">loading...</div>;
        }
        return <div className="main-data">
            {
                <p>{parentProps.noDataTip}</p>
            }
        </div>
    }
}