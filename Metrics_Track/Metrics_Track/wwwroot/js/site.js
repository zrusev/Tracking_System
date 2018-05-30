﻿$(function () {
    //Select process node
    $('.btn-default').on('click', function () {
        var selection = $(this).find('input').attr('id');
        var selectionName = $(this).text().trim();
        var division = $(this).parent().attr("id");

        switch (division) {
            case "Lob":
                $('input[name=LobSelection]').val(selection);
                $('input[name=LobName]').val(selectionName);
                break;
            case "Activity":
                $('input[name=ActivitySelection]').val(selection);
                $('input[name=ActivityName]').val(selectionName);
                break;
            case "Status":
                $('input[name=StatusSelection]').val(selection);
                $('input[name=StatusName]').val(selectionName);
                break;
        }
    });

    //Select country node
    $(".list-group-item").on("click", function () {
        var division = $(this).data("parent");
        var isAriaExpanded = $(this).attr("aria-expanded") === 'true' ? true : false;
        expandCollapseAria($(this), isAriaExpanded, division);
    });

    //Update status
    $("#useractivity").on('change', function (e) {
        e.preventDefault();
        e.stopImmediatePropagation();
       
        var token = $('input[name="__RequestVerificationToken"]', $("#submittranform")).val();
        var jsonObject = {
            Type : $(this).val(),
            Comment: $("#comment1").val()
        };

        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: '/dashboard/UpdateStatus',
            data: {
                "__RequestVerificationToken": token,
                model: jsonObject
            },
            success: function (response) {
                document.getElementById("currentstatus").innerHTML = response.status;
            },
            error: function (response) {
                alert('Internal error. Please contact support.');
            }
        });
        return false;
    });

    //Submit form
    $("#submittranform").on('submit', function (e) {
        e.preventDefault();
        e.stopImmediatePropagation();

        $(this).find("[data-valmsg-replace]")
            .removeClass("field-validation-error")
            .addClass("field-validation-valid")
            .empty();
        $(this).data("validator").settings.ignore = "";

        var token = $('input[name="__RequestVerificationToken"]', $(this)).val();
        var jsonObject = {
            IdCountry : $('input[name=CountrySelection]').val(),
            IdProcess : $('input[name=ProcessSelection]').val(),
            IdActivity : $('input[name=ActivitySelection]').val(),
            IdLob : $('input[name=LobSelection]').val(),
            IdDivision : $('input[name=ProcessSelection]').val(),
            IdTowerCategory : $('input[name=ProcessSelection]').val(),
            IdTower : $('input[name=ProcessSelection]').val(),
            ReceivedDate: moment($("#Transaction_ReceivedDate").val()).isValid() ? moment($("#Transaction_ReceivedDate").val()).format("YYYY-MM-DD HH:mm:ss") : null,
            StartDate: null,
            IdStatus : $('input[name=StatusSelection]').val(),
            Comment : $("#comment1").val(),
            IdNumber : $("#policynumber1").val(),
            Premium: $("#amount1").val(),
            IsPriority: $("#priorityCheck").prop('checked'),
            InceptionDate: moment($("#Transaction_InceptionDate").val()).isValid() ? moment($("#Transaction_InceptionDate").val()).format("YYYY-MM-DD HH:mm:ss") : null,
            DateReceivedInAig: moment($("#Transaction_DateReceivedInAig").val()).isValid() ? moment($("#Transaction_DateReceivedInAig").val()).format("YYYY-MM-DD HH:mm:ss") : null
        };

        if ($(this).valid()) {
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: '/dashboard/SubmitTransaction',
                data: {
                    "__RequestVerificationToken" : token,
                    model: jsonObject
                },
                success: function (data) {
                    if (data.success) {
                        //Add to pending transactions
                        var process = $('input[name=ProcessName]').val();
                        var processIdentifier = "#process-" + $('input[name=ProcessSelection]').val();
                        var lob = $('input[name=LobName]').val();
                        var premiumAmount = data.prem == null ? "" : data.prem;
                        var receivedDate = moment($("#Transaction_ReceivedDate").val()).format("YYYY-MM-DD HH:mm:ss");
                        var id = data.newId;
                        var status = $('input[name=StatusName]').val();

                        var previousTable = $("#previousTransactionTable");
                        $("#previousTransactionTable > tbody").html("");
                        var newRow = "<tr><td>" + process + "</td><td>" + lob + "</td><td>" + premiumAmount + "</td><td>" + receivedDate + "</td><td><button type=\"button\" class=\"btn btn-info btn-xs\">" + id + "</button></td><td>" + status + "</td></tr>";
                        previousTable.append(newRow);

                        if (data.pending) {
                            var pendingTable = $("#pendingTransactionTable");
                            pendingTable.append(newRow);
                        }

                        //Last transaction
                        $("#previousProcessId").val($('input[name=ProcessSelection]').val());
                        $("#previousLobId").val($('input[name=LobSelection]').val());
                        $("#previousPremium").val(premiumAmount);
                        $("#previousReceivedDate").val(receivedDate); 
                        $("#previousDocId").val(id);
                        $("#previousStatusId").val($('input[name=StatusSelection]').val());

                        //Clear form
                        var sectionBoxCheck = $("#sectionCheck").prop('checked');
                        var receivedBoxCheck = $("#receivedCheck").prop('checked');
                        var priorityBoxCheck = $("#priorityCheck").prop('checked');
                        resetForm($("#submittranform"), processIdentifier, sectionBoxCheck, receivedBoxCheck, priorityBoxCheck);

                        //Set new startDate
                        $("#Transaction_StartDate").val(moment(new Date(data.startDate)).format("YYYY-MM-DD HH:mm:ss"));
                    }
                    else {
                        $.each(data.errors, function (ind, err) {
                            alert(err);
                        });
                    }
                },
                error: function (response) {
                    alert('Internal error. Please contact support.');
                }
            });
        } else {
            var validator = $(this).validate();
            $.each(validator.errorList, function (key, value) {
                $errorSpan = $("span[data-valmsg-for='" + value.element.id.replace('_', '.') + "']");
                $errorSpan.html("<span style='color:red'>" + value.message + "</span>");
                //$errorSpan.show();
            });
        }
    });
});

//Set values
function expandCollapseAria(currentElement, isAriaExpanded, division) {
    if (isAriaExpanded) {
        if (division === "#country") {
            $('input[name=CountrySelection]').val("");

            var processIdentifier = "#process-" + $('input[name=ProcessSelection]').val();
            var btns = currentElement.parent().next().find(".btn-group-vertical label");
            btns.each(function (index, item) {
                item.classList.remove('active');
            });
            $(processIdentifier).attr('aria-expanded', false);
            $(processIdentifier).prev().children().attr('aria-expanded', false);
            $(processIdentifier).removeClass('in');
        }
        $('input[name=ProcessSelection]').val("");
        $('input[name=ProcessName]').val("");
        $('input[name=ActivitySelection]').val("");
        $('input[name=ActivityName]').val("");
        $('input[name=LobSelection]').val("");
        $('input[name=LobName]').val("");
        $('input[name=StatusSelection]').val("");
        $('input[name=StatusName]').val("");
        //console.log("hide");
    } else {
        var btns = currentElement.parent().next().find(".btn-group-vertical label");
        btns.each(function (index, item) {
            item.classList.remove('active');
        });

        var selection = currentElement.data("id");
        var selectionName = currentElement.text().trim();

        if (division === "#country") {
            $('input[name=CountrySelection]').val(selection);
        } else {
            $('input[name=ProcessSelection]').val(selection);
            $('input[name=ProcessName]').val(selectionName);
        }
        //console.log("show");
    }
};

//Get activities
function loadData() {
    $("#tblMining tbody tr").remove();
    $.ajax({
        type: "Get",
        url: '@Url.Action("GetMining")',
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { id: 145 },
        success: function (ajaxResponse) {
            var rows = '';
            $.each(ajaxResponse, function (index, ajaxResponse) {
                rows = "<tr>"
                    + "<td>" + ajaxResponse.idMining + "</td>"
                    + "<td>" + ajaxResponse.state + "</td>"
                    + "</tr>";
                $('#tblMining tbody').append(rows);
            });
        },
        error: function (ex) {
            var r = jQuery.parseJSON(response.responseText);
            alert("Message: " + r.Message);
            alert("StackTrace: " + r.StackTrace);
            alert("ExceptionType: " + r.ExceptionType);
        }
    });
    return false;
}

//Reset form
function resetForm($form, processIdentifier, sectionBoxCheck, receivedBoxCheck, priorityBoxCheck) {
    var sBox = sectionBoxCheck == true ? "#sectionCheck" : '';
    var rBox = receivedBoxCheck == true ? "#receivedCheck" : '';
    var pBox = priorityBoxCheck == true ? "#priorityCheck" : '';
    var rValue = receivedBoxCheck == true ? "#Transaction_ReceivedDate" : '';

    if (sectionBoxCheck == true) {
        //do nothing
    } else {
        $form.find('input:text, input:password, input:file, select, textarea').not(rValue).not("#Transaction_StartDate").val('');
        expandCollapseAria($(processIdentifier), true, '');
        $(processIdentifier).attr('aria-expanded', false);
        $(processIdentifier).prev().children().attr('aria-expanded', false);
        $(processIdentifier).removeClass('in');
    }

    $form.find('input:radio, input:checkbox').filter(sBox | rBox | pBox).prop('checked', true);
}

//Get tokens
function getTokens() {
    var countryId = parseInt($('#country .in').attr("id").split("-")[1]);
    var processId = parseInt($('#process' + '-' + countryId + ' .in').attr("id").split("-")[1]);
}