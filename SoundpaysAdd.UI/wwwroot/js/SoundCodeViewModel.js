var SoundCodeViewModel = new function () {
    var thisViewModel = this;
    var tblSoundCode = "tblSoundCode";
    this.SoundCodeListView = function (data) {
        $(`#${tblSoundCode}`).dataTable({
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
            "sAjaxSource": "/SoundCode/Index?handler=AllSoundCodeDataTable",
            "aoColumns": [
                { "mDataProp": "code", "sClass": "center" },
                { "mDataProp": "startZone", "sClass": "center" },
                { "mDataProp": "endZone", "sClass": "center" },
                {
                    "orderable": false,
                    "sWidth": "15%",
                    "mRender": function (data, type, full) {
                        if (!full.isActive) {
                            return `<a data-id="${full.id}" data-name="${full.shortName}" onClick="SoundCodeViewModel.Activate(this)" class="btn btn-sm text-light btn-secondary ladda-button " data-style="zoom-out" type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Activate">
                            <i class="fas fa-toggle-off"></i>
                            </a>`;
                        }
                        else {
                            return `<a data-id="${full.id}" data-name="${full.shortName}" onClick="SoundCodeViewModel.Deactivate(this)" class="btn btn-sm text-light btn-success ladda-button " data-style="zoom-out" type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Deactivate">
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
                            return `<a data-id="${full.id}" data-name="${full.shortName}" onClick="SoundCodeViewModel.Resume(this)" class="btn btn-sm text-light btn-secondary ladda-button " data-style="zoom-out" type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Activate">
                            <i class="fas fa-toggle-off"></i>
                            </a>`;
                        }
                        else {
                            return `<a data-id="${full.id}" data-name="${full.shortName}" onClick="SoundCodeViewModel.Pause(this)" class="btn btn-sm text-light btn-success ladda-button " data-style="zoom-out" type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Deactivate">
                            <i class="fas fa-toggle-on"></i>
                            </a>`;
                        }
                    }
                },
                {
                    "orderable": false,
                    "mRender": function (data, type, full) {
                        var actionString = "";
                        actionString = `<button onClick="SoundCodeViewModel.OpenSoundCodeModal(${parseInt(full.id)})" class="btn-lock-user btn btn-sm btn-secondary text-white ladda-button" data-style="zoom-out"
                                         type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Edit"><i class="fas fa-edit"></i></button>
                                         <button data-id="${full.id}" onClick="SoundCodeViewModel.Delete(this,${parseInt(full.id)})" class="btn-lock-user btn btn-sm btn-danger text-white ladda-button ml-2" data-style="zoom-out"
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


    //open Sound Code model
    this.OpenSoundCodeModal = function (soundCodeId) {
        GetData({
            url: `/SoundCode/Index?handler=CreateOrEdit`,
            successHandler: openModalSuccess,
            data: {
                formId: "formAddSoundCode",
                id: soundCodeId,
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
            refreshDataTable(tblSoundCode);
        }
    };

    //delete
    this.Activate = (e) => actionConfirmBox({ text: "Activate", url: "/SoundCode?handler=Activate", elem: e, cb: this.ActionCB });
    this.Deactivate = (e) => actionConfirmBox({ text: "Deactivate", url: "/SoundCode?handler=Deactivate", elem: e, cb: this.ActionCB });
    this.Pause = (e) => actionConfirmBox({ text: "Pause", url: "/SoundCode?handler=Pause", elem: e, cb: this.ActionCB });
    this.Resume = (e) => actionConfirmBox({ text: "Resume", url: "/SoundCode?handler=Resume", elem: e, cb: this.ActionCB });
    this.Delete = (e) => actionConfirmBox({ url: "/SoundCode?handler=Delete", elem: e, cb: this.ActionCB });
    this.ActionCB = function (result, data, elem) {
        if (result.success) {
            SwalSuccess(result.message);
        }
        else {
            SwalError("Something went wrong!");
        }
        elem && elem.stop(); //lada button
        refreshDataTable(tblSoundCode);
    };
}
