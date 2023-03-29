/***************************************Summer text file upload  Loader ***************************************/
//Dropzone Configuration
//Dropzone.autoDiscover = false;
var fileUploadId = "";
var attachmentFileId = "";
var loaderImageUrl = "<h5 id='waitLoader'><img src='" + baseUrl + "/Content/images/ajaxLoader.gif' height='50px' /> Processing.Thank you for your patience.</h5>";
// use for start show  loader on image upload

//show loader
function showLoader() {
    $("#loader").show();
}

//hide loader
function hideLoader() {
    $("#loader").hide();
}


function showAjaxLoader() {
    showLoader();
}

//use for stop show loader on image upload
function stopAjaxLoader() {
    hideLoader();
}
/***************************************Summer text file upload  Loading ***************************************/


function failAlert(message) {
    // Display an error toast, with a title
    toastr.error(message, 'Fail');
}

function successAlert(heading, message) {

    // Display a success toast, with a title
    toastr.success(message, heading);
}

function warningAlert(heading, message) {
    // Display a success toast, with a title
    toastr.warning(message, heading);
}
/******************************************** Start  Swal Alert ************************************************/

function SwalSuccess(message) {

    swal(message, {
        icon: "success"
    });
}
//
function SwalError(message) {
    swal(message, {
        icon: "error"
    });
}
//
function swalConfirmBeforeAction(title, url, onSuccessMethod, data) {
    swal({
        title: title,
        icon: "warning",
        buttons: true,
        dangerMode: true
    })
        .then((willDelete) => {
            if (willDelete) {
                PostData({ url: url, successHandler: onSuccessMethod, data: data });
            }
        });
}

//#region
/******************************************** Start  ladda spinn wheel ************************************************/

function startLaddaSpinWheel(clickedButonId) {

    var btnLadda = $("#" + clickedButonId).ladda();
    btnLadda.ladda('start');
}

function stopLaddaSpinWheel(clickedButonId) {

    var btnLadda = $("#" + clickedButonId).ladda();
    btnLadda.ladda('stop');
}

/******************************************** End ladda spinn wheel ************************************************/
//#endregion


//#region
/************************************** *Common Js Method for Ajax  Start  ***************************************/

function PostData(obj) {
    let { url, successHandler, data, showBlackImage, spinWheelButtonId, colorChartId, extraInfo, elem } = obj;
    var isSpinWheelButtonExist = false;
    var isSpinWheelButtonExist = false;
    if (showBlackImage) {
        showAjaxLoader();
    }
    if (extraInfo !== null && extraInfo !== undefined && extraInfo !== "") {
        $(".modal-hidden-input").val(extraInfo);
    }
    if (spinWheelButtonId !== null && spinWheelButtonId !== undefined && spinWheelButtonId !== "") {
        isSpinWheelButtonExist = true;
        startLaddaSpinWheel(spinWheelButtonId);
    }
    if (colorChartId !== null && colorChartId !== undefined && colorChartId !== "") {
        ColorChartId = colorChartId;
    }
    $.ajax({
        type: "POST",
        url: baseUrl + url,
        data: data,
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function (result) {
            if (isSpinWheelButtonExist) {
                stopLaddaSpinWheel(spinWheelButtonId);
            }
            if (showBlackImage) {
                stopAjaxLoader();
            }

            successHandler(result, data, elem);
        },
        error: function () {
            //your code here
            if (showBlackImage) {
                stopAjaxLoader();
            }
        }

        //global: showBlackImage
    });
}

//
function GetData(obj) {
    let { url, successHandler, data, showBlackImage, spinWheelButtonId, elem } = obj;
    var isSpinWheelButtonExist = false;
    if (spinWheelButtonId !== null && spinWheelButtonId !== undefined && spinWheelButtonId !== "") {
        isSpinWheelButtonExist = true;
        startLaddaSpinWheel(spinWheelButtonId);
    }
    if (showBlackImage === null) {
        showBlackImage = true;
    }
    $.ajax({
        type: "GET",
        url: baseUrl + url,
        data: data,
        success: function (result) {
            if (isSpinWheelButtonExist) {
                stopLaddaSpinWheel(spinWheelButtonId);
            }
            successHandler(result, data, elem);
        },
        global: showBlackImage
    });
}

/************************************** *Common Js Method for Ajax  End   ***************************************/
//#endregion


function ResetUnobtrusiveValidation(form) {
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);

}
/************************************** *Bootstrap Model  start ***************************************/
$(document).on('show.bs.modal', '.modal', function () {
    var zIndex = 1040 + (10 * $('.modal:visible').length);
    $(this).css('z-index', zIndex);
    setTimeout(function () {
        $('.modal-backdrop').not('.modal-stack').css('z-index', zIndex - 1).addClass('modal-stack');
    }, 0);
});


function addEventsToCommonModal() {
    var comModal = document.getElementById("common-modal-pop-up");
    if (comModal) {
        comModal.addEventListener('hidden.bs.modal', function (event) {
            $.each(window?.intlTelInputGlobals?.instances, (idx, elem) => {
                window.intlTelInputGlobals.instances[idx].destroy();
            });
            if (typeof init === "function") {
                //call init method
                init(true);
            }
        });
        comModal.addEventListener('shown.bs.modal', function (event) {
            setTimeout(() => {
                $.each(window?.intlTelInputGlobals?.instances, (idx, elem) => {
                    let intelInstance = window?.intlTelInputGlobals?.instances[idx];
                    if (intelInstance?.a?.value != '' && !intelInstance.isValidNumber()) {
                        let errorElem = intelInstance?.a?.parentElement?.nextElementSibling,
                            errorClassElem = errorElem.classList;
                        errorElem.innerHTML = "Invalid number";
                        errorClassElem.remove("d-none");
                        errorClassElem.add("d-block");
                    }
                });
            }, 500);
            if (typeof init === "function") {
                //call init method
                init(true);
            }
        })
    }
}

//#region common modal
function openCommonModel(data, formId, destinationDivId) {

    var targetDiv = destinationDivId != null ? destinationDivId : "common-modal-pop-up";

    var prevInstance = bootstrap.Modal.getInstance(document.getElementById(targetDiv))
    if (prevInstance != null) {
        prevInstance?.hide();
        prevInstance?.dispose();
    }

    $("#" + targetDiv).html(data);
    window.commonModal = new bootstrap.Modal(document.getElementById(targetDiv), {
        keyboard: false
    })
    window.commonModal.show();

    ResetUnobtrusiveValidation($("#" + formId));
    //get the validator setting from form and if ignoreHidden=true add this code
    var igonreSettingsValue = ($("#" + formId).data("ignore-settings"));
    if ((igonreSettingsValue !== "" || igonreSettingsValue !== "undefined" || igonreSettingsValue !== null)) {
        if (igonreSettingsValue === true) {
            $("#" + formId).data("validator").settings.ignore = "";
        }
    }

    var initSelect = ($("#" + formId).data("init-select"));
    if ((initSelect !== "" || initSelect !== "undefined" || initSelect !== null)) {
        if (initSelect === true) {
            $("select.dropDownSelect2").select2({
                placeholder: 'Select an option',
                width: 'resolve',
                theme: 'bootstrap-5',
                dropdownParent: $("#" + formId)
            });
            InitCasCadingDropdown()
        }
    }

}
function closeCommonModel() {
    window?.commonModal?.hide();
}
function closeCommonModelAndReload() {
    window?.commonModal?.hide();
    window.location.reload();
}

//#endregion


function refreshDataTable(tableId) {
    if (tableId !== null && tableId !== undefined && tableId !== "") {
        //second parameter false will keep user on the same page instead of first after load in pagination.
        $('#' + tableId).DataTable().ajax.reload(null, false);
    }
}

//success method 
function openModalSuccess(result, data, elem = null) {
    var destinationDivId = null;

    if (data.parentModalDiv != null && typeof data.parentModalDiv !== 'undefined') {
        destinationDivId = data.parentModalDiv;
    }


    openCommonModel(result, data.formId, destinationDivId);
    InitAjaxFormPost(data.formId);
}


//#region initPartialViewRender
/***************************************** Button Partial call Feature Starts *****************************************/
function initPartialViewRender() {
    $("a[data-load-partial-view='true']").on('click', function () {
        var data = $(this).data();
        // EVERY TIME RELOAD DATA 
        if (data.reloadDom || $("#" + data.domAppendIdentifier).children().length === 0) {
            GetData({ url: data.url, successHandler: partialViewRenderSuccess, data: { domAppendIdentifier: data.domAppendIdentifier } });
        }
        else { //         
            $("#" + data.domAppendIdentifier).siblings().hide();
            $("#" + data.domAppendIdentifier).show();
        }

    });
}
//success method 
function partialViewRenderSuccess(result, data) {
    // add html and show 
    $("#" + data.domAppendIdentifier).empty().html(result).tab('show');

    $("#" + data.domAppendIdentifier).siblings().hide();
    $("#" + data.domAppendIdentifier).show();

}
/***************************************** Button Partial call Feature Ends *****************************************/

//#endregion initPartialViewRender


/***************************************** DropDown Select2 Feature Starts *****************************************/
var config = {
    placeholder: 'Select an option',
    width: 'resolve',
    theme: 'bootstrap-5',
};
//
function InitCustomDropdown() {
    $("select.dropDownSelect2").select2(config);
}

function InitCasCadingDropdown() {
    $("select[data-select-parent='true']").on('change', function () {
        var parentSelction = $(this);
        var url = parentSelction.data("child-data-url");
        var selectedId = parentSelction.data("select-parent-id");
        var childSelector = parentSelction.data("child-selector");
        // post data 
        var Id = parseInt($('#' + selectedId).val());
        //check is Id null
        if (isNaN(Id)) { Id = -1; }
        showAjaxLoader();
        $.ajax({
            url: baseUrl + url + Id,
            type: 'GET',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (data) {
                let newOptions = '';
                $.each(data, (k, v) => newOptions += `<option value="${v.value}">${v.text}</option>`);
                let formId = document.getElementById(selectedId)?.closest("form")?.id;
                if (formId) {
                    config.dropdownParent = $(`#${formId}`);
                }
                $(`#${childSelector}`).select2('destroy').html(newOptions).prop("disabled", false).select2(config);
                stopAjaxLoader();
                return false;
            },
            failure: function (result) {
                stopAjaxLoader();
            },
            error: function (error) {
                stopAjaxLoader();
                console.log(error);
                alert("Error getting subcategories" + error.statusText);
                $('.result').html(error.responseText);
            }
        });

    });
}
//
function InitCustomDropdownById(id) {
    $("#" + id).select2(config);
}
//accept list of select
function ReInitCustomDropdownById(id, data) {
    var newOptions = '';
    $.each(data, function (key, value) {
        newOptions += '<option value="' + value.value + '">' + value.text + '</option>';
    });
    $("#" + id).select2('destroy').html(newOptions).prop("disabled", false)
        .select2(config);
}


/***************************************** DropDown Select2 Feature Ends *****************************************/
/********Document Ready function Starts********/

$(document).ready(function () {
    InitilizeDatePicker();
    InitializeTimePicker();
    //InitCustomDropdown();
    //initPartialViewRender();
    //InitCasCadingDropdown();
    InitAjaxFormPost();
    //InitilizeLightbox();

    //InitSummerNoteFileUpload();

    addEventsToCommonModal();


});
/********Document Ready function Ends********/

/************************************** *AJAX POST Starts***************************************/

function InitAjaxFormPost(formId) {
    var finder = $("form[data-post-type='ajax'][ method=\"post\"]").find('.save-form-button');
    if (formId !== undefined) {
        finder = $("#" + formId).find('.save-form-button');
    }
    finder.on("click", function () {

        var button = $(this);
        var form = $(this).parents('form:first');
        var successFunction = form.data("success-method");
        var resetForm = form.data("clear-form");
        var isValid = form.validate().form();
        // check for ladda button option
        var laddaButton = form.data("ladda-button");
        if (laddaButton === "" ||
            laddaButton === "undefined" ||
            laddaButton === null ||
            laddaButton === undefined) {
            laddaButton = button.attr('id');
        }

        //checking phone number validations
        var phoneError = [];
        $.each(window?.intlTelInputGlobals?.instances, (idx, elem) => {
            if (window.intlTelInputGlobals.instances[idx].a.value != '') {
                phoneError.push(window.intlTelInputGlobals.instances[idx].isValidNumber());
            }
        });
        //
        if (phoneError.length) {
            isValid = !phoneError.some(x => x == false);
        }

        if (isValid) {
            startLaddaSpinWheel(laddaButton);

            $.ajax({
                type: 'POST',

                url: baseUrl + form.attr('action'),
                data: form.serialize(),
                success: function (data) {
                    $(button).removeAttr("disabled");
                    if (resetForm === true && (data.success === undefined || data.success || data.success === null)) {
                        (form).trigger("reset");
                    }
                    if (trim(successFunction) !== "") {
                        let resultData = data?.data ? JSON.stringify(data?.data) : null;
                        var functionName = `${successFunction}(${data.success},"${data.message}",'${resultData}')`;
                        eval(functionName);
                    }

                    var resetDropZone = form.data("clear-dropzone");

                    if (resetDropZone != undefined && resetDropZone != null) {

                        closeCommanModalWithDropzone(resetDropZone)

                    }
                    //data - clear - dropzone="dropzoneSliderLogo-image"



                    //stop wheel
                    stopLaddaSpinWheel(laddaButton);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    failAlert(errorThrown);
                    //stop wheel
                    stopLaddaSpinWheel(laddaButton);
                }
            });
            return false;
        }
        return false;
    });
}

/************************************** *AJAX POST Ends***************************************/


/************************************** *AJAX POST  with file upload starts***************************************/

function InitAjaxFormWithFilesPost() {
    $("form[data-post-type-with-fileUpload='ajax'][ method=\"post\"]").find(':submit').on("click", function () {
        var button = $(this);
        var form = $(this).parents('form:first');
        var successFunction = form.data("success-method");
        var resetForm = form.data("clear-form");
        var apiPost = form.data("api-post");
        var url = form.attr('action');
        //if it is api post request
        if (apiPost !== null && apiPost !== undefined && apiPost) {
            url = apiEndPoint + url;
        }
        var isValid = form.validate().form();
        if (isValid) {
            $(button).attr("disabled", true);
            var fd = new FormData();
            var totalFiles = $('input[type="file"]').length;
            if (totalFiles > 0) {
                for (var inputFile = 0; inputFile < totalFiles; inputFile++) {
                    var file_data = $('input[type="file"]')[inputFile].files; // for multiple files
                    for (var i = 0; i < file_data.length; i++) {
                        fd.append("file_" + inputFile, file_data[i]);
                    }
                }
            }
            var other_data = form.serializeArray();

            $.each(other_data, function (i, field) {
                if (field.name === "fileUploaded") {
                    //do something
                }
                else {
                    fd.append(field.name, field.value);
                }
            });
            $.ajax({
                type: 'POST',
                url: baseUrl + url,
                data: fd,
                processData: false,
                contentType: false,
                success: function (data) {
                    $(button).removeAttr("disabled");
                    if (resetForm === true && (data.success === undefined || data.success || data.success === null)) {
                        (form).trigger("reset");
                    }
                    if (trim(successFunction) !== "") {
                        var functionName = successFunction + "(" + data.success + " , '" + data.message + "','" + data.IsResultSuccessfull + "' )";
                        eval(functionName);
                    }
                    //stop wheel
                    stopLaddaSpinWheel(button.attr('id'));
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    failAlert(errorThrown);
                    //stop wheel
                    stopLaddaSpinWheel(button.attr('id'));
                }
            });
            return false;
        }
        else {
            $(form).effect("shake", { distance: 6, times: 2 }, 20);
        }
        return false;
    });
}
/************************************** *AJAX POST  with file upload ends***************************************/


/************************************** *Trim Function Starts***************************************/

function trim(stringToTrim) {
    if (!isStringValid(stringToTrim)) return "";
    return stringToTrim.replace(/^\s+|\s+$/g, "");
}
function ltrim(stringToTrim) {
    if (!isStringValid(stringToTrim)) return "";
    return stringToTrim.replace(/^\s+/, "");
}
function rtrim(stringToTrim) {
    if (!isStringValid(stringToTrim)) return "";
    return stringToTrim.replace(/\s+$/, "");
}
function isStringValid(str) {
    if (str === "") return false;
    if (str === undefined) return false;
    return true;
}
/************************************** *Trim Function Ends***************************************/
function InitSummerNoteFileUpload() {
    $('.summernote-fileupload').each(function () {
        var editor =  // 1st change: will need this variable later
            $(this).summernote({
                height: 100,
                focus: false,
                callbacks: { // 2nd change - onImageUpload inside of "callbacks"
                    onImageUpload: function (files) {
                        // 3rd change - dont need other params
                        var formData = new FormData();
                        formData.append("file", files[0]);
                        //start summer Note File Upload Loading
                        showAjaxLoader();
                        $.ajax({
                            url: baseUrl + $(this).data('url'),
                            data: formData,
                            type: 'POST',
                            cache: false,
                            contentType: false,
                            processData: false,
                            success: function (imageUrl) {
                                if (!imageUrl) {
                                    // stop Summer Note File Upload Loading
                                    stopAjaxLoader();
                                    return;
                                }
                                // 4th change - create img element and add to document
                                $.each(imageUrl.message, function (index, value) {
                                    resultUrl = value.url;
                                    stopAjaxLoader();
                                });

                                var imgNode = document.createElement('img');
                                imgNode.src = resultUrl;
                                editor.summernote('insertNode', imgNode);
                            },
                            error: function () {
                                // stop Summer Note File Upload Loading
                                stopAjaxLoader();
                            }
                        });
                    }
                }
            });
    });
}

//method to initialize datePicker
//use css class  soundpays-datepicker
// data attributes in kebab case (i.e. data-allow-future-date)
//allowFutureDate ->  true/false
//allowPastDate ->  true/false
//setCurrentDate ->  true/false
//setDefaultDate ->  true/false
//isRangePicker  ->  true/false (required for range()
//endSelectorId -> id of element (required for range add on start)
//startSelectorId -> id of element (required for range add on end)

function InitilizeDatePicker() {
    let defaultOptions = { autoclose: true, dateFormat: 'dd-mm-yy' };
    $('.soundpays-datepicker').each(function () {
        let {
            allowFutureDate = true,
            allowPastDate = true,
            setCurrentDate = true,
            setDefaultDate = true,
            isRangePicker = false,
            endSelectorId = null,
            startSelectorId = null,
        } = $(this).data();
        let datePickerValue = $(this).val();
        let datePickerValueParsed;
        if (datePickerValue != "") {
            let dattArray = datePickerValue.split("-");// mm-dd-yy
            if (dattArray.length > 0) {
                datePickerValueParsed = new Date(`${dattArray[2]}/${dattArray[0]}/${dattArray[1]}`);
            }
        }
        //init datepicker
        $(this).datepicker(defaultOptions);
        var parsedDate = datePickerValue != "" ? datePickerValueParsed : new Date();
        $(this).datepicker("setDate", parsedDate);
        if (!allowFutureDate) {
            $(this).datepicker("option", "maxDate", parsedDate);
        }
        if (!allowPastDate) {
            $(this).datepicker("option", "minDate", parsedDate);
        }
        if (datePickerValue !== "" && datePickerValue == "01-01-0001" && setCurrentDate) {
            $(this).datepicker("setDate", parsedDate);
        }
        if (datePickerValue !== "" && datePickerValue == "01-01-0001" && setDefaultDate) {
            $(this).datepicker("setDate", parsedDate);
        }
        if (isRangePicker) {
            if (endSelectorId != null) {
                let startElem = $(this), endElem = $(`#${endSelectorId}`);
                startElem?.datepicker("option", "onSelect", (selectedDate) => {
                    endElem?.datepicker("option", "minDate", selectedDate || new Date());
                });
            }
            if (startSelectorId != null) {
                let endElem = $(this), startElem = $(`#${startSelectorId}`);
                if (datePickerValue != "") {
                    let dattArray = startElem?.val().split("-"); // dd-mm-yy
                    if (dattArray.length > 0) {
                        datePickerValueParsed = new Date(`${dattArray[2]}/${dattArray[1]}/${dattArray[0]}`); //yy-mm-dd
                    }
                }
                endElem?.datepicker("option", "minDate", datePickerValueParsed);
            }
        }
    });
}

//use css class  soundpays-number-only to make input number only
function InitilizeNumberOnly() {
    document.querySelectorAll('.soundpays-number-only')?.forEach(e => {
        let control = e;
        control.addEventListener("keypress", (evt) => {
            evt = (evt) ? evt : window.event;
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if ((charCode > 31 && (charCode < 48 || charCode > 57))) {
                evt.preventDefault();
                return false;
            }
        }, false);
    });
}

//method to initialize lightbox
function InitilizeLightbox() {
    $(document).on('click', '[data-toggle="lightbox"]', function (event) {
        event.preventDefault();
        $(this).ekkoLightbox({
            alwaysShowClose: true,
            selector: ".child__element"
        });
    });
}

/*method for generating PDF*/
//save bytes of generated pdf
function saveByteArray(reportName, byte) {
    var blob = new Blob([byte], { type: "application/pdf" });
    var link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    var fileName = reportName;
    link.download = fileName;
    link.click();
}

// convert base64 to array Buffer sent by server
function base64ToArrayBuffer(base64String) {
    var binaryString = window.atob(base64String);
    var binaryLen = binaryString.length;
    var bytes = new Uint8Array(binaryLen);
    for (var i = 0; i < binaryLen; i++) {
        var ascii = binaryString.charCodeAt(i);
        bytes[i] = ascii;
    }
    return bytes;
}

//get formatted time
function getFormattedTime() {
    var today = new Date();
    var y = today.getFullYear();
    // JavaScript months are 0-based.
    var m = today.getMonth() + 1;
    var d = today.getDate();
    var h = today.getHours();
    var mi = today.getMinutes();
    var s = today.getSeconds();
    return y + "-" + m + "-" + d + "-" + h + "-" + mi + "-" + s;
}

/***********************************************Phone validation******************************************************/

function convertPhoneToDashedNumber(control) {
    // get phone number
    var inputValue = $(control).val();
    // remove hyphen in phone number
    inputValue = inputValue.replace(/-/g, "");
    // regex for UK and check UK validate numbers
    var regexUK = /^((\+44\s?\d{4}|\(?\d{5}\)?)\s?\d{6})|((\+44\s?|0)7\d{3}\s?\d{6})$/;
    // check if UK phone number is not valid and length of phone number is greater than 9
    if (!inputValue.match(regexUK) && inputValue.length > 9) {
        //place hyhpen in phone number
        inputValue = inputValue
            .match(/\d*/g).join('')
            .match(/(\d{0,3})(\d{0,3})(\d{0,4})/).slice(1).join('-')
            .replace(/-*$/g, '');
    }
    $(control).val(inputValue);
}

/***********************************************Select price input text******************************************************/

function selectPriceInputText(control) {
    if ($(control).val() === 0.00) {
        $(control).select();
    }
}

/***********************************************full price format******************************************************/

function fullPriceFormat(control) {
    // take input value
    var inputValue = $(control).val();
    //split string input value into array
    var getInputArray = inputValue.split(".");
    // convert first array value to integer
    getInputArray[0] = parseInt(getInputArray[0]);
    // check first array value
    if (getInputArray[0] === "" || isNaN(getInputArray[0]) || getInputArray[0] === null || getInputArray[0] === 0) {
        getInputArray[0] = "0";
    }
    // check second array value 
    if (getInputArray[1] === "" || getInputArray[1] === "undefined" || getInputArray[1] === null || getInputArray[1] === 0) {
        getInputArray[1] = "00";
    }
    //check length of second array value
    if (getInputArray[1].length === 1) {
        getInputArray[1] = getInputArray[1] + "0";
    }
    // place converted values in input field
    $(control).val(getInputArray[0] + "." + getInputArray[1]);
}

/**********************************Hide button************************************/

function hideButton(identifier) {
    $(identifier).hide();
}


/************************ prevent form button to submit when enter press**************/
function preventFormSubmitOnEnterPress() {

    // prevent enter key
    $(window).on('keyup keypress', function (event) {
        //check submit button focused or not
        var hasFocus = $(".save-btn").is(':focus');
        if (!hasFocus) {
            if (event.keyCode === 13) {
                event.preventDefault();
                return false;
            }
        }
    });
}

//Sort dropdowns client side.
function sortSelect(selector, skipFirst) {
    var options = (skipFirst) ? $(selector + ' option:not(:first)') : $(selector + ' option');
    var arr = options.map(function (_, o) {
        return {
            t: $(o).text(), v: o.value, s: $(o).prop('selected')
        };
    }).get();
    arr.sort(function (o1, o2) {
        var t1 = o1.t.toLowerCase(), t2 = o2.t.toLowerCase();
        return t1 > t2 ? 1 : t1 < t2 ? -1 : 0;
    });
    options.each(function (i, o) {
        o.value = arr[i].v;
        $(o).text(arr[i].t);
        if (arr[i].s) {
            $(o).attr('selected', 'selected').prop('selected', true);
        } else {
            $(o).removeAttr('selected');
            $(o).prop('selected', false);
        }
    });
}
//-------------------------------------- Spin  On every Ajax request in the System -------------------

function spinOn() {
    $("#spinner").show();
}

function spinOff() {
    $("#spinner").hide();
}

$(document).ajaxSend(function () {
    spinOn();
});

$(document).ajaxStop(function () {
    spinOff();
});



//method to initialize fileuploader
function InitilizefileUploaderById(id) {
    let elem = $(`#${id}`);
    let { destinationControl, purpose, attachmentContainer, assetId, fileType, url, deleteUrl, recordId, attachmentName } = elem.data();
    let rowNumber = $(`#${attachmentContainer} > tbody > tr`).length, acceptedType = "", maxImgFilesize = "", customErrorMessage = "";
    if (fileType == "video" || fileType == "Video") {
        acceptedType = '.mov,.MP4,.wmv,.avi';
        customErrorMessage = "Uploaded file is not a valid video. Only .mov,.MP4,.wmv,.avi files are allowed ";
        maxImgFilesize = parseInt(100);
    }
    else if (fileType == "audio" || fileType == "Audio") {
        acceptedType = 'audio/*';
        customErrorMessage = "Uploaded file is not a valid audio. Only .MP3, .wav files are allowed.";
        maxImgFilesize = parseInt(25);
    }
    else if (fileType == "image" || fileType == "Image") {
        acceptedType = 'image/*';
        customErrorMessage = "Uploaded file is not a valid image. Only JPG, PNG and GIF files are allowed.";
        maxImgFilesize = parseInt(25);
    }
    else if (fileType == "doc" || fileType == "Doc") {
        acceptedType = '.doc,.docx,';
        customErrorMessage = "Uploaded file is not a valid doc. Only .doc and .docx files are allowed.";
        maxImgFilesize = parseInt(25);
    }
    else {
        if (fileType) {
            if (fileType.includes("image")) {
                acceptedType += 'image/*,';
            }
            if (fileType.includes("doc")) {
                acceptedType += '.doc,.docx,';
            }
            if (fileType.includes("pdf")) {
                acceptedType += '.pdf,';
            }
            if (fileType.includes("csv")) {
                acceptedType += '.xls,.xlsx,.csv';
            }
        }
        else {
            acceptedType = "audio/*,image/*,.psd,.pdf,.rar,.mov,.zip,.MP4,.MTS,.AI";
        }
    }
    new Dropzone(
        `#${id}`,
        {
            url: `${baseUrl}${url}`,// Set the url
            params: {
                index: rowNumber,
                recordId: recordId,
                deleteUrl: deleteUrl,
                attachmentListName: attachmentName
            },
            paramName: "files", // The name that will be used to transfer the file
            maxFilesize: 110, // MB
            clickable: true,
            maxFiles: 3,
            acceptedFiles: acceptedType,
            enqueueForUpload: false,
            previewsContainer: `#${id}`,
            init: function () {
                this.on("processing", function (file) {
                });
                this.on("maxfilesexceeded",
                    function (file) {
                        this.removeAllFiles();
                        this.addFile(file);
                    });
                this.on("success",
                    function (file, response) {
                        //bind an html to show a link and delete button below thw file uploader
                        if (response.success) {
                            if (response.attachment && response.attachment != null) {
                                $(`#${attachmentContainer} > tbody:last-child`).append(response.attachment);
                            }
                        }
                        else {
                            toastr.error("Something Went Wrong!", 'Error!');
                            //remove all the instance of the selected fileUploader and re-initialize it
                            $.each(Dropzone.instances, function (index, value) {
                                if (value != undefined && value.element != undefined && value.element.id == id) {
                                    //destroy
                                    value.destroy();
                                    //reinitialize
                                    InitilizefileUploaderById(value.element.id)
                                }
                            });

                        }
                    });
                this.on("error",
                    function (data, errorMessage, xhr) {
                        var isBlunderError = (errorMessage.includes('files of this type.') || errorMessage.includes('File is too big')) ? true : false;
                        if (errorMessage.includes('files of this type.') && customErrorMessage != "") {
                            toastr.error(customErrorMessage, 'Error!');
                        }
                        else {
                            toastr.error(errorMessage, 'Error!');
                        }
                        if (isBlunderError) {
                            //reinitialize file uploader
                            //remove all the instance of the selected fileUploader and re-initialize it
                            $.each(Dropzone.instances, function (index, value) {
                                if (value != undefined && value.element != undefined && value.element.id == id) {
                                    //destroy
                                    value.destroy();
                                    //reinitialize
                                    InitilizefileUploaderById(value.element.id)
                                }
                            });

                        }
                    });
            }
        });
};

function InitilizeMultifileUploaderById(id) {
    let elem = $(`#${id}`);
    let { destinationControl, purpose, attachmentContainer, assetId, fileType, url, deleteUrl, recordId } = elem.data();
    let rowNumber = $(`#${attachmentContainer} > tbody > tr`).length, acceptedType = "", maxImgFilesize = "", customErrorMessage = "";
    if (fileType == "video" || fileType == "Video") {
        acceptedType = '.mov,.MP4,.wmv,.avi';
        customErrorMessage = "Uploaded file is not a valid video. Only .mov,.MP4,.wmv,.avi files are allowed ";
        maxImgFilesize = parseInt(100);
    }
    else if (fileType == "audio" || fileType == "Audio") {
        acceptedType = 'audio/*';
        customErrorMessage = "Uploaded file is not a valid audio. Only .MP3, .wav files are allowed.";
        maxImgFilesize = parseInt(25);
    }
    else if (fileType == "image" || fileType == "Image") {
        acceptedType = 'image/*';
        customErrorMessage = "Uploaded file is not a valid image. Only JPG, PNG and GIF files are allowed.";
        maxImgFilesize = parseInt(25);
    }
    else if (fileType == "doc" || fileType == "Doc") {
        acceptedType = '.doc,.docx,';
        customErrorMessage = "Uploaded file is not a valid doc. Only .doc and .docx files are allowed.";
        maxImgFilesize = parseInt(25);
    }
    else {
        if (fileType) {
            if (fileType.includes("image")) {
                acceptedType += 'image/*,';
            }
            if (fileType.includes("doc")) {
                acceptedType += '.doc,.docx,';
            }
            if (fileType.includes("pdf")) {
                acceptedType += '.pdf,';
            }
            if (fileType.includes("csv")) {
                acceptedType += '.xls,.xlsx,.csv';
            }
        }
        else {
            acceptedType = "audio/*,image/*,.psd,.pdf,.rar,.mov,.zip,.MP4,.MTS,.AI";
        }
    }
    new Dropzone(
        $(`#${id}`),
        {
            url: baseUrl + url,
            params: {
                index: rowNumber,
                recordId: recordId,
                deleteUrl: deleteUrl
            },// Set the url
            paramName: "files", // The name that will be used to transfer the file
            maxFilesize: 100, // MB
            clickable: true,
            maxFiles: 10,
            acceptedFiles: acceptedType,
            enqueueForUpload: false,
            addRemoveLinks: true,
            parallelUploads: 3,
            uploadMultiple: true,
            previewsContainer: `#${id}`,
            init: function () {
                this.on("processing", function (file) {
                });
                this.on("maxfilesexceeded",
                    function (file) {
                        this.removeAllFiles();
                        this.addFile(file);
                    });
                this.on("successmultiple",
                    function (file, responseText) {
                        //bind an html to show a link and delete button below thw file uploader
                        if (response.success) {
                            if (response.attachment && response.attachment != null) {
                                $(`#${attachmentContainer} > tbody:last-child`).append(response.attachment);
                            }
                        }
                        else {
                            toastr.error("Something Went Wrong!", 'Error!');
                            //remove all the instance of the selected fileUploader and re-initialize it
                            $.each(Dropzone.instances, function (index, value) {
                                if (value.element.id == id) {
                                    //destroy
                                    value.destroy();
                                    //reinitialize
                                    InitilizeMultifileUploaderById(value.element.id)
                                }
                            });

                        }
                    });
                this.on("error",
                    function (data, errorMessage, xhr) {
                        toastr.error(errorMessage, 'Error!');
                    });
            }
        });
}



/* show the edit assets image*/
function ImagePreview(id) {


    window.open(baseUrl + '/Attachment/ImagePreview/' + id);
}

/* delete selected attachment/uploaded file */
function DeleteSingleAttachment(controlElement) {
    var data = $(controlElement).data();
    if (data.destinationControl != null && data.destinationControl != undefined && data.destinationControl != "") {
        fileUploadId = data.destinationControl;
    }
    swal({
        title: "Are you sure?",
        text: "Please confirm you wish to delete " + data.name,
        icon: "warning",
        buttons: true,
        dangerMode: true
    })
        .then((willDelete) => {
            if (willDelete) {
                var propertyName = data.attributeName;
                if (data.attachmentId == undefined || data.attachmentId == null || data.attachmentId == "") {
                    data.attachmentId = "0";
                }
                attachmentFileId = data.attachmentId;
                let obj = {};
                if (propertyName == undefined || propertyName == null || propertyName == "") {
                    obj = { url: "/attachment/deleteAttachment", successHandler: DeleteSingleAttachmentSuccess, data: { name: data.fileName, assetId: data.attachmentId } };
                }
                else {
                    obj = { url: "/attachment/deleteattachmentbyid", successHandler: DeleteSingleAttachmentSuccess, data: { attachmentId: data.attachmentId } };
                }
                PostData(obj);
            }
        });
}

/*success method for deleting single attachment */
function DeleteSingleAttachmentSuccess() {
    if (fileUploadId != "" && fileUploadId != null && fileUploadId != undefined) {
        var attachmentId = $("#" + fileUploadId).attr("data-destination-control");
        //delete it's id from attachmentId control
        if (attachmentId != null && attachmentId != undefined && attachmentId != "") {
            $("#" + attachmentId).val("");
        }

        //remove all the instance of the selected fileUploader and re-initialize it
        $.each(Dropzone.instances, function (index, value) {
            if (value.element.id == fileUploadId) {
                //destroy 
                value.destroy();
                //reinitialize 
                InitilizefileUploaderById(value.element.id)
            }
        });

        //show the drag and drop text 
        $("#" + fileUploadId).removeAttr("style");
        SwalSuccess("File Deleted Successfully!");
        $("#attachmentFile" + attachmentFileId).remove();
        // $(".attachmentDiv").remove();
        //show the drag and drop option
        $("#" + fileUploadId + " .dz-message").css("display", "block");
    }

}

//delete attachment
function DeleteAttachment(index) {
    swal({
        title: "Are you sure?",
        text: "Please confirm you wish to delete.",
        icon: "warning",
        buttons: true,
        dangerMode: true
    })
        .then((willDelete) => {
            if (willDelete) {
                $("#rowAttachment_" + index).remove();
                SwalSuccess("File Deleted Successfully!");
            }
        });
}

/*****************prevent the entire document from drag and drop files*********************/
function preventDragAndDropFeatures() {
    $(document).bind('drop dragover', function (e) {
        e.preventDefault();
    });
}

//method to initialize datePicker

//This will return the html for file uploader 
function mapFileuploaderHTML(counter, fileName, location) {
    //get the attachment List and show one by one 
    return '<div class="row mb-2" id="attachmentFile' + counter + '"> ' +
        '<input class="AttachmentMediaMappingId"  id="MediaItemList_' + counter + '__SaveLocation" name="MediaItemList[' + counter + '].SaveLocation" type="hidden" value="' + location + '"> ' +
        '<input class="AttachmentMediaMappingId"  id="MediaItemList_' + counter + '__Id" name="MediaItemList[' + counter + '].Id" type="hidden" value="0"> ' +
        '<input class="MediaItemId" data-val="true" data-val-required="The IsDeleted field is required." id="MediaItemList_' + counter + '__IsDeleted" name="MediaItemList[' + counter + '].IsDeleted" type="hidden" value="False">' +
        '<div class="col-xl-8 col-md-6 col-xs-6 text-left"> <i class="fa fa-file text-success"></i> ' +
        '<a class="title" target="_blank" href="' + location + '">' + fileName + '</a></div><div class="col-xl-4 col-md-6 col-xs-6 text-right">' +
        '<a class="btn btn-sm btn-danger" id="attachmentFile-' + counter + '"  style="cursor:pointer;"   title="delete file" data-deleteattachmentbutton="true" onclick="AttachmentViewModel.deletedSelectedAttachmentFile(' + counter + ')"><i class="fa fa-trash-o fa-1x"></i></a></div></div>';
}


//allow only numeric value for value
$(".allow_numeric").on("keypress keyup blur", function (e) {
    //if the letter is not digit then display error and don't type anything
    if (e.which !== 8 && e.which !== 0 && (e.which < 48 || e.which > 57)) {
        return false;
    }
});

//allow only numeric value for value
$(".allow_decimal").on("keypress keyup blur", function (e) {
    //if the letter is not digit then display error and don't type anything
    if ((event.which !== 46 || $(this).val().indexOf('.') !== -1) && (event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }
});

// set decimal text box default value  0.00
$(".setDefaultDecimalValue").each(function (e) {
    if ($(this).val() === 0 || $(this).val() === '0' || $(this).val() === null
        || $(this).val() === undefined
        || $(this).val() === 'undefined' || $(this).val() === '') {
        $(this).val('0.00');
    }
});



//method to initialize datePicker
function InitilizeAutoSearch(id, programId) {
    var data = $("#" + id).data();
    $('#' + id).autocomplete({
        source: function (request, response) {
            $.ajax({
                url: baseUrl + data.url,
                type: "POST",
                dataType: "json",
                data: { programId: programId, name: request.term },
                success: function (data) {
                    response(
                        $.map(data.data, function (item) {
                            return { label: item.Name, value: item.Name, id: item.Id };
                        }))
                }
            })
        },
        minLength: 2,
        select: function (event, ui) {
            $("#" + id).data('parentid', ui.item.id);
        }
    });

}



/***************************************** Top Menu Selected Starts *****************************************/
function leftMenuSelected() {

}

function GetURLParameter() {
    var sPageUrl = location.pathname.toLowerCase(); //window.location.href;
    var indexOfLastSlash = sPageUrl.lastIndexOf("/");
    if (indexOfLastSlash > 0 && sPageUrl.length - 1 != indexOfLastSlash)
        return sPageUrl.substring(indexOfLastSlash + 1);
    else
        return 0;
}
// InitBootstrapSwitch
function InitBootstrapSwitch() {
    $('.bootstrapSwitch-checkbox').on('click', function (event, state) {
        var url = $(this).data('url');
        var id = $(this).data('id');
        GetData({ url: `${url}?id=${id}`, successHandler: statusSuccess, data: true });
    });
}

//change User status success
statusSuccess = function (result) {

    if (result) {
        SwalSuccess("Status changed successfully!");
        setInterval('location.reload()', 3000);
    }
    else {
        SwalError("Something went wrong!");
    }
};

function disabledFormInputs(formId) {

    $("#" + formId).find("textarea,input:not(.no-disable),select,a,label input").each(function () {
        var id = $(this).attr("id");
        $("#" + id + "").attr("disabled", "disabled");
        $("#" + id + "").addClass("disableInputs");
        //disabling the select2
        $("select#" + id).select2({
            disabled: true
        });
    });

    //disable dropzone
    $(`#${formId}`).find(".soundpays-fileUpload").each((i, e) => Dropzone.forElement(`#${e.id}`)?.disable());

    $("#" + formId).find("button:not(.close-button)").each(function () {
        $(this).attr("disabled", "disabled");
    });
    //disabled clear button for readMode
    $("#clearBtn").attr("disabled", "disabled");
    //hide class control on view
    $("#" + formId).find(".hideOnView").each(function () {
        $(this).attr("class", "hidden btn btn-primary");
    });
};

//checked the check-box on click
function onChangeCheckBoxSelection(checkBoxElement) {
    var isChecked = checkBoxElement.checked;
    if (isChecked) {
        $(checkBoxElement).attr("value", "true");
    }
    else {
        $(checkBoxElement).attr("value", "false");
    }
}
// progress bar function for js 
function progressBar(element, isNumEven) {
    // var element = $("#progressBar")[0];
    if (element != null && element != undefined && element != "") {
        var currentValue = $(element).attr("aria-currentValue");
        var maxValue = $(element).attr("aria-maxValue");
        var intValue = 0;
        if (isNumEven) {
            intValue = parseInt(currentValue) + 25;
        } else {
            intValue = parseInt(currentValue) + 20;
        }

        console.log(intValue);
        if (intValue <= maxValue) {
            var progressPer = intValue + "%";
            if (intValue < 100) {
                $(element).css("width", progressPer);
            }
            else {
                $(element).css("width", "100%");
            }
            $(element).attr("aria-currentValue", intValue);
            $(element).text(progressPer);
        }
    }

};

function AllowEnterInTextArea() {
    $("textarea").keyup(function (e) {
        var code = e.keyCode ? e.keyCode : e.which;
        if (code == 13) {  // Enter keycode
            this.value = this.value + " \n";
        }
    });
}


//#region Tiny MCE Editor
function initTinyMCE(id) {
    tinymce.init({
        selector: id,
        plugins: [
            "advlist autolink lists link image charmap print preview anchor",
            "searchreplace visualblocks code fullscreen",
            "insertdatetime media table paste"
        ],
        toolbar: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image"//,

    });

}

//remove all tinymce instance
function RemoveExistingTinymceInstances() {
    for (i = 0; i < tinyMCE.editors.length; i++) {
        tinyMCE.remove();
    }
};

//remove tinymce instance by id
function removeTinymceInstanceById(id) {
    tinyMCE.remove(id);
};


//remove tinymce instance by id
function removeDropzoneInstanceById(id) {

};

// close Comman Modal With TinyMce
function closeCommanModalWithTinyMce(id) {
    tinymce.remove(id);
    $("#common-modal-pop-up").modal('hide');
}

function closeCommanModalWithDropzone(fileUploadId) {
    //remove all the instance of the selected fileUploader and re-initialize it
    $.each(Dropzone.instances, function (index, value) {
        if (value.element.id == fileUploadId) {
            //destroy 
            value.destroy();
            //reinitialize 
            // InitilizefileUploaderById(value.element.id)
        }
    });
    $("#common-modal-pop-up").modal('hide');
}


//bootstrap toggle buttons
function InitilizeToggleButtons() {
    $('.ContestToggleButton').each(function () {

        var data = $(this).data();
        if (data.buttononname != undefined && data.buttononname != ""
            && data.buttononname != null &&
            data.buttonoffname != undefined && data.buttonoffname != ""
            && data.buttonoffname != null) {
            $(this).bootstrapToggle({
                off: data.buttonoffname,
                on: data.buttononname
            });
        }
        else {
            $(this).bootstrapToggle({
                off: 'Off',
                on: 'On'
            });
        }
    });

    $('.ContestToggleButton').change(function () {
        $(this).prop('checked');
    });
}
//#endregion

//initialize tinymce editor
function InitilizeTinyMceEditor() {
    $('.tinymceEditor').each(function () {
        var id = $(this).attr("id");
        id = "#" + id;
        tinymce.init({
            selector: id,
            ///images_upload_url: baseUrl + '/Attachment/SaveEmailAttachments',
            ///images_upload_base_path: '',
            ///automatic_uploads: true,
            plugins: [
                "advlist autolink lists link image charmap print preview anchor",
                "searchreplace visualblocks code fullscreen",
                "insertdatetime media table paste"
            ],
            toolbar: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image"
        });
    });
};

//validate date range
//on change the date field
function isDateValid(startPicker, endPicker) {
    if (element != undefined && element != null && element != "") {
        var enteredDate = $(element).val();
        if (enteredDate != null && enteredDate != undefined && enteredDate != "") {
            var dateArray = enteredDate.split('/');
            var currentDate = new Date();
            if (dateArray[2] < currentDate.getFullYear()) {
                failAlert("Please enter valid date!");
                return false;
            }
            else if (dateArray[2] == currentDate.getFullYear()
                && dateArray[0] < (currentDate.getMonth() + 1)) {
                failAlert("Please enter valid date!");
                return false;
            }
            else if (dateArray[2] == currentDate.getFullYear()
                && dateArray[0] == (currentDate.getMonth() + 1)
                && dateArray[1] < currentDate.getDate()) {
                failAlert("Please enter valid date!");
                return false;
            }
            else {
                return true;
            }
        }
        else {
            failAlert("Please enter valid date!");
        }
        return true;
    }
};

function formattedDate(date) { return moment(date).format('D MMMM YYYY') };

function isTimeInRange(start, end) {
    let startDate = $(`#${start}`).val();
    let endDate = $(`#${end}`).val();
    if (moment(formattedDate(startDate)).isAfter(formattedDate(endDate))) {
        failAlert("End date should be greater than Start date!");
    }
}

//#region Enum Start 
function getKeyByValue(object, value) {
    for (var prop in object) {
        if (object.hasOwnProperty(prop)) {
            if (object[prop] === value)
                return prop;
        }
    }
}
//#endregion

//#region phone number validator
function ValidatePhone(inputId, targetId, errorId) {
    const errorMap = ["Invalid number", "Invalid country code", "Too short number", "Too long number", "Please enter only number"];
    const input = document.getElementById(inputId),
        errorMsg = document.getElementById(errorId);
    const iti = window.intlTelInput(input, {
        autoHideDialCode: true,
        separateDialCode: true,
        formatOnDisplay: false,
        utilsScript: "../../lib/phone-validate/utils.js",
    });

    var phnNo = $(`#${targetId}`).val().replace(/\s+/g, '');
    iti.setNumber(phnNo);
    const countryData = iti.getSelectedCountryData();
    iti.setCountry(countryData.iso2);
    var phn_no = $(`#${targetId}`).val().split(' ').pop();
    $(`#${inputId}`).val(phn_no);

    const reset = () => {
        input.classList.remove("error");
        errorMsg.innerHTML = "";
        errorMsg.classList.add("d-none");

    };
    const isNumber = (str) => /^\d+$/.test(str);

    input.addEventListener("blur", (event) => {
        reset();
        if (input.value.trim()) {
            if (iti.isValidNumber() && isNumber(input.value)) {
                const countryData = iti.getSelectedCountryData();
                const countryCode = countryData.dialCode;
                const inputNumber = `+${countryCode} ${input.value}`;
                $(`#${targetId}`).val(inputNumber);
            }
            else {
                input.classList.add("error");
                const errorCode = iti.getValidationError();
                errorMsg.innerHTML = errorCode == -99 ? errorMap["4"] : errorMap[errorCode];
                errorMsg.classList.remove("d-none");
                errorMsg.classList.add("d-block");
            }
        }
        else if (input.value == "") {
            $(`#${targetId}`).val("");
        }
    });

    input.addEventListener("change", reset);
    input.addEventListener("keyup", reset);
};

//keypress
function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    return !(charCode > 31 && (charCode < 48 || charCode > 57));
}

function FormValidationCheck(e) {
    var button = $(e);
    var form = $(e).parents('form:first');
    //return if form not valid
    if (!form.validate().form()) return;
    //checking phone number validations
    var phoneError = [];
    $.each(window?.intlTelInputGlobals?.instances, (idx, elem) => {
        if (window.intlTelInputGlobals.instances[idx].a.value != '') {
            phoneError.push(window.intlTelInputGlobals.instances[idx].isValidNumber());
        }
    });
    //return if phone input not valid
    if (phoneError.length > 0 && phoneError.some(x => x == false)) return;

    button.prev().trigger('click');
}

//#endregion

//#region time Picker
//method to initialize timePicker
function InitializeTimePicker() {
    $('.soundpays-timePicker').timepicker({
        'step': '15',
        'timeFormat': 'h:i A',
        'minTime': '07:00 A',
        'maxTime': '24'
    });
}
//#endregion

//#region convert Standard To Military Time
function convertStandardToMilitaryTime(control) {
    $getFieldId = control.id;
    var time = $("#" + $getFieldId).val();
    if (time != "" && time != undefined) {
        var hours = Number(time.match(/^(\d+)/)[1]);
        var minutes = Number(time.match(/:(\d+)/)[1]);
        var AMPM = time.match(/\s(.*)$/)[1];
        if (AMPM == "PM" && hours < 12) hours = hours + 12;
        if (AMPM == "AM" && hours == 12) hours = hours - 12;
        var sHours = hours.toString();
        var sMinutes = minutes.toString();
        if (hours < 10) sHours = "0" + sHours;
        if (minutes < 10) sMinutes = "0" + sMinutes;
        convertedStandardTime = sHours + ":" + sMinutes + ":" + "00";
        $("#" + $getFieldId).siblings(".sound-pays-time-picker").val(convertedStandardTime);
    }
}
//#endregion
//#region sidebar
function sideBarAction() {
    const sideBar = document.getElementById('sidebar');
    if (((window.innerWidth > 0) ? window.innerWidth : screen.width) >= 768) {
        sideBar && sideBar?.classList?.add("show");
        return;
    }
    if (sideBar) {
        var sidebarCollapse = bootstrap.Collapse.getOrCreateInstance(sideBar, { toggle: false });
        sidebarCollapse.hide();
        const sideOverlay = document.getElementById('sidebar-overlay');
        sideBar?.addEventListener('hidden.bs.collapse', event => sideOverlay?.classList?.remove("sidebar-overlay"));
        sideBar?.addEventListener('show.bs.collapse', event => sideOverlay?.classList?.add("sidebar-overlay"));
    }

}
window.addEventListener("resize", sideBarAction);
document.addEventListener("DOMContentLoaded", sideBarAction);
//#endregion

//action handler
// Delete, Active, Deactivate
//the element should have data-id and data-name and should pass the element 
function actionConfirmBox(obj) {
    let { title = "Are you sure?", text = "delete", icon = "warning", url, elem, cb } = obj;
    var data = $(elem).data();
    text = `Do you want to ${text} ${data.name}?`,
        swal({
            title: title,
            text: text,
            icon: icon,
            buttons: true,
            dangerMode: true
        }).then((willDelete) => {
            if (willDelete) {
                var la = Ladda.create(elem);
                la?.start();
                PostData({ url: url, successHandler: cb, data: { id: parseInt(data.id) } });
            }
        });
}




//Atchamnets
//recordId = id of the item with the attachment is assosiated
//attachmentId = id of the attachment
//actionUrl = delete url
//listId = tr id
function DeleteAttachment(event) {
    let elem = event.currentTarget;
    let { recordId, attachmentId, actionUrl, text = "Please confirm you wish to delete." } = elem.dataset;

    swal({
        title: "Are you sure?",
        text: text,
        icon: "warning",
        buttons: true,
        dangerMode: true
    })
        .then((willDelete) => {
            if (willDelete) {
                var la = Ladda.create(elem);
                la?.start();
                let obj = {
                    url: actionUrl,
                    successHandler: (result, data, elem) => {
                        let { listId } = elem.dataset;
                        if (result.success) {
                            $(`#${listId}`).remove();
                            SwalSuccess("File Deleted Successfully!");
                        }
                        else {
                            SwalError("Something went wrong!");
                        }
                    },
                    data: { recordId: recordId, id: attachmentId },
                    elem
                };
                PostData(obj);
            }
        });
}

