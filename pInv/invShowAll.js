

//Toggle control. Keeps track of whether or not the window shade is pulled down
var FilterIsShowing = true;

//Startup function
$(document).ready(function () {

    $('div[id*="FilterPane"]').slideUp(0); //Start with the Shade up...

    $('div[id*="FilterHandle"]').click(function () {
        ToggleFilterShade();
    });
});

//Worker. Implements the Shade toggle.
function ToggleFilterShade() {
    if (FilterIsShowing == true) {
        $('div[id*="FilterPane"]').slideDown(400);
    }
    else {
        $('div[id*="FilterPane"]').slideUp(400);
    }
    FilterIsShowing = !FilterIsShowing;
}
