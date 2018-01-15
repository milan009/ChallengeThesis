import React, { Component } from 'react';
import './App.css';
import {ChallengeList} from "./components/ChallengeList";
import {Login} from "./components/Login";
import {ChallengeEditForm} from "./components/ChallengeEditForm";
import scrollToElement from 'scroll-to-element';
import * as Pages from './utils/pageNames.js';

class App extends Component {

constructor(props) {
    super(props);
    this.state = {
        isLoggedIn: false,
        page: Pages.MENU,
    };
}

_handleSubmit = (loginGuid) => {
    this.setState(() => ({
        isLoggedIn: true,
        loginGuid
    }));
};

_handleEditorToggle = () => {
    this.setState({page: Pages.NEW});
};

_handleListToggle = () => {
    this.setState({page: Pages.LIST});
};

_handleBack = () => {
    this.setState({page: Pages.MENU});
};

_handleChallengeSelected = (challenge) => {
  scrollToElement("#header");
  this.setState({
      selectedChallenge: challenge,
      page: Pages.EDIT,
  });
};

_saveChallenge = async (challenge) => {
    const resp = await fetch("/api/challenges/" + challenge.Id, {
        method: 'PUT',
        redirect: 'follow',
        agent: null,
        headers: {
            "Content-Type" : "application/json",
            "AdminGuid" : this.state.loginGuid,
        },
        body: JSON.stringify(challenge),
        timeout: 5000
    });

    if(!resp.ok) {
        this.setState({error: resp.statusText});
        throw new Error(resp.statusText);
    }

    const res = await resp.json();
};

_createChallenge = async (challenge) => {
    const resp = await fetch("/api/challenges/", {
        method: 'POST',
        redirect: 'follow',
        agent: null,
        headers: {
            "Content-Type" : "application/json",
            "AdminGuid" : this.state.loginGuid,
        },
        body: JSON.stringify(challenge),
        timeout: 5000
    });

    if(!resp.ok) {
        this.setState({error: resp.statusText});
        throw new Error(resp.statusText);
    }

    const res = await resp.json();
};

  render() {

    let content;
    switch(this.state.page)
    {
        case Pages.MENU: {
            content =
                <div>
                    <div className="top-buffer-1">
                        <input type="button" value="Vytvořit novou zkoušku" onClick={this._handleEditorToggle} className="btn btn-primary btn-lg btn-block"/>
                    </div>
                    <div className="top-buffer-1">
                        <input type="button" value="Editovat existující zkoušku" onClick={this._handleListToggle} className="btn btn-primary btn-lg btn-block"/>
                    </div>
                </div>;
            break;
        }

        case Pages.LIST: {
            content =
                <div>
                    <ChallengeList challenges={this.props.challenges} onChallengeSelected={this._handleChallengeSelected}/>
                </div>;
            break;
        }

        case Pages.EDIT: {
            content =
                <div>
                    <ChallengeEditForm selectedChallenge={this.state.selectedChallenge} onSaveChallenge={this._saveChallenge} edit={true}/>
                </div>;
            break;
        }

        case Pages.NEW: {
            content =
                <div>
                    <ChallengeEditForm onSaveChallenge={this._createChallenge} edit={false}/>
                </div>;
            break;
        }

    }

    if(!this.state.isLoggedIn) {
        content = <Login onSubmit={this._handleSubmit}/>;
    }


    return (
        <div>
            <div className="App-header">
                <header className="" id="header">
                    <h1 className="App-title">Odborky online - administrace</h1>
                </header>
            </div>
            <div className="container col-lg-4 col-lg-offset-4">
                <div className="row top-buffer-1">
                    {content}
                </div>
                {
                    this.state.page !== Pages.MENU ?
                        <div className="row top-buffer-1">
                            <input type="button" value="Zpět do hlavní nabídky" onClick={this._handleBack} className="btn btn-primary btn-lg btn-block"/>
                        </div> : ""
                }

            </div>
        </div>
    );
  }
}

export default App;
