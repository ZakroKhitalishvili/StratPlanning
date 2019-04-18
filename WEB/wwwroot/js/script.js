
$(document).ready(function () {

    ///////
    //shared
    //
    $('.datepicker').datepicker();

    $('.m-select2').select2();

    $('.select2-without-search').select2({
        minimumResultsForSearch: Infinity
    });

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

    $('.stakeholders-rating-table .m-select2').select2({
        minimumResultsForSearch: Infinity
    });

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
    })


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

