﻿/**
 * calls a step initializer 
 */
$(document).ready(function () {
    initializeStep();
});

/**
 * @description initilizes a step for all types of questions
 **/
function initializeStep() {
    $('.stakeholders-rating-table .m-select2').select2({
        minimumResultsForSearch: Infinity
    });

    $('.sp-tooltip').tooltip();

    /**
     * Master list table initilized with JQuery datatable
     * source: https://datatables.net/
     * 
     */
    $('#issues-master-list-table').dataTable(
        {
            // rowReorder:
            // {
            //     selector: 'td:nth-child(1)'
            // },
            paging: false,
            serverSide: false,
            searching: false,
            info: false,
            ordering: true,
            columnDefs:
                [{ visible: false, targets: [0] },
                { orderable: false, targets: [1, 2, 3, 4, 5] },
                { orderDataType: 'dom-input-number', targets: [6] },
                { type: 'numeric', targets: [6] }
                ],
        }
    );

    /**
     * Issue master list ordering by a ranking column triggerred by a blur and an enter button press
     */
    $('#issues-master-list-table tr td:nth-child(6) input').blur(function (e) {
        $('#issues-master-list-table').DataTable().column(6).order('desc').draw();
    })
        .on('keyup', function (e) {
            if (e.keyCode == 13)//enter button
            {
                $('#issues-master-list-table').DataTable().column(6).order('desc').draw();
            }
        });

    /**
     * Change event control for a select type question with an option of specific answer
     * After attaching a handler it trigger a change event in order to show specific answer for received step form
     */
    $('select.select-specify').on('change', function (e) {
        let value = $(this).val();
        let specifyInput = $(this).parent().find('input.select-specify-input');

        if (value.toLowerCase() == '-1') {
            specifyInput.show();
        }
        else {
            specifyInput.hide();
        }
    }).trigger('change');

    /**
     * Initializes dropzone for file uplodings
     * source: https://www.dropzonejs.com
     */
    $('.dropzone-sp').each(function () {
        var element = $(this);
        // initiliizes uploaded file url
        var initurl = element.data('initurl');
        var inputname = element.data('inputname');

        element.dropzone({
            init: function () {
                thisDropzone = this;

                if (!initurl) return;

                //fetchs file data
                $.get(initurl, function (data) {
                    if (data === null) {
                        return;
                    }

                    $.each(data, function (key, value) {
                        var mockFile = { name: value.name, fileId: value.id };

                        thisDropzone.emit("addedfile", mockFile);

                        $(mockFile.previewElement).find(".sp-file-input").val(value.id);
                        $(mockFile.previewElement).find(".sp-file-link").attr('href', value.url);
                    });
                });

            },
            addRemoveLinks: true,
            removedfile: function (file) {
                // file remove event
                let currentPreviewElement = $(file.previewElement);
                let id = $(currentPreviewElement).find(".sp-file-input").val();

                //file remove confirm is display and after a confirmation processes an ajax call to delete file on a server
                removeFilePreview(currentPreviewElement).then(function (result) {
                    if (result) {
                        if (!!id) {
                            $.ajax(
                                {
                                    url: '/Worksheet/DeleteFile',
                                    method: "post",
                                    data: { id },
                                    success: function (data, statusText, xhr) {
                                        if (xhr.status == 201) {

                                        }
                                        if (xhr.status == 202 || xhr.status == 400) {
                                            notify(resource.frontInvalidInputData, "danger", 5);
                                        }
                                    },
                                    error: function (xhr, statusText, error) {
                                        notify(resource.frontRequestError, "danger", 5);
                                    }
                                });
                        }
                    }
                });

            },
            url: "/Worksheet/UploadFile", // On this url files are uploaded
            uploadMultiple: true,
            paramName: 'file',
            success: function (file, response) {
                //The function is called after successfull upload
                var index = 0;
                var fileResponse = response.filter(function (data, i) {
                    if (data.name === file.name) {
                        index = i;
                        return true;
                    }
                    return false;
                })[0];

                $(file.previewElement).find(".sp-file-input").val(fileResponse.id);
                $(file.previewElement).find(".sp-file-link").attr('href', fileResponse.url);
            },
            error: function (file, response) {
                //The function is called after unsuccessfull upload
                $(file.previewElement).find(".dz-image").addClass('error-file');
                $(file.previewElement).find(".dz-error-message").show();
                if (typeof (response.message) !== 'undefined') {
                    $(file.previewElement).find(".dz-error-message > span").html(response.message);
                }
                else {
                    $(file.previewElement).find(".dz-error-message > span").html('An error occured on a server');
                }
                $(file.previewElement).find(".sp-file-input").remove();
            },
            previewTemplate: `
                <div class="dz-preview dz-file-preview dz-processing dz-error dz-complete">
                    <input class="sp-file-input" type="hidden" name="${inputname}"/>
                    <div class="dz-image"></div>  
                    <div class="dz-details">
                        <div class="dz-filename">
                            <a class="sp-file-link" href="#" target="_Blank"><span data-dz-name=""></span></a>
                        </div>
                    </div>  
                    <div class="dz-progress">
                        <span class="dz-upload" data-dz-uploadprogress=""></span>
                    </div>
                    <div class="dz-error-message" style="display:none">
                        <span class="dz-upload" data-dz-errormessage=""></span>
                    </div>
                    </div>
                    <a class="dz-remove" href="javascript:undefined;" data-dz-remove="">${resource.frontRemoveFile}</a>
                </div>`
        }).addClass('dropzone');
    });

    /**
     * @description dropzone file previews remove relating functions
     * @param {any} e
     */

    function filePreviewRemoveHandler(e) {
        removeFilePreview($(e.target).parent());
    }

    function removeFilePreview(filePreview) {
        let parentDropzone = $(filePreview).parent();
        parentDropzone.find('.dz-message').hide();
        return deleteConfirm().then(function (isConfirm) {

            if (isConfirm) {
                filePreview.remove();

            }

            if (parentDropzone.find('.dz-preview').length == 0) {
                parentDropzone.find('.dz-message').show();
            }

            return isConfirm;

        });
    }

    /**
     * SWOT Table items delete handler
     * 
     */
    $(document).on('click', '.list-item-delete', function (e) {
        let listItem = $(this).closest('.list-item');
        listItem.remove();
    });

    /**
   * SWOT Table items adding handler
   * 
   */

    $(document).on('click', '.list-add-button', function (e) {
        e.preventDefault();
        let inputName = $(this).data('input-name');
        let value = $(this).parents('.list-items-add-group').first().find('.list-add-input').val();
        let val = $(this).parents('.list-items-add-group').first().find('.list-items').find('.list-item').has(`input[name="${inputName}"][value="${value}"]`);
        if (!!value) {
            let html = `<a class="m-list-badge__item m-list-badge__item--default list-item">
                            <input type="hidden" name="${inputName}" value="${value}"/>
                            ${value}
                            <span class="list-item-delete fa fa-close"></span>
                        </a>`;

            $(this).parents('.list-items-add-group').first().find('.list-items').append(html);
            $(this).parents('.list-items-add-group').first().find('.list-add-input').val('')
        }
    });

    /**
   * SWOT Table items add handler triggered by an enter button press
   * 
   */
    $(document).on('keyup', '.list-add-input', function (e) {
        if (e.which == 13) {
            e.preventDefault();
            //this triggers item add button change event
            $(this).parents('.list-items-add-group').first().find('.list-add-button').click();
        }
    })
}

/**
 * New user adding to plan - submit based
 * 
 */
$(document).on('submit', "form#add_user_to_plan_new", function (e) {
    e.preventDefault();

    let formData = new FormData(document.querySelector('form#add_user_to_plan_new'));

    formData.set('PlanId', GlobalPlanId);

    $.ajax(
        {
            url: AddNewUserToPlanURL,
            method: "post",
            data: formData,
            processData: false,
            contentType: false,
            success: function (data, statusText, xhr) {
                if (xhr.status == 201) {
                    notify(resource.frontAddSuccess, "success", 5);
                    $('#planning_team_add_modal').modal('hide');
                    $('body').removeClass('modal-open');
                    $('.modal-backdrop').remove();
                    updatePlanningTeam();
                    refreshStepForm(true);

                }
                if (xhr.status == 400) {
                    notify(resource.frontRequestError, "danger", 5);
                }

                $('form#add_user_to_plan_new').html(data);

                $('form#add_user_to_plan_new').find('.m-select2').select2();

                $.validator.unobtrusive.parse('form#add_user_to_plan_new');
            },
            error: function (xhr, statusText, error) {
                notify(resource.frontRequestError, "danger", 5);
            }
        })
});
/**
 * Existing user adding to plan - submit based
 * 
 */
$(document).on('submit', "form#add_user_to_plan_existing", function (e) {
    e.preventDefault();
    let formData = new FormData(document.querySelector('form#add_user_to_plan_existing'));
    formData.set('PlanId', GlobalPlanId);

    $.ajax(
        {
            url: AddExistingUserToPlanURL,
            method: "post",
            data: formData,
            processData: false,
            contentType: false,
            success: function (data, statusText, xhr) {
                if (xhr.status == 201) {
                    notify(resource.frontAddSuccess, "success", 5);
                    $('#planning_team_add_modal').modal('hide');
                    $('body').removeClass('modal-open');
                    $('.modal-backdrop').remove();
                    updatePlanningTeam();
                    refreshStepForm(true);
                }

                if (xhr.status == 400) {
                    notify(resource.frontRequestError, "danger", 5);
                }

                $('form#add_user_to_plan_existing').html(data);
                $('form#add_user_to_plan_existing').find('.m-select2').select2();
                $.validator.unobtrusive.parse('form#add_user_to_plan_existing');
            },
            error: function (xhr, statusText, error) {
                notify(resource.frontRequestError, "danger", 5);
            }
        })
});

/**
 *  User remove from a plan triggered by a button click
 * 
 */

$(document).on('click', '.remove-user-from-plan', function (e) {
    e.preventDefault();

    let userId = $(this).data('id');

    deleteConfirm().then(result => {
        if (result) {
            $.ajax(
                {
                    url: RemoveUserFromPlanURL,
                    method: "post",
                    data: {
                        userId: userId,
                        planId: GlobalPlanId
                    },
                    success: function (data, statusText, xhr) {
                        if (xhr.status == 400) {
                            notify(resource.frontRequestError, "danger", 5);
                        }

                        if (data.result) {
                            notify(resource.frontDeleteSuccess, "success", 5);
                            updatePlanningTeam();
                            refreshStepForm(true);
                        }
                        else {
                            notify(resource.frontRemovingUsersImpossibleDueToAnswers, "danger", 5);
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
 *  @description updates planning team table - fetches using an ajax call
 * 
 */
function updatePlanningTeam() {
    $.ajax(
        {
            url: GetPlanningTeamURL,
            method: "post",
            data: {
                planId: GlobalPlanId
            },
            success: function (data, statusText, xhr) {
                if (xhr.status == 400) {
                    notify(resource.frontPlanningTeamUpdateFailed, "danger", 5);
                }

                if (xhr.status > 500) {
                    notify(resource.frontServerError, "danger", 5);
                }

                $('#planning_team_portlet').html(data);

                $('#planning_team_portlet').find('.m-select2').select2();

                $.validator.unobtrusive.parse('#planning_team_portlet form');
            },
            error: function (xhr, statusText, error) {
                notify(resource.frontPlanningTeamUpdateError, "danger", 5);
            }
        })
}


/**
 *  Click event handler for a step save button
 * 
 */

$(document).on('click', 'button#step_form_save_button', function (e) {
    if ($('#step_form').valid()) {
        updateStep(false);
    }

});

/**
 *  Click event handler for a step submit button
 * 
 */

$(document).on('click', 'button#step_form_submit_button', function (e) {

    if ($('#step_form').valid()) {
        //Submits through an update function after a confirmation
        submitConfirm().then(function (result) {
            if (result) {
                updateStep(true);
            }
        });
    }

});
/**
 * @description blocks step form from an user and sends an update request, after unblocks ui
 * @param {bool} isSubmitted - whether update a step with submition
 */
function updateStep(isSubmitted) {

    // Metronic API for ui blocking that is based on BlockUI. source :http://jquery.malsup.com/block/
    mApp.block('#step_form', {
        overlayColor: '#000000',
        type: 'loader',
        state: 'success',
        centerY: true,
        centerX: true
    });

    let formData = new FormData(document.querySelector('form#step_form'));
    //append a value indicating whether a reqest is submitting
    formData.append('IsSubmitted', isSubmitted);
    $.ajax(
        {
            url: SaveStepURL,
            method: "post",
            data: formData,
            processData: false,
            contentType: false,
            success: function (data, statusText, xhr) {
                if (xhr.status == 200) {
                    notify(resource.frontSaveSuccess, "success", 5);
                }
                if (xhr.status == 202 || xhr.status == 400) {
                    notify(resource.frontInvalidInputData, "danger", 5);
                }

                $('#step_form_container').html(data);
                initializeInputs('#step_form_container');
                initializeStep();

                mApp.unblock('#step_form');
            },
            error: function (xhr, statusText, error) {
                notify(resource.frontRequestError, "danger", 5);
                mApp.unblock('#step_form');
            }
        });
}

$(document).ready(function () {

    /**
     * Predeparture step - Steptasks responsible person add modal showing event
     */
    $(document).on('show.bs.modal', '#edit_steptasks_modal', function (e) {
        // Responsible add form step property is set based calling button
        let step = $(e.relatedTarget).data('step');
        $(this).find('input[name$="Step"]').val(step);
    });

    /**
    * Predeparture step - Steptasks new responsible person add handler triggered by form submit
    */
    $(document).on('submit', 'form#add_responsible_user_to_step_new', function (e) {
        e.preventDefault();
        if (!$(this).valid()) {
            return;
        }

        let count = parseInt($('input#step_tasks_count').val());

        //takes values from a form
        let email = $(this).find('input[name$="Email"]').val();
        let firstName = $(this).find('input[name$="FirstName"]').val();
        let lastName = $(this).find('input[name$="LastName"]').val();
        let step = $(this).find('input[name$="Step"]').val();

        //composes html text from the give data and next is appended to a list
        let html = `<a class="m-list-badge__item m-list-badge__item--default list-item">
                                <input type="hidden" name="StepTaskAnswers.Answer.StepTaskAnswers[${count}].Email" value="${email}" />
                                <input type="hidden" name="StepTaskAnswers.Answer.StepTaskAnswers[${count}].FirstName" value="${firstName}" />
                                <input type="hidden" name="StepTaskAnswers.Answer.StepTaskAnswers[${count}].LastName" value="${lastName}" />
                                <input type="hidden" name="StepTaskAnswers.Answer.StepTaskAnswers[${count}].Step" value="${step}" />
                                <input type="hidden" name="StepTaskAnswers.Answer.StepTaskAnswers.Index" value="${count}" />
                                ${firstName} ${lastName} (${email})
                                <span class="fa fa-close list-item-delete"></span>
                            </a>`;

        $('#step_tasks_list_' + step).append(html);

        $('input#step_tasks_count').val(count + 1);

        $('#edit_steptasks_modal').modal('hide');
        $('body').removeClass('modal-open');
        $('.modal-backdrop').remove();
    });

    /**
   * Predeparture step - Steptasks existing responsible person add handler triggered by form submit
   */
    $(document).on('submit', 'form#add_responsible_user_to_step_existing', function (e) {
        e.preventDefault();
        if (!$(this).valid()) {
            return;
        }

        let count = parseInt($('input#step_tasks_count').val());

        let Id = $(this).find('select[name$="Id"]').val();
        let fullName = $(this).find('select[name$="Id"] option[value="' + Id + '"]').text();
        let step = $(this).find('input[name$="Step"]').val();

        let html = `<a class="m-list-badge__item m-list-badge__item--default list-item">
                                <input type="hidden" name="StepTaskAnswers.Answer.StepTaskAnswers[${count}].UserToPlanId" value="${Id}" />
                                <input type="hidden" name="StepTaskAnswers.Answer.StepTaskAnswers[${count}].Step" value="${step}" />
                                <input type="hidden" name="StepTaskAnswers.Answer.StepTaskAnswers.Index" value="${count}" />
                                ${fullName}
                                <span class="fa fa-close list-item-delete"></span>
                            </a>`;

        $('#step_tasks_list_' + step).append(html);

        $('input#step_tasks_count').val(count + 1);

        $('#edit_steptasks_modal').modal('hide');
        $('body').removeClass('modal-open');
        $('.modal-backdrop').remove();
    });

});

/**
 * Resets inputs after hiding a modal
 * 
 */
$(document).on('hidden.bs.modal', function (event) {
    var modal = $(event.target);

    modal.find('.m-input').each(function (i, el) {
        $(el).val("");
    });

    modal.find('select.m-select2').each(function (i, el) {
        $(el).select2('val', "");
        $(el).select2('close');
        $(el).trigger('change');
    });

    modal.find('.m-index').val("");

    modal.find('.field-validation-error').text("");
});
/**
 * @description Adds a new record or replaces an older one with new values from a modal
 * @param {string} modalId - Editing modal Id
 * @param {string} targetId - A List where these records are located
 */
function editRecord(modalId, targetId) {
    if (fieldOptions[targetId] === undefined) return;

    var options = fieldOptions[targetId];
    var modal = $(modalId);
    var list = $(targetId);

    if (modal.is('form') && !modal.valid()) return;

    var data = {};

    //collects values from a modal
    modal.find('.m-input').each(function (i, el) {
        data[$(el).attr('data-name')] = $(el).val();
    });

    modal.find('select.m-input').each(function (i, el) {
        data[$(el).attr('data-name') + '_label'] = $(el).find('option:selected').text();
    });

    var index = modal.find('.m-index').val();

    //checks if it has to replace an oldeer record or append new one
    if (index) {
        $('#' + index).replaceWith(options.template(data, index));
    }
    else {
        index = guid();
        var temp = options.template(data, index); // takes templates that are defined in Questions razors

        if (temp) {
            list.append(temp);
        }
    }

    if (modal.hasClass('modal')) {
        modal.modal('hide');
    }

    modal.find('.m-input').each(function (i, el) {
        $(el).val("");
    });

    modal.find('.m-index').val("");
}

/**
 * @description Copies desired answer from other answers to current user answers' list
 * @param {any} element An answer that to be copied
 * @param {any} targetId Target list
 */
function selectAnswer(element, targetId) {
    //fieldOptions is a map that contains templates for different add of different answers
    if (fieldOptions[targetId] === undefined) return;

    var options = fieldOptions[targetId];
    var list = $(targetId);

    var data = JSON.parse($(element).attr('data-val'));
    var index = guid();

    var temp = options.template(data, index);

    if (temp) {
        list.append(temp);
    }
}

/**
 * @description Opens a modal filled with values from from according answer record (from table row for an instance)
 * @param {any} source  An element that this methods is attached to (Edit button)
 * @param {any} modalId Modal's id
 */
function showRecordDetail(source, modalId) {
    var modal = $(modalId);

    var record = source.closest('tr');

    var data = JSON.parse($(record).attr('data-val'));

    var index = $(record).attr('id');

    modal.find('.m-input').each(function (i, el) {
        $(el).val(data[$(el).attr('data-name')]);
    });

    modal.find('select.m-select2.m-input[multiple]').each(function (i, el) {
        $(el).val(data[$(el).attr('data-name')].split(','));
        $(el).trigger('change');
    });

    modal.find('.m-index').val(index);

    modal.modal('show');
}

/**
 * @description Deletes an record from a table or a list
 * @param {any} source  An element that this methods is attached to (Edit button)
 * @param {any} targetSelector a selector of an element that has to be deleted
 */
function deleteRecord(source, targetSelector = false) {

    deleteConfirm().then(function (result) {
        if (result) {
            var record = source;

            if (targetSelector !== false) {
                record = source.closest(targetSelector);
            }
            $(record).remove();
        }
    })


}

/**
 * @description Controls a single checkbox to be checked in a checkbox group
 * @param {any} el 
 */

function checkLikeRadio(el) {
    if ($(el).is(':checked')) {
        $('.radio-checkbox[data-group="' + $(el).attr('data-group') + '"]').each(function () {
            if (this !== el) {
                $(this).prop('checked', false);
            }
        });
    }
}

/**
 * @description Generates GUID string
 */
function guid() {

    /**
     * @description Generates a random 4 character length string
     */
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
        s4() + '-' + s4() + s4() + s4();
}


/**
 * Step completion handler triggered by a switch in steptasks tables from Predeparture step
 * 
 */
$(document).on('click', '.step-complete-checkbox', function (e) {
    e.preventDefault();
    let checkbox = e.target;

    if (!$(checkbox).prop("checked")) {
        return;
    }

    let planId = $(checkbox).data('planid');
    let stepIndex = $(checkbox).data('step');

    completeStep(planId, stepIndex, function () {
        setTimeout(function () { location.reload(); }, 1000);
    });
});

/**
 * Step completion handler triggered by a Complete button
 * 
 */
$(document).on('click', 'button#step_form_complete', function (e) {
    e.preventDefault();

    let formData = new FormData(document.querySelector('form#step_form'));
    let planId = formData.get('PlanId');
    let stepIndex = formData.get('Step');

    completeStep(planId, stepIndex, function () {
        setTimeout(function () { location.reload(); }, 1000);
    })
});

/**
 * @description completes a step
 * @param {number} planId - Working plan's Id
 * @param {string} stepIndex - Completing step index
 * @param {function} callback - callback function that will be called after completion handler
 */
function completeStep(planId, stepIndex, callback) {

    submitConfirm(resource.frontStepCompleteConfirm).then(function (result) {
        if (result) {
            return $.ajax(
                {
                    url: CompleteStepTaskURL,
                    method: "post",
                    data: {
                        planId,
                        stepIndex
                    },
                    processData: true,
                    success: function (data, statusText, xhr) {
                        if (xhr.status == 200) {
                            notify(resource.frontCompleteSucccess, "success", 5);
                            callback(); // here is called a callback
                        }

                        if (xhr.status == 202) {
                            notify(resource.frontCompleteImpossible, "warning", 5);
                        }
                    },
                    error: function (xhr, statusText, error) {

                        notify(resource.frontRequestError, "danger", 5);
                    }
                });
        }
    })

}
/**
 * @description refreshes step form
 * @param {any} keepFilled - determines whether to keep already filled and unsaved data
 */
function refreshStepForm(keepFilled = false) {
    let formData = new FormData(document.querySelector('form#step_form'));
    formData.append('keepFilled', keepFilled);
    $.ajax(
        {
            url: RefreshStepFormURL,
            method: "post",
            data: formData,
            processData: false,
            contentType: false,
            success: function (data, statusText, xhr) {
                if (xhr.status == 200) {
                    //notify("Step successfully refreshed", "success", 5);
                }

                if (xhr.status >= 400) {
                    notify(resource.frontStepNotRefreshed, "danger", 5);
                }

                $('#step_form_container').html(data);
                initializeInputs('#step_form_container');
                initializeStep();
            },
            error: function (xhr, statusText, error) {
                notify(resource.frontRequestError, "danger", 5);
            }
        });
}
