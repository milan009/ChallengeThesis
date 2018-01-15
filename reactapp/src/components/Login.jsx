import React from 'react';
import PropTypes from 'prop-types';

export class Login extends React.Component {

    static propTypes = {
        onSubmit: PropTypes.func.isRequired,
    };

    _processLogin = async (event) => {
        event.preventDefault();

        const resp = await fetch("/api/admins/sessions", {
            method: 'POST',
            credentials: 'same-origin',
            redirect: 'follow',
            agent: null,
            headers: {
                "Content-Type": "text/plain",
                'Authorization': 'Basic ' + btoa(this.state.userName + ':' + this.state.password),
            },
            timeout: 5000
        });

        if(!resp.ok) {
            this.setState({error: resp.statusText});
            throw new Error("Yay");
        }

        const sessionGuid = await resp.json();
        console.log(sessionGuid);
        //Promise.resolve("fff97e29-8494-4d40-ba09-11a5249d690c");
        this.props.onSubmit(sessionGuid);
    };

    _onNameEdited = (event) => {
        this.setState({userName: event.target.value});
    };

    _onPasswordEdited = (event) => {
        this.setState({password: event.target.value});
    };

    render() {
        return (
            <div>
                <div className="row form-group"/>
                <div className="row">
                    <div className="col-lg-6 col-lg-offset-3">
                        <h3 className="text-center">Odborky - administrace</h3>
                        <form className="input-group form-control" onSubmit={this._processLogin}>
                            <div>
                                <label className="top-buffer-1">Přihlašovací jméno</label>
                                <input type="text" className="form-control" onChange={this._onNameEdited}/>
                            </div>
                            <div className="top-buffer-1">
                                <label className="top-buffer-1">Heslo</label>
                                <input type="password" className="form-control" onChange={this._onPasswordEdited}/>
                            </div>
                            <button type="submit" className="btn btn-primary form-control top-buffer-1">Přihlásit</button>
                        </form>
                    </div>
                </div>
            </div>
        )
    }
}