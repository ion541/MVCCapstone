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

$(function(){
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
