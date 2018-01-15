import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import {Challenge} from "./models/Challenge";

import 'bootstrap/dist/css/bootstrap.min.css';
import App from "./App";

fetch("api/challenges").then((resp) => resp.json().then((json) => {
    const challenges = json.map((dto) => Challenge.fromDto(dto));
    ReactDOM.render(<App challenges={challenges}/>, document.getElementById('root'));
}));
