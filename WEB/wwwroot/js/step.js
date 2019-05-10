
$(document).ready(function () {

    $('.stakeholders-rating-table .m-select2').select2({
        minimumResultsForSearch: Infinity
    });

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
        $('#issues-master-list-table').DataTable().column(6).order().draw();
    }).on('keyup', function (e) {
        if (e.keyCode == 13)//enter button
        {
            $('#issues-master-list-table').DataTable().column(6).order().draw();
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

        if (value.toLowerCase() == 'other') {
            specifyInput.show();
        }
        else {
            specifyInput.hide();
        }
    }).trigger('change');

});


$(document).on('submit', "form#add_user_to_plan_new", function (e) {
    e.preventDefault();

    let formData = new FormData(document.querySelector('form#add_user_to_plan_new'));

    let formObject = {};
    formData.forEach(function (value, key) {
        formObject[key] = value;
    });

    formObject['PlanId'] = GlobalPlanId;

    $.ajax(
        {
            url: AddNewUserToPlanURL,
            method: "post",
            data: formObject,
            success: function (data, statusText, xhr) {
                if (xhr.status == 201) {
                    notify("Successfully added", "success", 5);
                    updatePlanningTeam();

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

    let formObject = {};
    formData.forEach(function (value, key) {
        formObject[key] = value;
    });

    formObject['PlanId'] = GlobalPlanId;

    $.ajax(
        {
            url: AddExistingUserToPlanURL,
            method: "post",
            data: formObject,
            success: function (data, statusText, xhr) {
                if (xhr.status == 201) {

                    notify("Successfully added", "success", 5);
                    updatePlanningTeam();
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

    $('#planning_team_add_modal').modal('hide');

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

                $('#planning_team_portlet').html(data);

                $('#planning_team_portlet').find('.m-select2').select2();

                $.validator.unobtrusive.parse('#planning_team_portlet form');

            },
            error: function (xhr, statusText, error) {
                notify("An Error occured during updating the planning plan's view", "danger", 5);
            }
        })
}

$(document).on('submit', "form#step_form", function (e) {
    e.preventDefault();

    console.log('submitted');

    let formData = new FormData(document.querySelector('form#step_form'));

    let formObject = {};
    formData.forEach(function (value, key) {
        formObject[key] = value;
    });

    $.ajax(
        {
            url: SaveStepURL,
            method: "post",
            data: formObject,
            success: function (data, statusText, xhr) {
                if (xhr.status == 200) {

                    notify("Successfully saved", "success", 5);
                    updatePlanningTeam();
                }

                if (xhr.status == 400) {
                    notify("An Error occured during sending a request", "danger", 5);
                }

                $('form#step_form').html(data);

                initializeInputs($('form#step_form'));

                $.validator.unobtrusive.parse('form#step_form');
            },
            error: function (xhr, statusText, error) {
                notify("An Error occured during sending a request", "danger", 5);
            }
        })
});


