var SegmentViewModel = new function () {
    var thisViewModel = this;
    var tblSegment = "tblSegment";
    this.SegmentListView = function (data) {
        $(`#${tblSegment}`).dataTable({

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
            "sAjaxSource": "/Segment/Index?handler=AllSegmentDataTable",
            "aoColumns": [
                { "mDataProp": "name", "sClass": "center" },
                { "mDataProp": "description", "sClass": "center" },
                {
                    "orderable": false,
                    "sWidth": "15%",
                    "mRender": function (data, type, full) {
                        if (!full.isActive) {
                            return `<a data-id="${full.id}" data-name="${full.shortName}" onClick="SegmentViewModel.Activate(this)" class="btn btn-sm text-light btn-secondary ladda-button " data-style="zoom-out" type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Activate">
                            <i class="fas fa-toggle-off"></i>
                            </a>`;
                        }
                        else {
                            return `<a data-id="${full.id}" data-name="${full.shortName}" onClick="SegmentViewModel.Deactivate(this)" class="btn btn-sm text-light btn-success ladda-button " data-style="zoom-out" type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Deactivate">
                            <i class="fas fa-toggle-on"></i>
                            </a>`;
                        }
                    }
                },
                {
                    "orderable": false,
                    "mRender": function (data, type, full) {
                        var actionString = "";
                        actionString = `<button onClick="SegmentViewModel.OpenSegmentModal(${parseInt(full.id)})" class="btn-lock-user btn btn-sm btn-secondary text-white ladda-button" data-style="zoom-out"
                                         type="button" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Edit"><i class="fas fa-edit"></i></button>
                                         <button data-id="${full.id}" onClick="SegmentViewModel.Delete(this,${parseInt(full.id)})" class="btn-lock-user btn btn-sm btn-danger text-white ladda-button ml-2" data-style="zoom-out"
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
    this.OpenSegmentModal = function (SegmentId) {
        GetData({
            url: `/Segment/Index?handler=CreateOrEdit`,
            successHandler: openModalSuccess,
            data: {
                formId: "formAddSegment",
                id: SegmentId,
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
            refreshDataTable(tblSegment);
        }
    };

    //delete
    this.Activate = (e) => actionConfirmBox({ text: "Activate", url: "/Segment?handler=Activate", elem: e, cb: this.ActionCB });
    this.Deactivate = (e) => actionConfirmBox({ text: "Deactivate", url: "/Segment?handler=Deactivate", elem: e, cb: this.ActionCB });
    this.Pause = (e) => actionConfirmBox({ text: "Pause", url: "/Segment?handler=Pause", elem: e, cb: this.ActionCB });
    this.Resume = (e) => actionConfirmBox({ text: "Resume", url: "/Segment?handler=Resume", elem: e, cb: this.ActionCB });
    this.Delete = (e) => actionConfirmBox({ url: "/Segment?handler=Delete", elem: e, cb: this.ActionCB });
    this.ActionCB = function (result, data, elem) {
        if (result.success) {
            SwalSuccess(result.message);
        }
        else {
            SwalError("Something went wrong!");
        }
        elem && elem.stop(); //lada button
        refreshDataTable(tblSegment);
    };
}
