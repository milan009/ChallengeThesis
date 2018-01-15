import React from 'react';
import PropTypes from 'prop-types';
import {TaskShape} from "../proptypesShapes/TaskShape";

export class TaskEditForm extends React.Component {

    constructor(props) {
        super(props);

        if(props.selectedTask)
            this.state = {
                name: props.selectedTask.name,
                description: props.selectedTask.description,
            };
        else
             this.state = {
                name: "",
                description: "",
            };
    }

    static displayName = 'TaskEditForm';

    static propTypes = {
        selectedTask: TaskShape,

        onEditSave: PropTypes.func.isRequired,
        onCreateNew: PropTypes.func.isRequired,
    };

    _handleNameChange = (event) => {
        const setStateText = (text) => (() => ({name: text}));
        this.setState(setStateText(event.target.value));
    };

    _handleDescChange = (event) => {
        const setStateText = (text) => (() => ({description: text}));
        this.setState(setStateText(event.target.value));
    };

    _saveEdit = () => {
        if(this._canSubmit())
            this.props.onEditSave(this.state.name, this.state.description, this.props.selectedTask.id);
    };

    _createNew = () => {
        if(this._canSubmit()) {
            this.props.onCreateNew(this.state.name, this.state.description);
            this.setState({name: "", description: ""});
        }
    };

    _canSubmit = () => this.state.name !== "" && this.state.descriprion !== "";

    render() {
        return (
            <div>
                <div>
                    <label>Název úkolu</label>
                    <input type="text" className="form-control" onChange={this._handleNameChange} value={this.state ? this.state.name : ''}/>
                </div>

                <label>Popis úkolu</label>
                <div className="form-group">
                    <textarea className="form-control" rows="5" id="comment" onChange={this._handleDescChange} value={this.state ? this.state.description : ''}/>
                </div>

                <div className="form-group">
                    <input type="button"
                           className={"form-control btn btn-success " + ((this._canSubmit()) ? "" : "disabled")}
                           value={this.props.selectedTask ? "Uložit změny" : "Vytvořit úkol"}
                           onClick={this.props.selectedTask ? this._saveEdit : this._createNew}/>
                </div>
            </div>

        )
    }
}