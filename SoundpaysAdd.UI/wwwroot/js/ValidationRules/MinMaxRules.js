
jQuery.validator.addMethod("minmax", function (value, element) {
    if (!value && !element.dataset) return true;

    let { valComparewith, valCompare } = element.dataset;
    if (!valComparewith && !valCompare) return true;

    let elemCompareWith = $(`#${valComparewith}`);
    if (!elemCompareWith) return true;

    let compareValue = elemCompareWith.val();

    if (valCompare == "less" && compareValue > value) return true; 
    else if (valCompare == "greater" & compareValue < value)   return true;

    return false;
});


jQuery.validator.unobtrusive.adapters.add("minmax", ["valComparewith", "valCompare"],
    function (options) {
        options.rules['minmax'] = {
            valComparewith: options.params.valComparewith,
            valCompare: options.params.valCompare,
        };
        options.messages['minmax'] = options.message;
    });

