import React from 'react';
import PropTypes from 'prop-types';
import {ChallengeShape} from "../proptypesShapes/ChallengeShape";

export class ChallengeListItem extends React.Component {

    static displayName = 'ChallengeListItem';

    static propTypes = {
        challenge: ChallengeShape.isRequired,

        onSelect: PropTypes.func.isRequired,
    };

    _handleSelect = () =>
        this.props.onSelect(this.props.challenge);

    render() {
        const challenge = this.props.challenge;

        return (
            <li key={challenge.id} className="list-group-item btn-block btn" onClick={this._handleSelect}>
                <span>{challenge.id}: {challenge.names.male}/{challenge.names.female} </span>
            </li>
        )
    }
}