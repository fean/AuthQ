﻿window.AuthiQ = {
    _appID: '',
    _initDone: false,
    _tokenAvailable: false,
    _data: undefined,
    _retval: [false],

    onfailure: undefined,
    onsuccess: undefined,

    init: function (trustid) {
        this._appID = trustid;
        this._initDone = true;
        if (localStorage.getItem(trustid + '_token') != null)
            this._tokenAvailable = true;
    },

    createLogin: function () {
        if (!this._initDone)
            return null;
        var req = new XMLHttpRequest();
        if (req) {
            if (!this._tokenAvailable) {
                try {
                    req.open("GET", 'https://authiq.org/oauth/requesttoken?trustid=' + this._appID, true);
                    req.onreadystatechange = function () {
                        if (req.readyState == 4) {
                            if (req.status == 200) {
                                var response = JSON.parse(req.responseText);
                                if (!response.Success)
                                    return AuthiQ._fireEvent('failure');
                                var url = 'https://authiq.org/Authentication/Login?requesttoken=' + response.RequestToken + '&trustid=' + AuthiQ._appID;
                                var left = (screen.width - 600) / 2;
                                var top = (screen.height - 500) / 2;
                                var win = window.open(url, 'Login to your domain.',
                                    'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=no, copyhistory=no, width=600, height=500, top=' + top + ', left=' + left);
                                var t = setInterval(function () {
                                    if (win.closed) {
                                        if (AuthiQ._retval[0]) {
                                            AuthiQ._data = JSON.parse(AuthiQ._retval[1]);
                                            AuthiQ._retval = [false];
                                            localStorage.setItem(AuthiQ._appID + '_token', AuthiQ._data.AccessToken);
                                            localStorage.setItem(AuthiQ._appID + '_rtoken', AuthiQ._data.RefreshToken);
                                            localStorage.setItem(AuthiQ._appID + '_expire', new Date((new Date).getTime() + AuthiQ._data.Expires * 1000).toUTCString());
                                            clearInterval(t);
                                            return AuthiQ._fireEvent('success', AuthiQ._data);
                                        } else {
                                            clearInterval(t);
                                            return AuthiQ._fireEvent('failure');
                                        }
                                    }
                                }, 50);
                                var eventMethod = window.addEventListener ? 'addEventListener' : 'attachEvent';
                                var eventer = window[eventMethod];
                                var messageEvent = eventMethod == 'attachEvent' ? 'onmessage' : 'message';
                                eventer(messageEvent, function (e) {
                                    setReturnValue(e.data);
                                }, false);
                            } else {
                                return AuthiQ._fireEvent('failure');
                            }
                        }
                    };
                    req.send();
                } catch (e) {
                    console.log('Error: Something went wrong whilst starting the process. Are you sure you authorized the current domain?');
                    return AuthiQ._fireEvent('failure');
                }
            } else {
                try {
                    if (new Date() > new Date(localStorage.getItem(this._appID + '_expire'))) {
                        req.open('GET', 'https://authiq.org/oauth/checktoken?type=auth&token=' + localStorage.getItem(this._appID + '_token') + '&trustid=' + this._appID, true);
                    } else {
                        req.open('GET', 'https://authiq.org/oauth/refreshtoken?refreshToken=' + localStorage.getItem(this._appID + '_rtoken') + '&trustid=' + this._appID, true);
                    }
                    req.onreadystatechange = function () {
                        if (req.readyState == 4) {
                            if (req.status == 200) {
                                var response = JSON.parse(req.responseText);
                                if (!response.Success) {
                                    AuthiQ._tokenAvailable = false;
                                    return AuthiQ.createLogin();
                                }
                                AuthiQ._data = {
                                    AccessToken: response.AccessToken,
                                    RefreshToken: response.RefreshToken,
                                    Expires: response.Expires
                                };
                                localStorage.setItem(AuthiQ._appID + '_token', AuthiQ._data.AccessToken);
                                localStorage.setItem(AuthiQ._appID + '_rtoken', AuthiQ._data.RefreshToken);
                                localStorage.setItem(AuthiQ._appID + '_expire', new Date((new Date).getTime() + AuthiQ._data.Expires * 1000).toUTCString());
                                return AuthiQ._fireEvent('success', AuthiQ._data);
                            }
                        }
                    };
                    req.send();
                } catch (e) {
                    console.log('Error: Something went wrong whilst logging on. Are you sure you authorized the current domain?');
                    return AuthiQ._fireEvent('failure');
                }
            }
        }
    },

    _fireEvent: function (event, state) {
        if (event.toLowerCase() == 'failure') {
            if (!this.onfailure)
                return null;
            state ? this.onfailure(state) : this.onfailure();
        } else if (event.toLowerCase() == 'success') {
            if (!this.onsuccess)
                return null;
            state ? this.onsuccess(state) : this.onsuccess();
        }
    },

    destroyLogin: function () {
        var req = new XMLHttpRequest();
        if (req) {
            req.open('POST', 'https://authiq.org/oauth/unauthorize', false);
            req.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
            req.send('accessToken=' + AuthiQ._data.AccessToken);
            
            if (JSON.parse(req.responseText).Success) {
                this._data = undefined;
                localStorage.removeItem(AuthiQ._appID + '_token');
                localStorage.removeItem(AuthiQ._appID + '_rtoken');
                localStorage.removeItem(AuthiQ._appID + '_expire');
                return true;
            }
        }
        return false;
    },

    isAuthorized: function () {
        return this._data ? true : false;
    },

    getAuthResponse: function () {
        return this._data ? this._data : null;
    },

    api: {
        user: {
            getProfile: function () {
                if (AuthiQ.isAuthorized) {
                    var req = new XMLHttpRequest();
                    if (req) {
                        req.open('POST', 'https://authiq.org/Profile', false);
                        req.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
                        req.send('accessToken=' + AuthiQ._data.AccessToken );
                        if (JSON.parse(req.responseText).Success) {
                            return JSON.parse(req.responseText);
                        }
                    }
                }
                return null;
            },
            
            getName: function() {
                var r = AuthiQ.api.user.getProfile();
                return r.FullName == null ? r.Username : r.FullName;
            },
            
            getMailAddress: function() {
                return AuthiQ.api.getProfile().MailAddress;
            }
        }
    }
};
window.setReturnValue = function (e) {
    AuthiQ._retval = e;
};