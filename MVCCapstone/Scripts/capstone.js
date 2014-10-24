// Called when removing a bookmark
function removeRow(id) {
    // display a message with a fade in / fade out animation
    $("#message").fadeIn().text("Removing " + $("#" + id + " > td:nth-child(2)").text() + " from your bookmarks").fadeOut(2000);
    // remove the row from the table
    $("#" + id).animate({ 'line-height': 0 }, 100).hide(1000);
}

function moveToPost() {
    $('html, body').animate({
        scrollTop: $("#postSection").offset().top
    }, 700);
}

function moveToMessage() {
    $('html, body').animate({
        scrollTop: $("#message").offset().top
    }, 700);
}

function scrollToSearch() {
    $('html, body').animate({
        scrollTop: $("#seriesSearch").offset().top
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

function setSeriesId() {
    alert('good job');
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
