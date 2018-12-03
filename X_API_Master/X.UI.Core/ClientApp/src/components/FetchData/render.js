import React from 'react';
import { FETCHDATA } from '../../const/components';
/**
 * 函数组件
 * @param {*} props 
 */
export default function render(props) {
    //const { action, isLoading, data, state, context, target } = props;
    //代码	名称	价格	涨幅	封成比	封流比	封单金额 < 亿元 > 金额 < 亿元 > 第一次涨停	最后一次涨停	打开次数	涨停强度
    const { isLoading, state, context, target } = props;
    const stockDatas = state.stockDatas;
    const stateFilter = (p) => {
        if (state.filter) {
            return state.filter.indexOf(p.StockCode) > -1;
        }
        return true;
    }
    let contents = isLoading
        ? <p><em>Loading...</em></p>
        : <table className='table'>
            <thead>
                <tr>
                    <th>序号</th>
                    <th>代码</th>
                    <th>名称</th>
                    <th>价格</th>
                    <th>涨幅</th>
                    <th>封成比</th>
                    <th>封流比</th>
                    <th>封单金额(亿元)</th>
                    <th>金额(亿元)</th>
                    <th>第一次涨停</th>
                    <th>最后一次涨停</th>
                    <th>打开次数</th>
                    <th>涨停强度</th>
                </tr>
            </thead>
            <tbody>
                {stockDatas.filter(p => p.PriceLimit > 7 && stateFilter(p)).map((stockData, index) =>
                    <tr key={stockData.StockCode}>
                        <td>{index+1}</td>
                        <td>{stockData.StockCode}</td>
                        <td>{stockData.StockName}</td>
                        <td>{stockData.Price}</td>
                        <td>{stockData.PriceLimit}</td>
                        <td>{stockData.FCB.toFixed(2)}</td>
                        <td>{(stockData.FLB*100).toFixed(2)}</td>
                        <td>{(stockData.FDMoney / 1e8).toFixed(4)}</td>
                        <td>{(stockData.Amount/1e8).toFixed(4)}</td>
                        <td>{new Date(stockData.FirstZtTime).toLocaleTimeString()}</td>
                        <td>{new Date(stockData.LastZtTime).toLocaleTimeString()}</td>
                        <td>{stockData.OpenTime}</td>
                        <td>{stockData.Force.toFixed(4)}</td>
                    </tr>
                )}
            </tbody>
        </table>;

    const onClick = (tab) => {
        context.handleEvents.call(target, {
            type: FETCHDATA.Actions.CHANGE,
            data: {
                tab: tab
            }
        });
    };

    const onFilter = (e) => {
        context.handleEvents.call(target, {
            type: FETCHDATA.Actions.FILTER,
            data: {
                filter: e.target.value
            }
        });
    };

    return (
        <div>
            <h1>Stock Datas</h1>
            <textarea value={state.filter} onChange={onFilter}></textarea>
            <nav className='fetch'>
                <a className={state.tab === 0 ? "active" : ""} onClick={e => onClick(0)}>涨停</a>
                <a className={state.tab === 1 ? "active" : ""} onClick={e => onClick(1)}>烂板</a>
                <a className={state.tab === 2 ? "active" : ""} onClick={e => onClick(2)}>加速</a>
                <a className={state.tab === 3 ? "active" : ""} onClick={e => onClick(3)}>加速烂板</a>
                <a className={state.tab === 4 ? "active" : ""} onClick={e => onClick(4)}>加速小成交</a>
                <a className={state.tab === 5 ? "active" : ""} onClick={e => onClick(5)}>涨停小成交</a>
            </nav>
            {contents}
        </div>
    );
};