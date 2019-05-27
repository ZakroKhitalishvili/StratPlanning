$(document).ready(function () {
    initializeStep();
});

function initializeStep() {
    $('.stakeholders-rating-table .m-select2').select2({
        minimumResultsForSearch: Infinity
    });

    $('.sp-tooltip').tooltip();

    /////////////
    //stakeholders analysis worksheet
    //////////
    // $('#stakeholders-rating-table').DataTable(
    //     {
    //         responsive: true,
    //         paging: false,
    //         serverSide: false,
    //         searching: false,
    //         info: false,
    //         columnDefs:
    //             [{ "orderDataType": "dom-select", targets: [1, 2, 3, 4, 5, 6, 7, 9] }],
    //     }
    // );

    /////////////////////
    ////////////

    ////////////
    // master list of issues
    ///
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
                { orderDataType: 'dom-input', targets: [6] }],
        }
    );

    $('#issues-master-list-table tr td:nth-child(6) input').blur(function (e) {
        $('#issues-master-list-table').DataTable().column(6).order('asc').draw();
    }).on('keyup', function (e) {
        if (e.keyCode == 13)//enter button
        {
            $('#issues-master-list-table').DataTable().column(6).order('asc').draw();
        }
    });

    $('#issues-master-list-table').on('row-reordered.dt', function (e, details, edit) {

        details.map(function (el, ind) {
            $(el.node).find("input.order").val(el.newPosition);
        });

    });

    /////////////
    ///////

    //////
    /// evalution sliders
    ///

    $(".evalution-slider").each(function (ind, slider) {
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
    });
    ///////////
    //// selects speficy-other event
    //////////////

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

    $('.dropzone-sp').each(function () {
        var element = $(this);
        var initurl = element.data('initurl');
        var inputname = element.data('inputname');

        element.dropzone({
            init: function () {
                thisDropzone = this;

                if (!initurl) return;

                $.get(initurl, function (data) {
                    if (data === null) {
                        return;
                    }

                    $.each(data, function (key, value) {
                        var mockFile = { name: value.name, fileId: value.id };

                        thisDropzone.emit("addedfile", mockFile);

                        $(mockFile.previewElement).find(".sp-file-input").val(value.id);
                        $(mockFile.previewElement).find(".sp-file-link").attr('href', value.url);

                        //thisDropzone.options.thumbnail.call(thisDropzone, mockFile, value.path);
                        //// Make sure that there is no progress bar, etc...
                        //thisDropzone.emit("complete", mockFile);
                        //thisDropzone.emit("complete", $(mockFile._removeLink).html("<div ><span class='fa fa-trash text-danger' style='font-size: 1.5em'></span></div>"));
                        //thisDropzone.emit("complete", $(mockFile._removeLink).click(function () { $.post("/Manage/Remove", { "id": value.id }, function (data) { console.log(data); }); }));
                    });
                });

            },
            addRemoveLinks: true,
            removedfile: function (file) {
                let currentPreviewElement = $(file.previewElement);

                removeFilePreview(currentPreviewElement);

            },
            url: "/Worksheet/UploadFile",
            uploadMultiple: true,
            paramName: 'file',
            success: function (file, res) {
                var index = 0;
                var fileResponse = res.filter(function (data, i) {
                    if (data.name === file.name) {
                        index = i;
                        return true;
                    }

                    return false;
                })[0];


                $(file.previewElement).find(".sp-file-input").val(fileResponse.id);
                $(file.previewElement).find(".sp-file-link").attr('href', fileResponse.url);
            },
            previewTemplate: `
                <div class="dz-preview dz-file-preview dz-processing dz-error dz-complete">
                    <input class="sp-file-input" type="hidden" name="${inputname}"/>
                    <div class="dz-image">
                        <img data-dz-thumbnail="">
                    </div>  
                    <div class="dz-details">
                        <div class="dz-filename">
                            <a class="sp-file-link" href="#" target="_Blank"><span data-dz-name=""></span></a>
                        </div>
                    </div>  
                    <div class="dz-progress">
                        <span class="dz-upload" data-dz-uploadprogress=""></span>
                    </div>   
                    </div>
                    <a class="dz-remove" href="javascript:undefined;" data-dz-remove="">Remove file</a>
                </div>`
        })
            .droppable({
                accept: '.draggable-file',

                drop: function (e, ui) {
                    console.log('drop');

                    e.preventDefault();

                    let name = $(ui.draggable).html();

                    $(this).find('.dz-message').hide();

                    $(this).append(`
                <div class="dz-preview dz-file-preview dz-processing dz-error dz-complete">
                    <input class="sp-file-input" type="hidden" name="${inputname}"/>
                    <div class="dz-image">
                        <img data-dz-thumbnail=""/>
                    </div>
                    <div class="dz-details">      
                        <div class="dz-filename">
                            <span data-dz-name="">${name}</span>
                        </div>  
                        <div class="dz-progress">
                            <span class="dz-upload" data-dz-uploadprogress=""></span>
                        </div>   
                    </div>
                    <a class="dz-remove" href="javascript:undefined;" onclick="filePreviewRemoveHandler(event)" data-dz-remove="">Remove file</a>
                </div>`);
                }
            })
            .addClass('dropzone');
    });

    //////
    /// list-items events
    ///

    $(document).on('click', '.list-item-delete', function (e) {
        let listItem = $(this).closest('.list-item');
        listItem.remove();
    });

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

    $(document).on('keyup', '.list-add-input', function (e) {
        if (e.which == 13) {
            e.preventDefault();
            $(this).parents('.list-items-add-group').first().find('.list-add-button').click();
        }
    })
    //////
    ////
}


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
                    notify("Successfully added", "success", 5);
                    $('#planning_team_add_modal').modal('hide');
                    $('body').removeClass('modal-open');
                    $('.modal-backdrop').remove();
                    updatePlanningTeam();
                    refreshStepForm(true);

                }
                if (xhr.status == 400) {
                    notify("An Error occured during sending a request", "danger", 5);
                }

                $('form#add_user_to_plan_new').html(data);

                $('form#add_user_to_plan_new').find('.m-select2').select2();

                $.validator.unobtrusive.parse('form#add_user_to_plan_new');
            },
            error: function (xhr, statusText, error) {
                notify("An Error occured during sending a request", "danger", 5);
            }
        })
});

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
                    notify("Successfully added", "success", 5);
                    $('#planning_team_add_modal').modal('hide');
                    $('body').removeClass('modal-open');
                    $('.modal-backdrop').remove();
                    updatePlanningTeam();
                    refreshStepForm(true);
                }

                if (xhr.status == 400) {
                    notify("An Error occured during sending a request", "danger", 5);
                }

                $('form#add_user_to_plan_existing').html(data);
                $('form#add_user_to_plan_existing').find('.m-select2').select2();
                $.validator.unobtrusive.parse('form#add_user_to_plan_existing');
            },
            error: function (xhr, statusText, error) {
                notify("An Error occured during sending a request", "danger", 5);
            }
        })
});


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
                            notify("An Error occured during sending a request", "danger", 5);
                        }

                        if (data.result) {
                            notify("Successfully deleted", "success", 5);
                            updatePlanningTeam();
                            refreshStepForm(true);
                        }
                        else {
                            notify("Removing the user failed", "danger", 5);
                        }

                    },
                    error: function (xhr, statusText, error) {
                        notify("An Error occured during sending a request", "danger", 5);
                    }
                })
        }
    })

});

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
                    notify("Planning team update failed", "danger", 5);
                }

                if (xhr.status > 500) {
                    notify("An error occured on the server", "danger", 5);
                }

                $('#planning_team_portlet').html(data);

                $('#planning_team_portlet').find('.m-select2').select2();

                $.validator.unobtrusive.parse('#planning_team_portlet form');

            },
            error: function (xhr, statusText, error) {
                notify("An Error occured during updating the planning plan's view", "danger", 5);
            }
        })
}

$(document).on('click', 'button#step_form_save_button', function (e) {
    if ($('#step_form').valid()) {
        updateStep(false);
    }

});

$(document).on('click', 'button#step_form_submit_button', function (e) {

    if ($('#step_form').valid()) {
        submitConfirm().then(function (result) {
            if (result) {
                updateStep(true);
            }
        });
    }

});

function updateStep(isSubmitted) {
    let formData = new FormData(document.querySelector('form#step_form'));
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
                    notify("Successfully saved", "success", 5);
                }
                if (xhr.status == 202 || xhr.status == 400) {
                    notify("Input data are not valid ", "danger", 5);
                }

                $('#step_form_container').html(data);
                initializeInputs($('form#step_form'));
                initializeStep();
            },
            error: function (xhr, statusText, error) {
                notify("An Error occured on the request", "danger", 5);
            }
        });
}

////////////////////////////////
//// step tasks in predeparture step
////////
$(document).ready(function () {
    $(document).on('show.bs.modal', '#edit_steptasks_modal', function (e) {
        let step = $(e.relatedTarget).data('step');
        console.log('called bs modal show');
        $(this).find('input[name$="Step"]').val(step);
    });

    $(document).on('submit', 'form#add_responsible_user_to_step_new', function (e) {
        e.preventDefault();
        if (!$(this).valid()) {
            return;
        }

        let count = parseInt($('input#step_tasks_count').val());

        let email = $(this).find('input[name$="Email"]').val();
        let firstName = $(this).find('input[name$="FirstName"]').val();
        let lastName = $(this).find('input[name$="LastName"]').val();
        let step = $(this).find('input[name$="Step"]').val();

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

/////////
/////

$(document).on('hidden.bs.modal', function (event) {
    var modal = $(event.target);

    modal.find('.m-input').each(function (i, el) {
        $(el).val("");
    });

    modal.find('.m-index').val("");
});

function editRecord(modalId, targetId) {
    if (fieldOptions[targetId] === undefined) return;

    var options = fieldOptions[targetId];
    var modal = $(modalId);
    var list = $(targetId);

    var data = {};

    modal.find('.m-input').each(function (i, el) {
        data[$(el).attr('name')] = $(el).val();
    });

    modal.find('select.m-input').each(function (i, el) {
        data[$(el).attr('name') + '_label'] = $(el).find('option:selected').text();
    });

    var index = modal.find('.m-index').val();

    if (index) {
        $('#' + index).replaceWith(options.template(data, index));
    }
    else {
        index = guid();

        list.append(options.template(data, index));
    }

    if (modal.hasClass('modal')) {
        modal.modal('hide');
    }

    modal.find('.m-input').each(function (i, el) {
        $(el).val("");
    });

    modal.find('.m-index').val("");
}

function showRecordDetail(source, modalId) {
    var modal = $(modalId);

    var record = source.closest('tr');

    var data = JSON.parse($(record).attr('data-val'));

    var index = $(record).attr('id');

    modal.find('.m-input').each(function (i, el) {
        $(el).val(data[$(el).attr('name')]);
    });

    modal.find('.m-index').val(index);

    modal.modal('show');
}

function deleteRecord(source, targetSelector = false) {
    var record = source;

    if (targetSelector !== false) {
        record = source.closest(targetSelector);
    }

    $(record).remove();
}

function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
        s4() + '-' + s4() + s4() + s4();
}


///////////////////////////
//// steptask completion event
////////
$(document).on('click', 'input.step-complete-checkbox', function (e) {
    e.preventDefault();

    let completeCheckBox = $(this);
    let stepTaskId = completeCheckBox.data('id');

    if ($(this).val()) {
        submitConfirm("You won't be able to change it after").then(function (result) {
            if (result) {
                $.ajax(
                    {
                        url: CompleteStepTaskURL,
                        method: "post",
                        data: {
                            stepTaskId
                        },
                        processData: true,
                        success: function (data, statusText, xhr) {
                            if (xhr.status == 200) {
                                notify("Successfully completed", "success", 5);
                                completeCheckBox.prop('checked', true);
                            }

                            if (xhr.status == 202 || xhr.status == 400) {
                                notify("Input data are not valid ", "danger", 5);
                            }

                        },
                        error: function (xhr, statusText, error) {
                            notify("An Error occured on the request", "danger", 5);
                        }
                    });
            }
        });
    }
});
///////
////

/////////////////////////
////// step form refresh
////////
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
                    notify("Step successfully refreshed", "success", 5);
                }

                if (xhr.status >= 400) {
                    notify("Step was not refreshed", "danger", 5);
                }

                $('#step_form_container').html(data);
                initializeInputs('#step_form_container');
                initializeStep();
            },
            error: function (xhr, statusText, error) {
                notify("An Error occured on the request", "danger", 5);
            }
        });
}
//////////
//////
////