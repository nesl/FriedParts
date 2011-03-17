
$(document).ready(function () {
    $("div[class='fpMsgbox']").delay(6000).fadeOut(1000);
    //Fix empty box not firing server-side event problem
    $("input[id$='SearchBox']").keydown(
        function (event) {
            if (event.keyCode == '13') {
                if ($(this).val() == '') { $(this).val(' '); };
            };
        }
    )
});

//Prevent session expiration when the user still has the browser window open
function keepMeAlive(imgName) {
    myImg = document.getElementById(imgName);
    if (myImg) myImg.src = myImg.src.replace(/\?.*$/, '?' + Math.random());
}
window.setInterval("keepMeAlive('keepAliveIMG')", 500000);