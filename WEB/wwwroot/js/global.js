function initializeInputs(selector) {

    $(selector).find('.m-select2').select2();

    $(selector).find(".tag-select").select2({

        tags: true,
        tokenSeparators: [","]
    });

    $(selector).find('.datepicker').datepicker({ format: 'dd-mm-yyyy' });

    $(selector).find('.select2-without-search').select2({
        minimumResultsForSearch: Infinity
    });

    if (typeof valueAnswerOptions !== 'undefined') {
        $(selector).find('.m-values-autocomplete').autocomplete({
            source: valueAnswerOptions.valuesDictionary
        });
        $(selector).find('.m-values-autocomplete').autocomplete('option', 'appendTo', '.modal');
    }

    $.validator.unobtrusive.parse(selector);
}


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
        }
    });
}