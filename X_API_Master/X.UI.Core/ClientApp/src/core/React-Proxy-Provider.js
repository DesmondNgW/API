/**
 * Page->Component->[Begin->CSS->Render->Load->Render->End->User->Event->Render->Load->Render->End]
 * Page->Component->[Begin->CSS->Render->End->User->Event->Render->End]
 * Page->Component->[Begin->CSS->Render->Load->Render->End]
 */
import { Component } from 'react';
import eventBus from './eventBus.js';
import { glabalEvents } from '../const/const';

//onLoad
//onAppear
//onDisAppear
//onOnline
//onOffline

/**
 * props:
 * {
 *      id --组件id
 *      parent -- 组件this
 *      context,--全局上下文
 *      action{type,data} --组件变化发送action实体
 *      version --组件版本号,版本号变化会渲染页面
 *      middleWare --组件中间件逻辑
 * }
 * onLoad 组件加载
 * onDisAppear 组件处于未激活
 * onAppear 组件因其他原因重新激活
 * onOnline 组件因网络原因重新联网
 * onOffline 组件因网路原因断网
 */
export default class ReactProxyProvider extends Component {
    constructor(props) {
        super(props)

        //ProxyProvider's props
        this.actionType = "onLoad";
        this.actionData = null;
        this.propsState = null;
        this.globalEvents = Object.values(glabalEvents);
        //Parent's props
        this.id = this.props.id || +new Date();
        this.parent = this.props.parent;
        this.action = this.props.action;
        this.execContext = this.props.context || {};
        this.middleWare = this.props.middleWare;
        if (this.props.globalEvents && this.props.globalEvents.length) {
            this.globalEvents = [...this.props.globalEvents]
        }

        // get Action
        if (this.action) {
            this.actionType = this.action.type;
            this.actionData = this.action.data;
        }

        // get MiddleWare
        if (this.middleWare) {
            this.propsState = this.middleWare.state;
            this.events = this.middleWare.events;
            if (this.events) {
                this.target = this.events[this.actionType];
                if (this.target) {
                    this.fetchOptions = this.target.fetchOptions;
                    this.fetchingOptions = this.target.fetchingOptions;
                    this.fetchedOptions = this.target.fetchedOptions;
                }
            }
        }

        //state
        this.state = {
            version: 1,
            isLoading: this.fetchOptions ? true : false,
            data: null,
            state: this.propsState,
        }

        //Global Context 
        if (!this.execContext[this.id]) {
            this.execContext[this.id] = {};
        }
        this.execContext[this.id].name = this.id;
        this.execContext[this.id].handleEvents = this.handleEvents.bind(this.parent);
        this.execContext[this.id].refresh = this.refresh.bind(this.parent);
        this.execContext[this.id].state = Object.assign({}, this.state.state);
        if (!this.execContext.handleEvents) this.execContext.handleEvents = this.handleEvents;
        if (!this.execContext.emitContextRefresh) this.execContext.emitContextRefresh = this.emitContextRefresh;
        if (!this.execContext.getContextState) this.execContext.getContextState = this.getContextState;
        if (!this.execContext.targetNames) this.execContext.targetNames = [];
        if (this.execContext.targetNames.indexOf(this.id) === -1) this.execContext.targetNames.push(this.id);

        this.globalKey = Symbol.for("ProxyProvider.Context");
        //register Global Context into Global
        if (window) {
            window[this.globalKey] = this.execContext;
        } else if (global) {
            global[this.globalKey] = this.execContext;
        }
    }

    /**
     * register context api
     * @param {*} action 
     */
    handleEvents(action) {
        this.setState({
            action: Object.assign({}, action, { isfresh: false }),
            version: (this.state.version || 0) + 1
        });
    }

    /**
     * register context api
     */
    refresh() {
        this.setState({
            action: Object.assign({}, this.state.action, { isfresh: true }),
            version: (this.state.version || 0) + 1
        })
    }

    /**
     * register context api
     * @param {*} context 
     * @param {*} target 
     */
    emitContextRefresh(context, target) {
        if (target) {
            if (Object.prototype.toString.call(target) === "[object String]") {
                context[target].refresh();
            } else if (Array.isArray(target)) {
                for (let i = 0, len = target.length; i < len; i++) {
                    context[target[i]].refresh();
                }
            }
        }
    }

    /**
     * register context api
     * @param {*} context 
     * @param {*} target 
     */
    getContextState(context, target) {
        if (target) {
            if (Object.prototype.toString.call(target) === "[object String]") {
                return context[target].state;
            } else if (Array.isArray(target)) {
                let ret = [], len = target.length;
                for (let i = 0; i < len; i++) {
                    ret[i] = context[target[i]].state;
                }
                return ret;
            }
            return null;
        }
    }

    /**
     * fetch new action data
     * @param {*} param 
     */
    async load(param) {
        return await this.fetchOptions.fetch.call(this.parent, param, this.parent.state.action, this.state.state, this.execContext);
    }
    //next State
    getState(state, data) {
        if (Object.prototype.toString.call(state) === "[object Function]") {
            if (data) {
                return Object.assign({}, this.state.state, state.call(this.parent, this.parent.state.action, data, this.state.state, this.execContext));
            }
            return Object.assign({}, this.state.state, state.call(this.parent, this.parent.state.action, this.state.state, this.execContext));
        } else if (Object.keys(state).length) {
            return Object.assign({}, this.state.state, state);
        }
        return this.state.state;
    }

    /**
     * loading state
     */
    setLoading() {
        let state = {
            isLoading: true,
            version: this.state.version + 1
        };
        if (this.fetchingOptions && this.fetchingOptions.nextState) {
            state["state"] = this.getState(this.fetchingOptions.nextState);
        }

        this.execContext[this.id].state = state.state;
        this.setState(state, () => {
            this.execContext[this.id].state = this.state.state;
        });
    }

    /**
     * loaded state
     * @param {*} data 
     */
    setLoaded(data) {
        let state = {
            data: data,
            isLoading: false,
            version: this.state.version + 1
        };
        if (this.fetchedOptions && this.fetchedOptions.nextState) {
            state["state"] = this.getState(this.fetchedOptions.nextState, data);
        }

        this.execContext[this.id].state = state.state;
        this.setState(state, () => {
            this.execContext[this.id].state = this.state.state;
        });
    }

    /**
     * fresh state
     */
    freshState() {
        let state = {
            data: this.state.data,
            isLoading: false,
            version: this.state.version + 1
        };
        this.execContext[this.id].state = state.state;
        this.setState(state, () => {
            this.execContext[this.id].state = this.state.state;
        });
    }

    binding() {
        if (this.fetchOptions) {
            this.setLoading();
            this.load(this.fetchOptions.params).then((data) => {
                this.setLoaded(data);
            }).catch((err) => {
                console.log(err);
            })
        } else if (this.fetchedOptions) {
            this.setLoaded({});
        }
    }

    registerGlobalEvents() {
        this.globalEvents.map((item, index) => {
            const fun = () => {
                this.handleEvents.call(this.parent, {
                    type: item,
                    data: this.parent
                })
            };
            fun.id = this.parent;
            return eventBus.addListener(item, fun);
        })
    }

    componentDidMount() {
        this.binding();
        this.registerGlobalEvents();
    }

    componentDidUpdate() { }

    /**
     * shouldComponentUpdate
     * @param {*} nextProps 
     * @param {*} nextState 
     */
    shouldComponentUpdate(nextProps, nextState) {
        return this.props.version !== nextProps.version || nextState.version !== this.state.version
    }

    /**
     * componentWillReceiveProps
     * @param {*} nextProps 
     */
    componentWillReceiveProps(nextProps) {
        this.actionType = null;
        this.actionData = null;
        this.action = nextProps.action;
        if (this.action) {
            this.actionType = this.action.type;
            this.actionData = this.action.data;
        }
        if (this.middleWare && this.events && this.actionType) {
            this.target = this.events[this.actionType];
            if (this.target) {
                this.fetchOptions = this.target.fetchOptions;
                this.fetchingOptions = this.target.fetchingOptions;
                this.fetchedOptions = this.target.fetchedOptions;
                if (this.props.version !== nextProps.version) {
                    if (this.action.isfresh) {
                        this.freshState();
                    } else {
                        this.binding();
                    }
                }
            }
        }
    }

    render() {
        console.log("ReactProxyProvider", {
            target: this.parent,
            action: this.parent.state.action,
            version: this.parent.state.version,
            ProxyVersion: this.state.version,
            ProxyLoading: this.state.isLoading,
            ProxyData: this.state.data
        });

        if (this.fetchOptions || this.fetchedOptions) {
            if (this.state.isLoading) {
                return this.fetchingOptions.render.call(this.parent, this.parent.state.action, true, null, this.state.state, this.execContext);
            } else {
                return this.fetchedOptions.render.call(this.parent, this.parent.state.action, false, this.state.data, this.state.state, this.execContext);
            }
        } else {
            return this.props.children || null;
        }
    }
}