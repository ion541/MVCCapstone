// Called when removing a bookmark
function removeRow(id) {
    // display a message with a fade in / fade out animation
    $("#message").fadeIn().text("Removing " + $("#" + id + " > td:nth-child(2)").text() + " from your bookmarks").fadeOut(2000);
    // remove the row from the table
    $("#" + id).animate({ 'line-height': 0 }, 100).hide(1000);
}