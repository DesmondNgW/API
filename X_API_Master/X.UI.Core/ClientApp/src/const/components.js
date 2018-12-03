//组件id 以及Actions 配置表
const COUNTER = {
    ID: "Counter",
    Actions: {
        LOAD: "onLoad",
        Add: "onAdd"
    }
}

const FETCHDATA = {
    ID: "FetchData",
    Actions: {
        LOAD: "onLoad",
        CHANGE: "onChange",
        FILTER: "onFilter"
    }
}

const LOADMORE = {
    ID: "LoadMore",
    Actions: {
        LOAD: "onLoad",
        LOADMORE: "onLoadMore"
    }
};

export { COUNTER, FETCHDATA, LOADMORE }