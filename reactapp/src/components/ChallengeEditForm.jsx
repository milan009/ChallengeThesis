import React from 'react';
import PropTypes from 'prop-types';
import {ChallengeShape} from "../proptypesShapes/ChallengeShape";
import {TaskList} from "./TaskList";
import {TaskEditForm} from "./TaskEditForm";
import {Task} from "../models/Task";
import {Challenge} from "../models/Challenge";

export class ChallengeEditForm extends React.Component {

    constructor(props) {
        super(props);
        if(props.selectedChallenge) {
            this.state = {
                currentChallenge: props.selectedChallenge,
                maleName: props.selectedChallenge.names.male,
                femaleName: props.selectedChallenge.names.female,
                description: props.selectedChallenge.description,
                category: props.selectedChallenge.category,
                imageUri: props.selectedChallenge.imageUri,
                basicTasks: props.selectedChallenge.tasks.basic,
                extraTasks: props.selectedChallenge.tasks.extra,
                basicCubs: props.selectedChallenge.requirements.basic.cubs,
                basicScouts: props.selectedChallenge.requirements.basic.scouts,
                basicGuides: props.selectedChallenge.requirements.basic.guides,
                extraScouts: props.selectedChallenge.requirements.extra.scouts,
                extraGuides: props.selectedChallenge.requirements.extra.guides,
                extraCubs: props.selectedChallenge.requirements.extra.cubs,
            }
        }
        else {
            this.state = {
                currentChallenge: {
                    tasks: {
                            basic: [],
                            extra: [],
                        },
                },
                maleName: "",
                femaleName: "",
                description: "",
                category: 0,
                imageUri: "",
                basicTasks: [],
                extraTasks: [],
                basicCubs: 0,
                basicScouts: 0,
                basicGuides: 0,
                extraScouts: 0,
                extraGuides: 0,
                extraCubs: 0,
            }
        }
    }

    static displayName = 'ChallengeEditForm';

    static propTypes = {
        selectedChallenge: ChallengeShape,
        edit: PropTypes.bool.isRequired,

        onSaveChallenge: PropTypes.func.isRequired,
    };

    _handleMaleNameChange = (event) => {
        const setStateText = (text) => (() => ({maleName: text}));
        this.setState(setStateText(event.target.value));
    };


    _handleFemaleNameChange = (event) => {
        const setStateText = (text) => (() => ({femaleName: text}));
        this.setState(setStateText(event.target.value));
    };

    _handleDescChange = (event) => {
        const setStateText = (text) => (() => ({description: text}));
        this.setState(setStateText(event.target.value));
    };

    _handleUriChange = (event) => {
        const setStateText = (text) => (() => ({imageUri: text}));
        this.setState(setStateText(event.target.value));
    };

    _handleReqsBasicCubsChange = (event) => {
        const setStateValue = (value) => (() => ({basicCubs: value}));
        this.setState(setStateValue(event.target.value));
    };

    _handleReqsBasicGuidesChange = (event) => {
        const setStateValue = (value) => (() => ({basicGuides: value}));
        this.setState(setStateValue(event.target.value));
    };

    _handleReqsBasicScoutsChange = (event) => {
        const setStateValue = (value) => (() => ({basicScouts: value}));
        this.setState(setStateValue(event.target.value));
    };

    _handleReqsExtraCubsChange = (event) => {
        const setStateValue = (value) => (() => ({extraCubs: value}));
        this.setState(setStateValue(event.target.value));
    };

    _handleReqsExtraGuidesChange = (event) => {
        const setStateValue = (value) => (() => ({extraGuides: value}));
        this.setState(setStateValue(event.target.value));
    };

    _handleReqsExtraScoutsChange = (event) => {
        const setStateValue = (value) => (() => ({extraScouts: value}));
        this.setState(setStateValue(event.target.value));
    };

    _handleBasicTaskSelect = (task) =>
        this.setState({selectedBasicTask: task});

    _handleExtraTaskSelect = (task) =>
        this.setState({selectedExtraTask: task});

    _editBasicTask = (name, desc, id) =>
    {
        this.setState((prevState) => {
            const ch = prevState.currentChallenge;

            let i = 0;
            while(ch.tasks.basic[i].id !== id) {
                i++;
            }

            ch.tasks.basic[i] = new Task(id, name, desc, undefined);
            return {
                currentChallenge: ch,
                selectedBasicTask: undefined,
                selectedExtraTask: undefined,
            };
        })
    };

    _createBasicTask = (name, desc) =>
    {
        this.setState((prevState) => {
            const ch = prevState.currentChallenge;

            ch.tasks.basic.push(new Task(-1 * (ch.tasks.basic.length + 1), name, desc, undefined));
            return {
                currentChallenge: ch,
                selectedBasicTask: undefined,
                selectedExtraTask: undefined,
            };
        })
    };

    _editExtraTask = (name, desc, id) =>
    {
        this.setState((prevState) => {
            const ch = prevState.currentChallenge;

            let i = 0;
            while(ch.tasks.extra[i].id !== id) {
                i++;
            }

            ch.tasks.extra[i] = new Task(id, name, desc, undefined);
            return {
                currentChallenge: ch,
                selectedBasicTask: undefined,
                selectedExtraTask: undefined,
            };
        })
    };

    _createExtraTask = (name, desc) =>
    {
        this.setState((prevState) => {
            const ch = prevState.currentChallenge;

            ch.tasks.extra.push(new Task(-1 * (ch.tasks.extra.length + 1), name, desc, undefined));
            return {
                currentChallenge: ch,
                selectedBasicTask: undefined,
                selectedExtraTask: undefined,
            };
        })
    };

    _saveChallenge = () =>
    {
        const editedChallenge = new Challenge({
            names: {
                male: this.state.maleName,
                female: this.state.femaleName,
            },
            description: this.state.description,
            competences: this.state.currentChallenge.competences,
            tasks: this.state.currentChallenge.tasks,

            imageUri: this.state.imageUri,
            requirements: {
                basic: {
                    cubs: this.state.basicCubs,
                    scouts: this.state.basicScouts,
                    guides: this.state.basicGuides,
                },
                extra: {
                    cubs: this.state.extraCubs,
                    scouts: this.state.extraScouts,
                    guides: this.state.extraGuides,
                },
            },
            category: this.state.category,
            // ...this.state.currentChallenge,
        });

        if(this.props.edit)
            editedChallenge.id = this.state.currentChallenge.id;

        const challengeDto = editedChallenge.toDto();
        console.log(challengeDto);
        this.props.onSaveChallenge(challengeDto);
    };


    render() {
        const challenge = this.props.selectedChallenge;

        return (
            <div id="chEdit">
                <form className="form">
                    <div>
                        <h3>Základní údaje</h3>
                        <label>Název zkoušky</label>
                        <div className="form-group form-inline">
                            <div className="input-group">
                                <input type="text"
                                       className="form-control"
                                       placeholder="Mužský název"
                                       defaultValue={challenge ? this.state.maleName : ""}
                                       onChange={this._handleMaleNameChange}/>
                            </div>
                            <div className="input-group"> / </div>
                            <div className="input-group">

                                <input type="text"
                                       className="form-control"
                                       placeholder="Ženský název"
                                       defaultValue={challenge ? this.state.femaleName : ""}
                                       onChange={this._handleFemaleNameChange}
                                />
                            </div>
                        </div>
                    </div>
                    <div>
                        <label>URL obrázku</label>
                        <div className="form-group">
                            <input type="text"
                                className="form-control"
                                id="url"
                                defaultValue={challenge ? this.state.imageUri : "" }
                                onChange={this._handleUriChange}
                            />
                        </div>
                    </div>
                    <div>
                    <label>Kategorie zkoušky</label>
                        <div className="form-group">
                            <select
                                className="form-control"
                                defaultValue={challenge ? this.state.category : 0}
                                onChange={this._handleCategoryChange}
                            >
                                <option value="0">Umělecké</option>
                                <option value="1">Technické</option>
                                <option value="2">Tábornicko-Cestovatelské</option>
                                <option value="3">Humanitní</option>
                                <option value="4">Přírodovědné</option>
                                <option value="5">Vodácké Světle</option>
                                <option value="6">Služba bližním</option>
                                <option value="7">Sportovní</option>
                                <option value="8">Duchovní</option>
                                <option value="9">Život v oddíle</option>
                            </select>
                        </div>
                    </div>
                    <div>
                        <label>Popis zkoušky</label>
                        <div className="form-group">
                            <textarea
                                className="form-control"
                                rows="5"
                                id="comment"
                                defaultValue={challenge ? this.state.description : "" }
                                onChange={this._handleDescChange}
                            />
                        </div>
                    </div>
                    <div>
                        <label>Požadované počty hlavních úkolů:</label>
                        <div className="form-group form-inline">
                            <div className="form-group">
                                <input type="text"
                                       className="form-control"
                                       placeholder="Vlčata"
                                       defaultValue={challenge ? this.state.basicCubs : ""}
                                       onChange={this._handleReqsBasicCubsChange}/>
                            </div>
                            <div className="form-group">

                                <input type="text"
                                       className="form-control"
                                       placeholder="Skauti"
                                       defaultValue={challenge ? this.state.basicScouts : ""}
                                       onChange={this._handleReqsBasicScoutsChange}
                                />
                            </div>
                            <div className="form-group">

                                <input type="text"
                                       className="form-control"
                                       placeholder="Roveři"
                                       defaultValue={challenge ? this.state.basicGuides : ""}
                                       onChange={this._handleReqsBasicGuidesChange}
                                />
                            </div>
                        </div>
                        <label>Požadované počty doplňkových úkolů:</label>
                        <div className="form-group form-inline">
                            <div className="form-group">
                                <input type="text"
                                       className="form-control"
                                       placeholder="Vlčata"
                                       defaultValue={challenge ? this.state.extraCubs : ""}
                                       onChange={this._handleReqsExtraCubsChange}/>
                            </div>
                            <div className="form-group">

                                <input type="text"
                                       className="form-control"
                                       placeholder="Skauti"
                                       defaultValue={challenge ? this.state.extraScouts : ""}
                                       onChange={this._handleReqsExtraScoutsChange}
                                />
                            </div>
                            <div className="form-group">

                                <input type="text"
                                       className="form-control"
                                       placeholder="Roveři"
                                       defaultValue={challenge ? this.state.extraGuides : ""}
                                       onChange={this._handleReqsExtraGuidesChange}
                                />
                            </div>
                        </div>
                    </div>
                    <hr />
                    <div>
                        <h3>Hlavní úkoly</h3>
                        <TaskEditForm
                            selectedTask={this.state.selectedBasicTask}
                            key={this.state.selectedBasicTask ? this.state.selectedBasicTask.id : "emptyBasic" }
                            onEditSave={this._editBasicTask}
                            onCreateNew={this._createBasicTask}
                            />
                        {
                            this.state.currentChallenge && this.state.currentChallenge.tasks.basic ?
                                <TaskList tasks={this.state.currentChallenge.tasks.basic} onTaskSelected={this._handleBasicTaskSelect}/> : ""
                        }
                    </div>
                    <hr />
                    <div>
                        <h3>Doplňující úkoly</h3>
                        <TaskEditForm
                            selectedTask={this.state.selectedExtraTask}
                            key={this.state.selectedExtraTask ? this.state.selectedExtraTask.id : "emptyExtra"}
                            onEditSave={this._editExtraTask}
                            onCreateNew={this._createExtraTask}
                        />
                        {
                            this.state.currentChallenge && this.state.currentChallenge.tasks.extra ?
                                <TaskList tasks={this.state.currentChallenge.tasks.extra} onTaskSelected={this._handleExtraTaskSelect}/> : ""
                        }
                    </div>
                    <div className="top-buffer-1">
                        <input type="button" value="Uložit zkoušku" onClick={this._saveChallenge} className="btn btn-success btn-lg btn-block"/>
                    </div>
                </form>
            </div>
        )
    }
}