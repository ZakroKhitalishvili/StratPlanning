
var canPing = (function () {

    let lastPing = null;

    return function () {

        if (lastPing == null) {
            lastPing = new Date();
            return true;
        }

        let currentDate = new Date();
        let duration = (currentDate.getTime() - lastPing.getTime()); //milliseconds
        if (duration >= 1 * 60 * 1000) {
            lastPing = currentDate;
            return true;
        }
        return false;
    }
})();

$.sessionTimeout({
    message: 'Your session will be expired in a minute.',
    ignoreUserActivity: false,
    //ajaxType: 'post',
    //keepAliveUrl: '/Auth/Ping',
    keepAlive: false,
    //keepAliveInterval: 1000,
    logoutUrl: '/Auth/Logout',
    redirUrl: '/Auth/Login',
    countdownBar: true,
    onStart: function (options) {
        console.log('onStart');

        if (canPing()) {

            console.log("pinged");
            $.ajax(
                {
                    url: '/Auth/Ping',
                    method: "post",
                    success: function (data, statustext, xhr) {
                        if (xhr.status == 200) {
                            options.redirAfter = data.expirationSeconds * 1000;

                            if (data.expirationSeconds > 60) {
                                options.warnAfter = (data.expirationSeconds - 60) * 1000;
                            }
                            else {
                                options.warnAfter = 0;
                            }

                        }
                        console.log(options);
                    },
                    error: function (xhr, statustext, error) {
                        if (xhr.status == 401) {
                            notify("Authorization failed", "danger", 5);
                        }
                        else {
                            notify("An error occured during ping", "danger", 5);
                        }
                    }
                });
        }

    }

});
