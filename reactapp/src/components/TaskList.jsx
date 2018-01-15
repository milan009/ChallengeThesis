import React from 'react';

import {TaskShape} from "../proptypesShapes/TaskShape";
import PropTypes from "prop-types";

import {TaskListItem} from "./TaskListItem";

export class TaskList extends React.Component {

    static propTypes = {
        tasks: PropTypes.arrayOf(TaskShape.isRequired).isRequired,

        onTaskSelected: PropTypes.func.isRequired,
    };

    render() {

        const elements = this.props.tasks.map((t) => <TaskListItem key={t.id} task={t} onSelect={() => this.props.onTaskSelected(t)}/>);

        return (
            <div>
                <ul className="list-group">
                    {elements}
                </ul>
            </div>
        )
    }
}