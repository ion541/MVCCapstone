// Called when removing a bookmark
function removeRow(id) {
    // display a message with a fade in / fade out animation
    $('<div>Removing ' + $("#" + id + " > td:nth-child(2)").text() + ' from your bookmarks' + '</div>')
        .insertAfter('#message').fadeIn().delay(3000).fadeOut(1000);

    // remove the row from the table
    $("#" + id).animate({ 'line-height': 0 }, 100).hide(1000);
}

// scrolls to the postSection
function moveToPost() {
    $('html, body').animate({
        scrollTop: $("#postSection").offset().top
    }, 700);
}

// sets the storePost value attribute to the current 'content' text
// done since the ajax post will reload the page which will lose the current text in the textarea
function storeEditPost() {
    $("#storePost").attr("value", $("#content").val());
}

// sets the textarea text to the current storePost value attribute
function setEditPost() {
    $("#content").val($("#storePost").attr("value"));
}

// changes text messages
function switchLockActionText() {
    var currentText = $("#LockButton").text();
    var newText;
    if (currentText == "Lock Thread") {
        newText = "Unlock Thread";
    } else {
        newText = "Lock Thread";
    }
    $("#LockButton").text(newText);

}

// scrolls to the specified id
function scrollTo(id) {
    $('html, body').animate({
        scrollTop: $("#" + id).offset().top
    }, 700);
}

// moves to the message
function moveToMessage() {
    $('html, body').animate({
        scrollTop: $("#message").offset().top
    }, 700);
}

// shows the the section where the the options for editting in the admin - add a book
function showSeriesOption() {
    $("#isSeriesOption").show(500);
}

// hides the the section where the the options for editting in the admin - add a book
function hideSeriesOption() {
    $("#isSeriesOption").hide(500);
}

// shows the the section where the the options for editting in the admin - add a book - new series
function hideSeriesTitle() {
    $("#seriesTitle").hide(500);

    $("#seriesId").show(500);
    $("#seriesSearch").show(500);
}

// hide the the section where the the options for editting in the admin - add a book - new series
function showSeriesTitle() {
    $("#seriesTitle").show(500);

    $("#seriesId").hide(500);
    $("#seriesSearch").hide(500);
}



// solution provided at
// http://web.archive.org/web/20110102112946/http://www.scottklarr.com/topic/425/how-to-insert-text-into-a-textarea-where-the-cursor-is/
// Inserts text at where the current mouse cursor is placed
function insertAtCaret(areaId, tag) {
    
    var startTag = "[" + tag + "]";
    var endTag = "[/" + tag + "]";
    var txtarea = document.getElementById(areaId);
    var scrollPos = txtarea.scrollTop;
    var strPos = 0;
    var br = ((txtarea.selectionStart || txtarea.selectionStart == '0') ?
        "ff" : (document.selection ? "ie" : false));
    if (br == "ie") {
        txtarea.focus();
        var range = document.selection.createRange();
        range.moveStart('character', -txtarea.value.length);
        strPos = range.startTag.length;
    }
    else if (br == "ff") strPos = txtarea.selectionStart;

    var front = (txtarea.value).substring(0, strPos);
    var back = (txtarea.value).substring(strPos, txtarea.value.length);

    txtarea.value = front + startTag + endTag + back;
    strPos = strPos + startTag.length;

    if (br == "ie") {
        txtarea.focus();
        var range = document.selection.createRange();
        range.moveStart('character', -txtarea.value.length);
        range.moveStart('character', strPos);
        range.moveEnd('character', 0);
        range.select();
    }
    else if (br == "ff") {
        txtarea.selectionStart = strPos;
        txtarea.selectionEnd = strPos;
        txtarea.focus();
    }
    txtarea.scrollTop = scrollPos;

    // update the amount of charactesr available
    var maximum = 7500;
    if ($("#" + areaId).val().length > 7500) {
        $("#textCount").text("Over by: " + ($("#" + areaId).val().length - maximum)).attr('style', 'color:red');
        $("#previewbutton").hide();
    } else {
        $("#textCount").text("Characters left: " + (maximum - $("#" + areaId).val().length)).removeAttr('style');
        $("#previewbutton").show();
    }
}

$(function () {


    // updates the number of characters left
    $("#reviewContent").keyup(function () {
        var maximum = 7500;

        if ($(this).val().length > 7500) {
            $("#textCount").text("Over by: " + ($(this).val().length - maximum)).attr('style', 'color:red');
            $("#previewbutton").hide();
        } else {
            $("#textCount").text("Characters left: " + (maximum - $(this).val().length)).removeAttr('style');
            $("#previewbutton").show();
        }
     
    });

    $(".show-more a").on("click", function () {
        var $link = $(this);
        var $content = $link.parent().prev("div.text-content");
        var linkText = $link.text().toUpperCase();

        $content.toggleClass("short-text, full-text");

        $link.text(getShowLinkText(linkText));

        return false;
    });

    function getShowLinkText(currentText) {

        return (currentText === "SHOW MORE [ + ]") ? "Show Less [ - ]" : "Show More [ + ]"
    }
});
