//$.jqx.theme = 'arctic';
//$.jqx.theme = 'Theme2';

var auto = true;

//******************************************************************************************************//

jQuery.fn.extend({
    form: function () {
        //var save_val = this.serializeArray();
        var save_val = this.find("input,select").serializeArray();

        var arr = new Array();
        $(save_val).each(function (index, element) {
            arr[element.name] = element.value;
        });

        return arr;
    }
});

//******************************************************************************************************//

jQuery.fn.extend({
    post2: function (url, data, success) {
        //var save_val = this.serializeArray();
        var save_val = this.find("input,select").serializeArray();

        var arr = {};
        $(save_val).each(function (index, element) {
            arr[element.name] = element.value;
        });

        for (var p in data) {
            if (data.hasOwnProperty(p)) {
                //console.log(p + " , " + data[p]);

                arr[p] = data[p];
            }
        }

        $("#init").removeClass('no-show');

        setTimeout(function() {
                $.ajax({
                    type: 'POST',
                    contentType: "application/json; charset=utf-8",
                    url: url,
                    data: JSON.stringify(arr),
                    dataType: "json",
                    //async: false,
                    success: success,
                    complete: function() {
                        //$("#init").addClass('no-show');
                    },
                    error: function(data) {
                        try {
                            console.debug("************* Error ***************");
                            console.debug(data);
                        } catch (err) {
                        }

                        if (data.status == 403) {
                            //window.location.href = "/";
                            window.location.href = data.responseJSON.LogOnUrl;
                        }

                        if (data.status == 401) {
                            //window.location.href = "/";
                            window.location.href = ruteLogin;
                        }
                    }
                });

            },30);
    },
    postTrim2: function (url, data, success) {
        //var save_val = this.serializeArray();
        var save_val = this.find("input,select").serializeArray();

        var arr = {};
        $(save_val).each(function (index, element) {
            arr[element.name] = element.value.trim();
        });

        for (var p in data) {
            if (data.hasOwnProperty(p)) {
                //console.log(p + " , " + data[p]);

                arr[p] = data[p];
            }
        }

        $("#init").removeClass('no-show');

        setTimeout(function () {
            $.ajax({
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                url: url,
                data: JSON.stringify(arr),
                dataType: "json",
                //async: false,
                success: success,
                complete: function () {
                    //$("#init").addClass('no-show');
                },
                error: function (data) {
                    try {
                        console.debug("************* Error ***************");
                        console.debug(data);
                    } catch (err) {
                    }

                    if (data.status == 403) {
                        //window.location.href = "/";
                        window.location.href = data.responseJSON.LogOnUrl;
                    }

                    if (data.status == 401) {
                        //window.location.href = "/";
                        window.location.href = ruteLogin;
                    }
                }
            });

        }, 30);
    },
    post1: function (url, success) {

        var save_val = this.find("input,select").serializeArray();

        var arr = {};
        $(save_val).each(function (index, element) {
            arr[element.name] = element.value;
        });

        $("#init").removeClass('no-show');

        setTimeout(function() {
                $.ajax({
                    type: 'POST',
                    contentType: "application/json; charset=utf-8",
                    url: url,
                    data: JSON.stringify(arr),
                    dataType: "json",
                    //async: false,
                    success: success,
                    complete: function() {
                        //$("#init").addClass('no-show');
                    },
                    error: function(data) {
                        try {
                            console.debug("************* Error ***************");
                            console.debug(data);
                        } catch (err) {
                        }

                        if (data.status == 403) {
                            //window.location.href = "/";
                            window.location.href = data.responseJSON.LogOnUrl;
                        }

                        if (data.status == 401) {
                            //window.location.href = "/";
                            window.location.href = ruteLogin;
                        }
                    }
                });
            },30);
    },
    postTrim1: function(url, success) {

        $("#init").removeClass('no-show');

        var save_val = this.find("input,select").serializeArray();

        var arr = {};
        $(save_val).each(function(index, element) {
            arr[element.name] = element.value.trim();
        });

        setTimeout(function() {
                $.ajax({
                    type: 'POST',
                    contentType: "application/json; charset=utf-8",
                    url: url,
                    data: JSON.stringify(arr),
                    dataType: "json",
                    //async: false,
                    success: success,
                    complete: function() {
                        //$("#init").addClass('no-show');
                    },
                    error: function(data) {
                        try {
                            console.debug("************* Error ***************");
                            console.debug(data);
                        } catch (err) {
                        }

                        if (data.status == 403) {
                            //window.location.href = "/";
                            window.location.href = data.responseJSON.LogOnUrl;
                        }

                        if (data.status == 401) {
                            //window.location.href = "/";
                            window.location.href = ruteLogin;
                        }
                    }
                });
            },30);
    }
});
//******************************************************************************************************//

//$("#jqxMenu").jqxMenu({ autoSizeMainItems: false, showTopLevelArrows: true, width: '95%' });
//$("#jqxMenu").css("visibility", "visible");

//******************************************************************************************************//
/*
function getData(cUrl, cData) {
    var result = null;

    $.ajax({
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        url: cUrl,
        data: JSON.stringify(cData),
        dataType: "json",
        async: false,
        success: function (data) {
            try {
                result = JSON.parse(data.d);
            } catch (err) {
                try {
                    result = data.d;
                } catch (err) {
                    result = data;
                }
            }
        },
        error: function (data) {
            try {
                console.debug("************* Error ***************");
                console.debug(data);
            } catch (err) {
            }

            if (data.status == 403) {
                window.location.href = "/";
            }
        }
    });


    return result;
}*/

function isEmpty(value) {
    //return (value == null || value.length === 0);
    return (value == null || $.trim(value) === '');
}

function isValidDate(value) {
    var dateWrapper = new Date(value);
    return !isNaN(dateWrapper.getDate());
}

//******************************************************************************************************//


function getData2(url, data, success) {

    

    $("#init").removeClass('no-show');
    setTimeout(function() {
            $.ajax({
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                url: url,
                data: JSON.stringify(data), 
                dataType: "JSON",
                //async: false,
                success: success,
                beforeSend: function() {
                    
                },
                complete: function() {
                    //$("#init").addClass('no-show');
                },
                error: function(data) {
                    try {
                        console.debug("************* Error ***************");
                        console.debug(data);
                    } catch (err) {
                    }

                    if (data.status == 403) {
                        //window.location.href = "/";
                        window.location.href = data.responseJSON.LogOnUrl;
                    }

                    if (data.status == 401) {
                        //window.location.href = "/";
                        window.location.href = ruteLogin;
                    }
                }
            });
        },30);
}


function getData1(url, success) {
    getData2(url, {}, success);
}


//******************************************************************************************************//

function position(wnd) {
    var windowHeight = ($(window).height() / 2) - ($(wnd).outerHeight() / 2);
    var windowWidth = ($(window).width() / 2) - ($(wnd).outerWidth() / 2);
    if (windowHeight < 0) windowHeight = 5;

    /*                    $('.ui-dialog').css({
    left: windowWidth,
    top: windowHeight
    });*/

    $(wnd).css({
        left: windowWidth,
        top: windowHeight
    });

    

}