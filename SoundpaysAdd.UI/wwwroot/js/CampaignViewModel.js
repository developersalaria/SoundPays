var CampaignViewModel = new function () {
    var thisViewModel = this;
    var tblCampaign = "tblCampaign";
    this.CampaignListView = function (data) {
        $(`#${tblCampaign}`).dataTable({
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
            "sAjaxSource": "/Campaign/Index?handler=AllCampaignDataTable",
            "aoColumns": [
                { "mDataProp": "advertiserLongName", "sClass": "center", "visible": isAdmin },
                { "mDataProp": "shortName", "sClass": "center" },
                { "mDataProp": "longName", "sClass": "center" },
                { "mDataProp": "startDateDisplay", "sClass": "center" },
                { "mDataProp": "startTimeStandardString", "sClass": "center" },
                { "mDataProp": "stopDateDisplay", "sClass": "center" },
                { "mDataProp": "stopTimeStandardString", "sClass": "center" },
                { "mDataProp": "priority", "sClass": "center" },
                { "mDataProp": "cpm", "sClass": "center" },
                { "mDataProp": "minImpressions", "sClass": "center" },
                { "mDataProp": "maxImpressions", "sClass": "center" },
                {
                    "orderable": false,
                    "sWidth": "15%",
                    "mRender": function (data, type, full) {
                        if (!full.isActive) {
                            return `<a data-id="${full.id}" data-name="${full.shortName}" onClick="CampaignViewModel.Activate(this)" class="btn btn-sm text-light btn-secondary ladda-button " data-style="zoom-out" type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Activate">
                            <i class="fas fa-toggle-off"></i>
                            </a>`;
                        }
                        else {
                            return `<a data-id="${full.id}" data-name="${full.shortName}" onClick="CampaignViewModel.Deactivate(this)" class="btn btn-sm text-light btn-success ladda-button " data-style="zoom-out" type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Deactivate">
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
                            return `<a data-id="${full.id}" data-name="${full.shortName}" onClick="CampaignViewModel.Resume(this)" class="btn btn-sm text-light btn-secondary ladda-button " data-style="zoom-out" type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Pause">
                            <i class="fas fa-toggle-off"></i>
                            </a>`;
                        }
                        else {
                            return `<a data-id="${full.id}" data-name="${full.shortName}" onClick="CampaignViewModel.Pause(this)" class="btn btn-sm text-light btn-success ladda-button " data-style="zoom-out" type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Resume">
                            <i class="fas fa-toggle-on"></i>
                            </a>`;
                        }
                    }
                    , "visible": isAdmin
                },
                {
                    "orderable": false,
                    "mRender": function (data, type, full) {
                        return `
                            <button onClick="CampaignViewModel.OpenCampaignModal(${parseInt(full.id)},'true')" class="btn btn-sm btn-info text-white ladda-button" data-style="zoom-out"
                                                                     type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="View"><i class="fa-solid fa-eye"></i></button>
                            <button onClick="CampaignViewModel.OpenCampaignModal(${parseInt(full.id)})" class="btn btn-sm btn-secondary text-white ladda-button" data-style="zoom-out"
                                                                     type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Edit"><i class="fas fa-edit"></i></button>
                            <button  data-id="${full.id}" data-name="${full.shortName}" onClick="CampaignViewModel.Delete(this)" class="btn btn-sm btn-danger text-white ladda-button ml-2" data-style="zoom-out"
                                                                     type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Delete"><i class="fas fa-trash"></i></button>`;
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


    //open Campaign model
    this.OpenCampaignModal = function (campaignId, readOnly = false) {
        let url = `/Campaign/Index?handler=CreateOrEdit`;
        if (readOnly) url = `${url}&readOnly=true`;
        GetData({ url: url, successHandler: openModalSuccess, data: { formId: "formAddCampaign", id: campaignId, } });
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
            refreshDataTable(tblCampaign);
        }
    };

    //Actions
    this.Activate = (e) => actionConfirmBox({ text: "Activate", url: "/Campaign?handler=Activate", elem: e, cb: this.ActionCB });
    this.Deactivate = (e) => actionConfirmBox({ text: "Deactivate", url: "/Campaign?handler=Deactivate", elem: e, cb: this.ActionCB });
    this.Pause = (e) => actionConfirmBox({ text: "Pause", url: "/Campaign?handler=Pause", elem: e, cb: this.ActionCB });
    this.Resume = (e) => actionConfirmBox({ text: "Resume", url: "/Campaign?handler=Resume", elem: e, cb: this.ActionCB });
    this.Delete = (e) => actionConfirmBox({ url: "/Campaign?handler=Delete", elem: e, cb: this.ActionCB });
    this.ActionCB = function (result, data, elem) {
        if (result.success) {
            SwalSuccess(result.message);
        }
        else {
            SwalError("Something went wrong!");
        }
        elem && elem.stop(); //lada button
        refreshDataTable(tblCampaign);
    };
}

