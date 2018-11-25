import React, { Component } from 'react';

export class FrameWork extends Component {
    displayName = FrameWork.name

    render() {
        if (props.box) {
            return this.props.children
        }
        return <div className="__framework-box">{this.props.children}</div>
    }
}