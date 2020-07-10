/**
 * @description Initializes inputs (selects, datespickers and so on)
 * @param {any} selector - root element or initializing element dom tree
 */

function initializeInputs(selector) {

    $(selector).find('.dynamic-modal').each(function () {
        var targetId = $(this).attr('data-id');

        if ($('#' + 'targetId').length > 0) {
            $('#' + 'targetId').remove();
        }

        var modalElement = $(this).children().detach();

        $('body').append(modalElement);

        var modalAttrs = {
            id: targetId
        };

        $.each(modalElement[0].attributes, function (idx, attr) {
            modalAttrs[attr.nodeName] = attr.nodeValue;
        });

        modalElement.replaceWith(function () {
            return $("<form />", modalAttrs).append(modalElement.contents());
        });
    });

    $(selector).find('.m-select2').select2();

    $(selector).find(".tag-select").select2({

        tags: true,
        tokenSeparators: [","]
    });

    $(selector).find('.datepicker').datepicker({ format: 'dd-mm-yyyy' });

    $(selector).find('.select2-without-search').select2({
        minimumResultsForSearch: Infinity
    });

    $(selector).find('.evalution-slider').each(function (ind, slider) {
        noUiSlider.create(slider, {
            start: [5],
            step: 1,
            range: {
                min: [1],
                max: [10]
            },
            tooltips: true,
            format: wNumb({
                decimals: 0
            }),
            connect: [true, false]
        });

        slider.noUiSlider.on('change.one', function (value) {
            $(this.target).parent().find('input').val(value);
        });

        var val = $(slider).next('input[type="hidden"]').val();

        if (val) {
            slider.noUiSlider.set(val);
        }
        else {
            slider.noUiSlider.set(5);
            $(slider).next('input[type="hidden"]').val(5);
        }
    });

    if (typeof fieldOptions !== 'undefined') {
        $(selector).find('.m-values-autocomplete').autocomplete({
            minLength: 0,
            source: fieldOptions.valuesDictionary
        }).on('click', function () {
            $(this).autocomplete("search", $(this).val());
        });

        $(selector).find('.m-stakeholders-autocomplete').autocomplete({
            minLength: 0,
            source: fieldOptions.stakeholderUsers,
            change: function (event, ui) {
                if (!ui.item) {
                    $(this).val('');
                    return;
                }

                $(event.target).next("[data-name='stakeholderId']").val(ui.item.id);
            }
        }).on('click', function () {
            $(this).autocomplete("search", $(this).val());
        })
            .on('keyup keydown', function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                }
            });
    }

    $.validator.unobtrusive.parse(selector);

    $(selector).find('.sp-tooltip').tooltip();

    $(selector).find('.m-scrollable').each(function (ind, el) {
        mApp.initScroller($(el), {});
    });

    $(selector).find('.text-editor').summernote();
}


/*
 * read more/less for large texts
 * 
 */

$(document).ready(function () {
    $('.nav-toggle').click(function () {
        var collapse_content_selector = $(this).attr('href');
        var toggle_switch = $(this);
        $(collapse_content_selector).toggle(function () {
            if ($(this).css('display') == 'none') {
                toggle_switch.html('Read More');
            } else {
                toggle_switch.html('Read Less');
            }
        });
    });

});

/*
 *  show/hide button
 */

$(".collapser-text").click(function (e) {
    if ($(this).html() == 'Hide') {
        $(this).html('Show');
    }
    else {
        $(this).html('Hide');
    }
})


$(document).ready(function () {

    initializeInputs(document);

    /**
     * quickSearch API from metronic
     * 
     */
    //var quickSearch = $('#m_quicksearch');

    //quickSearch.mQuicksearch({
    //    type: quickSearch.data('search-type'), // quick search type
    //    source: 'search_url',
    //    spinner: 'm-loader m-loader--skin-light m-loader--right',

    //    input: '#m_quicksearch_input',
    //    iconClose: '#m_quicksearch_close',
    //    iconCancel: '#m_quicksearch_cancel',
    //    iconSearch: '#m_quicksearch_search',

    //    hasResultClass: 'm-list-search--has-result',
    //    minLength: 1,
    //    templates: {
    //        error: function (qs) {
    //            return '<div class="m-search-results m-search-results--skin-light"><span class="m-search-result__message">Something went wrong</div></div>';
    //        }
    //    }
    //});

    /**
     * Datatable ordering functions
     * 
     */

    $.fn.dataTable.ext.order['dom-select'] = function (settings, col) {
        return this.api().column(col, { order: 'index' }).nodes().map(function (td, i) {
            return $('select', td).val();
        });
    };

    $.fn.dataTable.ext.order['dom-input'] = function (settings, col) {
        return this.api().column(col, { order: 'index' }).nodes().map(function (td, i) {
            return $('input', td).val();
        });
    };

    $.fn.dataTable.ext.order['dom-input-number'] = function (settings, col) {
        return this.api().column(col, { order: 'index' }).nodes().map(function (td, i) {
            return parseFloat($('input', td).val());
        });
    };

});

/**
 * 
 *  delete confirm alert
 */

function deleteConfirm() {
    return swal({
        icon: "warning",
        title: resource.frontDeleteConfirm,
        buttons:
        {
            confirm:
            {
                text: resource.frontYes,
                value: true,
                visible: true,
                className: "btn btn-sp m-btn",
                closeModal: true,
            },
            cancel:
            {
                text: resource.frontNo,
                value: false,
                visible: true,
                className: "btn btn-secondary m-btn",
                closeModal: true,
            }
        }
    }
    );
}

/**
 * 
 *  submit confirm alert
 */

function submitConfirm(text) {
    return swal({
        icon: "warning",
        title: resource.frontSubmitConfirm,
        text: text,
        buttons:
        {
            confirm:
            {
                text: resource.frontYes,
                value: true,
                visible: true,
                className: "btn btn-sp m-btn",
                closeModal: true,
            },
            cancel:
            {
                text: resource.frontNo,
                value: false,
                visible: true,
                className: "btn btn-secondary m-btn",
                closeModal: true,
            }
        }
    }
    );
}

/**
 * @description shows a notification.
 * 
 * @param {string} text -  displaying text on notification
 * @param {string} type - derermines color : "success", "danger", "warning" 
 * @param {number} seconds - duration
 */

function notify(text, type, seconds = 5) {
    $.notify({
        // options
        message: text
    },
        {
            // settings
            type: type,
            delay: seconds * 1000,
            placement: {
                from: "bottom",
                align: "right"
            },
            offset:
            {
                x: 20,
                y: 100
            },
            z_index: 5000,
        });
}

/**
 * submit event based plan creation ajax
 */

$(document).on('submit', 'form#add_plan_form', function (e) {
    e.preventDefault();

    // validates a form
    if (!$('#add_plan_form').valid()) {
        return;
    }

    // takes valued from a form
    let formData = new FormData(document.querySelector('form#add_plan_form'));

    $.ajax(
        {
            url: CreatePlanURL,
            method: "post",
            data: formData,
            processData: false,
            contentType: false,
            success: function (data, statusText, xhr) {
                if (xhr.status == 201) {
                    notify(resource.frontCreateSuccess, "success", 5);
                    setTimeout(function () {
                        document.location.reload(false)
                    }, 2000);
                }
                if (xhr.status == 202 || xhr.status == 400) {
                    notify(resource.frontInvalidInputData, "danger", 5);
                }

                $('#add_plan_form').html(data);
                initializeInputs('#add_plan_form');
            },
            error: function (xhr, statusText, error) {
                notify(resource.frontRequestError, "danger", 5);
            }
        });
});

/**
 *  
 * plan delete ajax triggered by a button click
 */

$(document).on('click', '.delete-plan', function (e) {
    e.preventDefault();

    let planId = $(this).data('id');

    //delete ajax is called after a confirm
    deleteConfirm().then(result => {
        if (result) {
            $.ajax(
                {
                    url: DeletePlanURL,
                    method: "post",
                    data: {
                        planId
                    },
                    success: function (data, statusText, xhr) {
                        if (xhr.status == 400) {
                            notify(resource.frontRequestError, "danger", 5);

                        }

                        if (data.result) {
                            notify(resource.frontDeleteSuccess, "success", 5);
                            setTimeout(function () {
                                document.location.reload(false)
                            }, 2000);

                        }
                        else {
                            notify(resource.frontPlanDeleteError, "danger", 5);
                        }

                    },
                    error: function (xhr, statusText, error) {
                        notify(resource.frontRequestError, "danger", 5);
                    }
                })
        }
    })

});

/** ==========================
 * Dictionary management
 * =========================
 */

/**
 * submit event based dictionary creation ajax
 * 
 */

$(document).on('submit', 'form#add_dictionary_form', function (e) {
    e.preventDefault();

    if (!$('#add_dictionary_form').valid()) {
        return;
    }

    let formData = new FormData(document.querySelector('form#add_dictionary_form'));

    $.ajax(
        {
            url: CreateDictionaryURL,
            method: "post",
            data: formData,
            processData: false,
            contentType: false,
            success: function (data, statusText, xhr) {
                if (xhr.status == 201) {
                    notify(resource.frontCreateSuccess, "success", 5);
                    setTimeout(function () {
                        document.location.reload(false)
                    }, 2000);
                }
                if (xhr.status == 202 || xhr.status == 400) {
                    notify(resource.frontInvalidInputData, "danger", 5);
                }

                $('#add_dictionary_form').html(data);
                initializeInputs('#add_dictionary_form');
            },
            error: function (xhr, statusText, error) {
                notify(resource.frontRequestError, "danger", 5);
            }
        });
});

/**
 * dictionary delete ajax triggered by a button click
 * 
 */

$(document).on('click', '.delete-dictionary', function (e) {
    e.preventDefault();

    let id = $(this).data('id');

    deleteConfirm().then(result => {
        if (result) {
            $.ajax(
                {
                    url: DeleteDictionaryURL,
                    method: "post",
                    data: {
                        id
                    },
                    success: function (data, statusText, xhr) {
                        if (xhr.status == 400) {
                            notify(resource.frontRequestError, "danger", 5);

                        }

                        if (data.result) {
                            notify(resource.frontDeleteSuccess, "success", 5);
                            setTimeout(function () {
                                document.location.reload(false)
                            }, 2000);

                        }
                        else {
                            notify(resource.frontDeleteImpossible, "danger", 5);
                        }

                    },
                    error: function (xhr, statusText, error) {
                        notify(resource.frontRequestError, "danger", 5);
                    }
                })
        }
    })

});

/**
 * dictionary activation ajax triggered by a button click
 * 
 */
$(document).on('click', '.activate-dictionary', function (e) {
    e.preventDefault();

    let id = $(this).data('id');

    deleteConfirm().then(result => {
        if (result) {
            $.ajax(
                {
                    url: ActivateDictionaryURL,
                    method: "post",
                    data: {
                        id
                    },
                    success: function (data, statusText, xhr) {
                        if (xhr.status == 400) {
                            notify(resource.frontRequestError, "danger", 5);

                        }

                        if (data.result) {
                            notify(resource.frontActivateSuccess, "success", 5);
                            setTimeout(function () {
                                document.location.reload(false)
                            }, 2000);

                        }
                        else {
                            notify(resource.frontActivateImpossible, "danger", 5);
                        }

                    },
                    error: function (xhr, statusText, error) {
                        notify(resource.frontRequestError, "danger", 5);
                    }
                })
        }
    })

});

/**
 * dictionary deactivation ajax triggered by a button click
 * 
 */

$(document).on('click', '.disactivate-dictionary', function (e) {
    e.preventDefault();

    let id = $(this).data('id');

    deleteConfirm().then(result => {
        if (result) {
            $.ajax(
                {
                    url: DisactivateDictionaryURL,
                    method: "post",
                    data: {
                        id
                    },
                    success: function (data, statusText, xhr) {
                        if (xhr.status == 400) {
                            notify(resource.frontRequestError, "danger", 5);

                        }

                        if (data.result) {
                            notify(resource.frontDeactivateSuccess, "success", 5);
                            setTimeout(function () {
                                document.location.reload(false)
                            }, 2000);

                        }
                        else {
                            notify(resource.frontDeactivateImpossible, "danger", 5);
                        }

                    },
                    error: function (xhr, statusText, error) {
                        notify(resource.frontRequestError, "danger", 5);
                    }
                })
        }
    })

});

/** ==========================
 * Setting management
 * =========================
 */

/**
 * an ajax call that gets editing setting form triggered by a button click
 * 
 */
$(document).on('click', '.edit-setting', function (e) {
    e.preventDefault();

    let index = $(this).data('index');

    $.ajax(
        {
            url: UpdateSettingURL,
            method: "get",
            data: {
                index
            },
            success: function (data, statusText, xhr) {
                if (xhr.status == 200) {
                    $('#edit_setting_form').html(data);
                    initializeInputs('#edit_setting_form');
                    $('#edit_setting_modal').modal('show')
                }

            },
            error: function (xhr, statusText, error) {
                notify(resource.frontRequestError, "danger", 5);
            }
        })
});

/**
 * submit event based ajax call for a setting update
 * 
 */

$(document).on('submit', 'form#edit_setting_form', function (e) {
    e.preventDefault();

    let formData = new FormData(document.querySelector('form#edit_setting_form'));

    $.ajax(
        {
            url: UpdateSettingURL,
            method: "post",
            data: formData,
            processData: false,
            contentType: false,
            success: function (data, statusText, xhr) {
                if (xhr.status == 200) {
                    notify(resource.frontUpdateSuccess, "success", 5);

                    setTimeout(function () {
                        document.location.reload(false)
                    }, 2000);
                }

                $('form#edit_setting_form').html(data);

                initializeInputs('form#edit_setting_form');

            },
            error: function (xhr, statusText, error) {
                notify(resource.frontRequestError, "danger", 5);
            }
        })

});

/** ==========================
 * User management
 * =========================
 */


/**
 * initilizes inputs on showing user add modal
 * 
 */

$('#add_user_modal').on('shown.bs.modal', function () {
    initializeInputs($(this));
})

/**
 * submit event based ajax call for an user creation
 */
$(document).on('submit', 'form#add_user_form', function (e) {
    e.preventDefault();

    if (!$('#add_user_form').valid()) {
        return;
    }

    let formData = new FormData(document.querySelector('form#add_user_form'));

    $.ajax(
        {
            url: AddNewUserURL,
            method: "post",
            data: formData,
            processData: false,
            contentType: false,
            success: function (data, statusText, xhr) {
                if (xhr.status == 201) {
                    notify(resource.frontCreateSuccess, "success", 5);
                    setTimeout(function () {
                        document.location.reload(false)
                    }, 2000);
                }
                if (xhr.status == 202 || xhr.status == 400) {
                    notify(resource.frontInvalidInputData, "danger", 5);
                }

                $('#add_user_form').html(data);
                initializeInputs('#add_user_form');
            },
            error: function (xhr, statusText, error) {
                notify(resource.frontRequestError, "danger", 5);
            }
        });
});

/**
 * 
 * An ajax call for getting an user update form triggered by a button click
 */
$(document).on('click', '.edit-user', function (e) {
    e.preventDefault();

    let id = $(this).data('id');

    $.ajax(
        {
            url: UserEditUrl,
            method: "get",
            data: {
                id
            },
            success: function (data, statusText, xhr) {
                if (xhr.status == 200) {
                    $('#edit_user_form').html(data);
                    initializeInputs('#edit_user_form');
                    $('#edit_user_modal').modal('show')
                }

            },
            error: function (xhr, statusText, error) {
                notify(resource.frontRequestError, "danger", 5);
            }
        })
});

/**
 * submit event based an user update ajax call
 * 
 */
$(document).on('submit', 'form#edit_user_form', function (e) {
    e.preventDefault();

    let formData = new FormData(document.querySelector('form#edit_user_form'));

    $.ajax(
        {
            url: UserEditUrl,
            method: "post",
            data: formData,
            processData: false,
            contentType: false,
            success: function (data, statusText, xhr) {
                if (xhr.status == 200) {
                    notify(resource.frontUpdateSuccess, "success", 5);

                    setTimeout(function () {
                        document.location.reload(false)
                    }, 2000);
                }

                $('form#edit_user_form').html(data);

                initializeInputs('form#edit_user_form');

            },
            error: function (xhr, statusText, error) {
                notify(resource.frontRequestError, "danger", 5);
            }
        })

});

/**
 * User new password geenration ajax call triggered by a button click
 * 
 */
$(document).on('click', '.generate-user-password', function (e) {
    e.preventDefault();

    let id = $(this).data('id');

    submitConfirm(resource.frontUserPasswordGenerateConfirm).then(function (result) {
        if (result) {
            $.ajax(
                {
                    url: GeneratePasswordURL,
                    method: "post",
                    data: {
                        id
                    },
                    success: function (data, statusText, xhr) {
                        if (data.result) {
                            notify(resource.frontPasswordGenerateSuccess, "success", 5);
                        }
                        else {
                            notify(resource.frontPasswordGenerateFailed, "danger", 5);
                        }

                    },
                    error: function (xhr, statusText, error) {
                        notify(resource.frontRequestError, "danger", 5);
                    }
                });
        }
    });

});

/**
 * User delete ajax call triggered by a button click
 * 
 */
$(document).on('click', '.delete-user', function (e) {
    e.preventDefault();

    let id = $(this).data('id');

    deleteConfirm().then(result => {
        if (result) {
            $.ajax(
                {
                    url: DeleteUserURL,
                    method: "post",
                    data: {
                        id
                    },
                    success: function (data, statusText, xhr) {
                        if (xhr.status == 400) {
                            notify(resource.frontRequestError, "danger", 5);

                        }

                        if (data.result) {
                            notify(front.frontDeleteSuccess, "success", 5);
                            setTimeout(function () {
                                document.location.reload(false)
                            }, 2000);

                        }
                        else {
                            notify(front.frontDeleteImpossible, "danger", 5);
                        }

                    },
                    error: function (xhr, statusText, error) {
                        notify(resource.frontRequestError, "danger", 5);
                    }
                })
        }
    })

});

/**
 * User activating ajax call triggered by a button click
 * 
 */
$(document).on('click', '.activate-user', function (e) {
    e.preventDefault();

    let id = $(this).data('id');

    deleteConfirm().then(result => {
        if (result) {
            $.ajax(
                {
                    url: ActivateUserURL,
                    method: "post",
                    data: {
                        id
                    },
                    success: function (data, statusText, xhr) {
                        if (xhr.status == 400) {
                            notify(resource.frontRequestError, "danger", 5);

                        }

                        if (data.result) {
                            notify(resource.frontActivateSuccess, "success", 5);
                            setTimeout(function () {
                                document.location.reload(false)
                            }, 2000);

                        }
                        else {
                            notify(resource.frontActivateImpossible, "danger", 5);
                        }

                    },
                    error: function (xhr, statusText, error) {
                        notify(resource.frontRequestError, "danger", 5);
                    }
                })
        }
    })

});

/**
 * User deactivating ajax call triggered by a button click
 * 
 */

$(document).on('click', '.disactivate-user', function (e) {
    e.preventDefault();

    let id = $(this).data('id');

    deleteConfirm().then(result => {
        if (result) {
            $.ajax(
                {
                    url: DisactivateUserURL,
                    method: "post",
                    data: {
                        id
                    },
                    success: function (data, statusText, xhr) {
                        if (xhr.status == 400) {
                            notify(resource.frontRequestError, "danger", 5);

                        }

                        if (data.result) {
                            notify(resource.frontDeactivateSuccess, "success", 5);
                            setTimeout(function () {
                                document.location.reload(false)
                            }, 2000);

                        }
                        else {
                            notify(resource.frontDeactivateImpossible, "danger", 5);
                        }

                    },
                    error: function (xhr, statusText, error) {
                        notify(resource.frontRequestError, "danger", 5);
                    }
                })
        }
    })

});