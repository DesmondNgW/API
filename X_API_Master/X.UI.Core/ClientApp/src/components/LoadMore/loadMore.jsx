import React, { Component } from 'react';
import { LOADMORE } from '../../const/components';
import ReactProxyProvider from '../../core/React-Proxy-Provider'
import middleWare from './middleWare';

require("./loadMore.css")

/**
 * props:
 * {
 *      load(loadParams) --加载数据，返回值格式{list:[],noMore:} 请求页码 loadParams.currentIndex
 *      loadParams -- 加载数据的参数
 *      execContext-- 全局上下文
 *      pageSize --每页条数
 *      noMoreTip -- 加载至最后一页时文案
 *      noDataTip -- 无数据时文案
 *      render -- 外部渲染函数
 * }
 */
export default class LoadMore extends Component {
    constructor(prop) {
        super(prop);
        this.state = {
            action: {
                type: LOADMORE.Actions.LOAD,
                data: null
            },
            version: 1
        };
        this.execContext = this.props.context || {};
        this.load = this.props.load;
        this.loadParams = this.props.loadParams;
    }

    componentDidMount() {

    }

    render() {
        return <ReactProxyProvider id={LOADMORE.ID} parent={this} action={this.state.action} version={this.state.version}
            context={this.execContext} middleWare={middleWare} />
    }
}