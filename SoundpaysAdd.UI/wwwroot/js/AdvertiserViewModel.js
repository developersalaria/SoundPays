var AdvertiserViewModel = new function () {
    var thisViewModel = this;
    var tblAdvertiser = "tblAdvertiser";
    this.ListView = function (data) {
        $(`#${tblAdvertiser}`).dataTable({
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
            "sAjaxSource": "/Advertiser/Index?handler=AllAdvertiserDataTable",
            "aoColumns": [
                { "mDataProp": "shortName", "sClass": "center" },
                { "mDataProp": "longName", "sClass": "center" },
                { "mDataProp": "email", "sClass": "center" },
                {
                    "orderable": false,
                    "mRender": function (data, type, full) {
                        var actionString = "";
                        if (!full.isActive) {
                            actionString = `${actionString}  <a data-Id="${full.id}" data-name="${full.longName}" onClick="AdvertiserViewModel.Activate(this)" class="btn btn-sm text-light btn-secondary ladda-button " data-style="zoom-out" type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Activate">
                            <i class="fas fa-toggle-off"></i>
                            </a>`;
                        }
                        else {
                            actionString = `${actionString}  <a data-Id="${full.id}" data-name="${full.longName}" onClick="AdvertiserViewModel.Deactivate(this)" class="btn btn-sm text-light btn-success ladda-button " data-style="zoom-out" type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Deactivate">
                            <i class="fas fa-toggle-on"></i>
                            </a>`;
                        }
                        return actionString;
                    }
                },

                {
                    "orderable": false,
                    "mRender": function (data, type, full) {
                        var actionString = "";
                        if (full.isPaused) {
                            actionString = `${actionString}  <a data-Id="${full.id}" data-name="${full.longName}" onClick="AdvertiserViewModel.Resume(this)" class="btn btn-sm text-light btn-secondary ladda-button " data-style="zoom-out" type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Pause">
                            <i class="fas fa-toggle-off"></i>
                            </a>`;
                        }
                        else {
                            actionString = `${actionString}  <a data-Id="${full.id}" data-name="${full.longName}" onClick="AdvertiserViewModel.Pause(this)" class="btn btn-sm text-light btn-success ladda-button " data-style="zoom-out" type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Resume">
                            <i class="fas fa-toggle-on"></i>
                            </a>`;
                        }
                        return actionString;
                    }
                    , "visible": isAdmin
                },

                {
                    "orderable": false,
                    "sWidth": "15%",
                    "mRender": function (data, type, full) {
                        return `
                            <button onClick="AdvertiserViewModel.OpenAdvertiserModal(${parseInt(full.id)},'true')" class="btn btn-sm btn-info text-white ladda-button" data-style="zoom-out"
                                                                     type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="View"><i class="fa-solid fa-eye"></i></button>
                            <button onClick="AdvertiserViewModel.OpenAdvertiserModal(${parseInt(full.id)})" class="btn btn-sm btn-secondary text-white ladda-button" data-style="zoom-out"
                                                                     type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Edit"><i class="fas fa-edit"></i></button>
                            <button onClick="AdvertiserViewModel.OpenAdvertiserAPIModal(${parseInt(full.id)})" class="btn btn-sm btn-primary text-white ladda-button" data-style="zoom-out"
                                                                     type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Edit"><i class="fa-solid fa-key"></i></button>
                            <button  data-id="${full.id}" data-name="${full.longName}" onClick="AdvertiserViewModel.Delete(this)" class="btn btn-sm btn-danger text-white ladda-button ml-2" data-style="zoom-out"
                                         type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Delete"><i class="fas fa-trash"></i></button>`;
                    }
                }
            ],
            columnDefs: [
                {
                    "defaultContent": " ",
                    "targets": "_all"
                }],
            "autoWidth": true
        });
    };

    //#region Model
    this.OpenAdvertiserModal = function (advertiserId, readOnly = false) {
        let url = `/Advertiser/Index?handler=CreateOrEdit`;
        if (readOnly) url = `${url}&readOnly=true`;
        GetData({
            url: url,
            successHandler: openModalSuccess,
            data: {
                formId: "formAddAdvertiser",
                id: advertiserId,
            }
        });

    };
    this.UpdateSuccess = function (success, message) {
        if (success == false) {
            failAlert(message);
        }
        else {
            successAlert(message);
            closeCommonModel();
            refreshDataTable(tblAdvertiser);
        }
    };
    //#endregion

    //#region Actions
    this.Activate = (e) => actionConfirmBox({ text: "Activate", url: "/Advertiser?handler=Activate", elem: e, cb: this.ActionCB });
    this.Deactivate = (e) => actionConfirmBox({ text: "Deactivate", url: "/Advertiser?handler=Deactivate", elem: e, cb: this.ActionCB });
    this.Pause = (e) => actionConfirmBox({ text: "Pause", url: "/Advertiser?handler=Pause", elem: e, cb: this.ActionCB });
    this.Resume = (e) => actionConfirmBox({ text: "Resume", url: "/Advertiser?handler=Resume", elem: e, cb: this.ActionCB });
    this.Delete = (e) => actionConfirmBox({ url: "/Advertiser?handler=Delete", elem: e, cb: this.ActionCB });
    this.ActionCB = function (result, data, elem) {
        if (result.success) {
            SwalSuccess(result.message);
        }
        else {
            SwalError("Something went wrong!");
        }
        elem && elem.stop(); //lada button
        refreshDataTable(tblAdvertiser);
    };
    //#endregion

    //#region API Model
    this.OpenAdvertiserAPIModal = function (advertiserId) {
        GetData({
            url: `/Advertiser/Index?handler=CreateOrViewAPI`,
            successHandler: openModalSuccess,
            data: {
                formId: "formAdvertiserAPI",
                id: advertiserId,
            }
        });
    };

    this.APICB = function (success, message, data) {
        success && successAlert(message);
        !success && failAlert(message);
        if (data) {
            let api = JSON.parse(data);
            let clientIdElem = document.getElementById('ClientId'),
                clientSecretElem = document.getElementById('ClientSecret'),
                apiKeyElem = document.getElementById('ApiKey'),
                clientKeyElem = document.getElementById('ClientKey'),
                genrateBtnElem = document.getElementById('btnAdvertiserApiSave');
            if (clientIdElem) clientIdElem.value = api.clientId;
            if (clientSecretElem) clientSecretElem.value = api.clientSecret;
            if (apiKeyElem) apiKeyElem.value = api.apiKey;
            if (clientKeyElem) clientKeyElem.value = api.clientKey;
            if (genrateBtnElem) genrateBtnElem.style.display = 'none';
            disabledFormInputs("formAdvertiserAPI");
        }
    }

    //#endregion
}






