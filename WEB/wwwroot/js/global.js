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
            source: fieldOptions.valuesDictionary
        });

        $(selector).find('.m-stakeholders-autocomplete').autocomplete({
            source: fieldOptions.stakeholderUsers,
            change: function (event, ui) {
                if (!ui.item) {
                    $(this).val('');
                    return;
                }

                $(event.target).next("[data-name='stakeholderId']").val(ui.item.id);
            }
        });
    }

    $.validator.unobtrusive.parse(selector);

    $(selector).find('.sp-tooltip').tooltip();

    //$(selector).find('[maxlength],[data-val-maxlength-max]').maxlength(
    //    {
    //        alwaysShow: true,
    //        warningClass: "alert alert-primary"
    //    });
}
//////
/// read more/less for large texts
//
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
////
//

$(document).ready(function () {

    ///////
    //shared
    //
    //$('form').on('keyup keypress', function (e) {
    //    var keyCode = e.keyCode || e.which;
    //    if (keyCode === 13) {
    //        e.preventDefault();
    //        return false;
    //    }
    //});
    initializeInputs(document);

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

    $('.user-submit-btn').click(submitConfirm);

    $('.draggable-file').draggable({
        revert: "invalid", // when not dropped, the item will revert back to its initial position
        containment: "document",
        helper: "clone",
        cursor: "move"
    });

    $('.table-row-delete').click(function (e) {
        e.preventDefault();
        deleteConfirm();
    });

    /////
    ///

});


///////
// delete confirm alert
//

function deleteConfirm() {
    return swal({
        icon: "warning",
        title: "Are you sure?",
        buttons:
        {
            confirm:
            {
                text: "Yes",
                value: true,
                visible: true,
                className: "btn btn-sp m-btn",
                closeModal: true,
            },
            cancel:
            {
                text: "No",
                value: false,
                visible: true,
                className: "btn btn-secondary m-btn",
                closeModal: true,
            }
        }
    }
    );
}

function submitConfirm(text) {
    return swal({
        icon: "warning",
        title: "Are you sure?",
        text: text,
        buttons:
        {
            confirm:
            {
                text: "Yes",
                value: true,
                visible: true,
                className: "btn btn-sp m-btn",
                closeModal: true,
            },
            cancel:
            {
                text: "No",
                value: false,
                visible: true,
                className: "btn btn-secondary m-btn",
                closeModal: true,
            }
        }
    }
    );
}

//////////
/// dropzone file previews remove relating functions
//

function filePreviewRemoveHandler(e) {
    removeFilePreview($(e.target).parent());
}

function removeFilePreview(filePreview) {
    let parentDropzone = $(filePreview).parent();
    parentDropzone.find('.dz-message').hide();
    deleteConfirm().then(function (isConfirm) {

        if (isConfirm) {
            filePreview.remove();
        }

        if (parentDropzone.find('.dz-preview').length == 0) {
            parentDropzone.find('.dz-message').show();
        }
    });
}

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

////////////
//// plan create ajax
///


$(document).on('submit', 'form#add_plan_form', function (e) {
    e.preventDefault();

    if (!$('#add_plan_form').valid()) {
        return;
    }

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
                    notify("Successfully created", "success", 5);
                    setTimeout(function () {
                        document.location.reload(false)
                    }, 3000);
                }
                if (xhr.status == 202 || xhr.status == 400) {
                    notify("Input data are not valid ", "danger", 5);
                }

                $('#add_plan_form').html(data);
                initializeInputs('#add_plan_form');
            },
            error: function (xhr, statusText, error) {
                notify("An Error occured on the request", "danger", 5);
            }
        });
});

////
///


///////
/// plan delete ajax
///

$(document).on('click', '.delete-plan', function (e) {
    e.preventDefault();

    let planId = $(this).data('id');

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
                            notify("An Error occured during sending a request", "danger", 5);

                        }

                        if (data.result) {
                            notify("Successfully deleted", "success", 5);
                            setTimeout(function () {
                                document.location.reload(false)
                            }, 3000);

                        }
                        else {
                            notify("Deleting the plan is not possible due to existing answers", "danger", 5);
                        }

                    },
                    error: function (xhr, statusText, error) {
                        notify("An Error occured during sending a request", "danger", 5);
                    }
                })
        }
    })

});

//////
///