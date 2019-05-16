function initializeInputs(element) {

    $(element).find('.m-select2').select2();

    $(element).find(".tag-select").select2({

        tags: true,
        tokenSeparators: [","]
    });

    $(element).find('.datepicker').datepicker();

    $(element).find('.select2-without-search').select2({
        minimumResultsForSearch: Infinity
    });
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

    $('.sp-tooltip').tooltip();

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

    $('.dropzone-sp').dropzone(
        {
            addRemoveLinks: true,
            removedfile: function (file) {
                let currentPreviewElement = $(file.previewElement);

                removeFilePreview(currentPreviewElement);

            },
            previewTemplate: `
            <div class="dz-preview dz-file-preview dz-processing dz-error dz-complete">  
                <div class="dz-image">
                    <img data-dz-thumbnail="">
                </div>  
                <div class="dz-details">    
                    <div class="dz-size">
                        <span data-dz-size=""><strong></strong> KB</span>
                    </div>    
                    <div class="dz-filename">
                        <span data-dz-name=""></span>
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
                e.preventDefault();

                let name = $(ui.draggable).html();

                $(this).find('.dz-message').hide();

                $(this).append(`<div class="dz-preview dz-file-preview dz-processing dz-error dz-complete">  
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
    }, {
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