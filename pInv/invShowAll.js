

//Toggle control. Keeps track of whether or not the window shade is pulled down
var FilterIsShowing = true;

//Startup function
$(document).ready(function () {

    $("#FilterPane").slideUp(0); //Start with the Shade up...

    $("#FilterHandle").click(function () {
        ToggleFilterShade();
    });
});

//Worker. Implements the Shade toggle.
function ToggleFilterShade() {
    if (FilterIsShowing == true) {
        $("#FilterPane").slideDown(400);
    }
    else {
        $("#FilterPane").slideUp(400);
    }
    FilterIsShowing = !FilterIsShowing;
}
