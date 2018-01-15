import React from 'react';
import {ChallengeShape} from "../proptypesShapes/ChallengeShape";

export class Challenge extends React.Component {

    constructor(props) {
        super(props);
    }

    static displayName = 'Challenge';

    static propTypes = {
        challenge: ChallengeShape,
    };

    render() {
        return (
            <div>
                <h1>{this.props.challenge.id}: {this.props.challenge.names.male}</h1>
                <div>{this.props.challenge.description}</div>
                <div>{this.props.challenge.requirements.cubs}</div>
                <div>{this.props.challenge.requirements.scouts}</div>
                <div>{this.props.challenge.requirements.guides}</div>
            </div>
        )
    }
}