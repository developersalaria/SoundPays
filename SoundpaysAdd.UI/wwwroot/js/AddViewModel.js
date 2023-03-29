var AddViewModel = new function () {
    var thisViewModel = this;
    var tblAdd = "tblAdd";
    this.AddListView = function (data) {
        $(`#${tblAdd}`).dataTable({

            "bStateSave": false,
            "iDisplayLength": 10,
            "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
            //for CSV buttons
            "dom": "Blfrtip",
            buttons: [

            ],
            cache: true,
            "responsive": true,
            "bServerSide": true,
            "bProcessing": true,
            "bJqueryUI": true,
            "sPaginationType": "full_numbers",
            "aaSorting": [[0, "asc"]],
            "sAjaxSource": "/Add/Index?handler=AllAddDataTable",
            "aoColumns": [
                { "mDataProp": "soundCodeName", "sClass": "center" },
                { "mDataProp": "campaignName", "sClass": "center" },
                { "mDataProp": "longName", "sClass": "center" },
                { "mDataProp": "shortName", "sClass": "center" },
                { "mDataProp": "startDateDisplay", "sClass": "center" },
                { "mDataProp": "startTime", "sClass": "center" },
                { "mDataProp": "stopDateDisplay", "sClass": "center" },
                { "mDataProp": "stopTime", "sClass": "center" },
                { "mDataProp": "addTypeName", "sClass": "center" },
                { "mDataProp": "minWidth", "sClass": "center" },
                { "mDataProp": "maxWidth", "sClass": "center" },
                { "mDataProp": "minHeight", "sClass": "center" },
                { "mDataProp": "maxHeight", "sClass": "center" },
                {
                    "orderable": false,
                    "sWidth": "15%",
                    "mRender": function (data, type, full) {
                        if (!full.isActive) {
                            return `<a data-id="${full.id}" data-name="${full.shortName}" onClick="AddViewModel.Activate(this)" class="btn btn-sm text-light btn-secondary ladda-button " data-style="zoom-out" type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Activate">
                            <i class="fas fa-toggle-off"></i>
                            </a>`;
                        }
                        else {
                            return `<a data-id="${full.id}" data-name="${full.shortName}" onClick="AddViewModel.Deactivate(this)" class="btn btn-sm text-light btn-success ladda-button " data-style="zoom-out" type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Deactivate">
                            <i class="fas fa-toggle-on"></i>
                            </a>`;
                        }
                    }
                },
                {
                    "orderable": false,
                    "sWidth": "15%",
                    "mRender": function (data, type, full) {
                        if (full.isPaused) {
                            return `<a data-id="${full.id}" data-name="${full.shortName}" onClick="AddViewModel.Resume(this)" class="btn btn-sm text-light btn-secondary ladda-button " data-style="zoom-out" type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Activate">
                            <i class="fas fa-toggle-off"></i>
                            </a>`;
                        }
                        else {
                            return `<a data-id="${full.id}" data-name="${full.shortName}" onClick="AddViewModel.Pause(this)" class="btn btn-sm text-light btn-success ladda-button " data-style="zoom-out" type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Deactivate">
                            <i class="fas fa-toggle-on"></i>
                            </a>`;
                        }
                    }
                    , "visible": isAdmin
                },
                {
                    "orderable": false,
                    "mRender": function (data, type, full) {
                        var actionString = "";
                        actionString = `
 <button onClick="AddViewModel.OpenAddModal(${parseInt(full.id)},'true')" class="btn btn-sm btn-info text-white ladda-button" data-style="zoom-out"
                                                                     type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="View"><i class="fa-solid fa-eye"></i></button>
                                         <button onClick="AddViewModel.OpenAddModal(${parseInt(full.id)})" class="btn-lock-user btn btn-sm btn-secondary text-white ladda-button" data-style="zoom-out"
                                         type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Edit"><i class="fas fa-edit"></i></button>
                                         <button onClick="AddViewModel.Delete(this,${parseInt(full.id)})" class="btn-lock-user btn btn-sm btn-danger text-white ladda-button ml-2" data-style="zoom-out"
                                         type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Delete"><i class="fas fa-trash"></i></button>`;
                        return actionString;
                    }
                },
            ],
            columnDefs: [
                {
                    "defaultContent": " ",
                    "targets": "_all"
                }],
            "autoWidth": true
        });
    };


    //open Add model
    this.OpenAddModal = function (addId, readOnly = false) {
        let url = `/Add/Index?handler=CreateOrEdit`;
        if (readOnly) url = `${url}&readOnly=true`;
        GetData({
            url: url,
            successHandler: openModalSuccess,
            data: {
                formId: "formAddAdd",
                id: addId,
            }
        });

    };

    this.UpdateSuccess = function (success, message, data = null) {
        if (success == false) {
            failAlert(message);
            if (data) {
                data = JSON.parse(data);
            }
        }
        else {
            successAlert(message);
            closeCommonModel();
            refreshDataTable(tblAdd);
        }
    };
    //Delete

    this.Activate = (e) => actionConfirmBox({ text: "Activate", url: "/Add?handler=Activate", elem: e, cb: this.ActionCB });
    this.Deactivate = (e) => actionConfirmBox({ text: "Deactivate", url: "/Add?handler=Deactivate", elem: e, cb: this.ActionCB });
    this.Pause = (e) => actionConfirmBox({ text: "Pause", url: "/Add?handler=Pause", elem: e, cb: this.ActionCB });
    this.Resume = (e) => actionConfirmBox({ text: "Resume", url: "/Add?handler=Resume", elem: e, cb: this.ActionCB });
    this.Delete = (e) => actionConfirmBox({ url: "/Add?handler=Delete", elem: e, cb: this.ActionCB });
    this.ActionCB = function (result, data, elem) {
        if (result.success) {
            SwalSuccess(result.message);
        }
        else {
            SwalError("Something went wrong!");
        }
        elem && elem.stop(); //lada button
        refreshDataTable(tblAdd);
    };

    // delete attachment
    this.deleteAttachment = function (addId, id, index) {
        swal({
            title: "Are you sure?",
            text: "Please confirm you wish to delete.",
            icon: "warning",
            buttons: true,
            dangerMode: true
        })
            .then((willDelete) => {
                if (willDelete) {
                    let obj = {};
                    obj = { url: "/Add?handler=DeleteAttachment", successHandler: AddViewModel.deleteAttachmentSuccess(index), data: { addId: addId, id: id } };
                    PostData(obj);
                }
            });
    }

    //delete attachment success
    this.deleteAttachmentSuccess = function (index) {
        $("#rowAttachment_" + index).remove();
        SwalSuccess("File Deleted Successfully!");
    }
}
