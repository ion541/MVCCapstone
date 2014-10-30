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


function scrollTo(id) {
    $('html, body').animate({
        scrollTop: $("#" + id).offset().top
    }, 700);
}

function moveToMessage() {
    $('html, body').animate({
        scrollTop: $("#message").offset().top
    }, 700);
}


function showSeriesOption() {
    $("#isSeriesOption").show(500);
}

function hideSeriesOption() {
    $("#isSeriesOption").hide(500);
}

function hideSeriesTitle() {
    $("#seriesTitle").hide(500);

    $("#seriesId").show(500);
    $("#seriesSearch").show(500);
}

function showSeriesTitle() {
    $("#seriesTitle").show(500);

    $("#seriesId").hide(500);
    $("#seriesSearch").hide(500);
}


$(function () {

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
