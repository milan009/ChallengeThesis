import React from 'react';
import PropTypes from "prop-types";
import {ChallengeShape} from "../proptypesShapes/ChallengeShape";
import {ChallengeListItem} from "./ChallengeListItem";

export class ChallengeList extends React.Component {

    static propTypes = {
        challenges: PropTypes.arrayOf(ChallengeShape.isRequired).isRequired,

        onChallengeSelected: PropTypes.func.isRequired,
    };

    render() {

        const elements = this.props.challenges.map((ch) => <ChallengeListItem key={ch.id} challenge={ch} onSelect={this.props.onChallengeSelected}/>);

        return (
            <div className="row">
                <h3>Přehled zkoušek</h3>
                <ul className="list-group">
                    {elements}
                </ul>
            </div>
        )
    }
}