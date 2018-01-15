import React from 'react';
import PropTypes from 'prop-types';
import {TaskShape} from "../proptypesShapes/TaskShape";

export class TaskListItem extends React.Component {

    static displayName = 'TaskListItem';

    static propTypes = {
        task: TaskShape.isRequired,

        onSelect: PropTypes.func.isRequired,
    };

    _handleSelect = () =>
    {
        this.props.onSelect(this.props.task);
    };

    render() {
        const task = this.props.task;

        return (
            <li key={task.id} className="list-group-item btn-block btn" onClick={this._handleSelect}>
                <span>{Math.abs(task.id)}: {task.name} </span>
            </li>
        )
    }
}