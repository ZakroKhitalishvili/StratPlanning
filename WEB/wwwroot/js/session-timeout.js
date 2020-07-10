/**
 * checks whether it's possible to ping to a server 
 * This helps to avoid sending exceeding amount of requests
 * Implemented by a closure
 */
var canPing = (function () {

    let lastPing = null; // initilizes a variable for latest pinging date

    return function () {

        if (lastPing == null) // check if it is a first time call
        {
            lastPing = new Date();
            return true;
        }

        let currentDate = new Date();
        let duration = (currentDate.getTime() - lastPing.getTime()); // milliseconds
        if (duration >= 1 * 60 * 1000) // check if a duration is longer than a minute ( default ping timespan)
        {
            lastPing = currentDate;
            return true;
        }
        return false;
    }
})();

/**
 * initilizes a session timeout controller
 * source: https://github.com/orangehill/bootstrap-session-timeout
 */

$.sessionTimeout({
    message: resource.frontSessionExpirationMinute,
    ignoreUserActivity: false,
    //ajaxType: 'post',
    //keepAliveUrl: '/Auth/Ping',
    keepAlive: false,
    //keepAliveInterval: 1000,
    logoutUrl: '/Auth/Logout',
    redirUrl: '/Auth/Login',
    countdownBar: true,
    onStart: function (options) {
        if (canPing()) {
            // Once A ping was permitted, An ajax call is processed in order to renew session on a server
            // A call returns expirationSeconds that represents remaining seconds of a session
            $.ajax(
                {
                    url: '/Auth/Ping',
                    method: "post",
                    success: function (data, statustext, xhr) {
                        if (xhr.status == 200) {
                            options.redirAfter = data.expirationSeconds * 1000;

                            // after a successfull call session timeout controller's configs are updated
                            if (data.expirationSeconds > 60) {
                                options.warnAfter = (data.expirationSeconds - 60) * 1000;
                            }
                            else {
                                options.warnAfter = 0;
                            }

                        }
                    },
                    error: function (xhr, statustext, error) {
                        if (xhr.status == 401) {
                            notify(resource.frontPingFailed, "danger", 5);
                        }
                        else {
                            notify(resource.frontPingError, "danger", 5);
                        }
                    }
                });
        }

    }

});
