function setCookie(c_name, value, exdays) {
    var exdate = new Date();
    exdate.setDate(exdate.getDate() + exdays);
    var c_value = encodeURIComponent(value) + (exdays == null ? "" : "; expires=" + exdate.toUTCString());
    document.cookie = c_name + "=" + c_value;
}

function getCookie(c_name) {
    var ARRcookies = document.cookie.split(";");
    for (var i = 0; i < ARRcookies.length; i++) {
        var x = ARRcookies[i].split("=");
        var key = x[0].trim();
        var value = x[1];
        if (key === c_name) {
            return decodeURIComponent(value);
        }
    }
    return "";
}

document.getElementById("loginEmail").focus();

$("#loginEmail, #loginPassword").on('keydown', function (e) {
    if (e.key === 'Enter') {
        btn_Login();
    }
});

function btn_Login() {
    var email = $("#loginEmail").val().trim();
    var password = $("#loginPassword").val().trim();

    $.ajax({
        type: "POST",
        url: "/Loginnow",
        data: { email: email, password: password },
        success: function (data) {
            if (data.resultID === 1) {
                Swal.fire({
                    position: 'center',
                    icon: 'success',
                    text: data.resultMessage,
                    showConfirmButton: false
                });

                sessionStorage.setItem("userrole", data.userid?.authority || "");

                if ($("#rememberMe").prop("checked")) {
                    setCookie("rememberMeEmail", email, 30);
                    setCookie("rememberMePassword", password, 30);
                } else {
                    setCookie("rememberMeEmail", "", -1);
                    setCookie("rememberMePassword", "", -1);
                }

                setTimeout(() => {
                    window.location.href = sessionStorage.getItem("goToUrl") || data.redirectURL;
                }, 1500);
            } else {
                Swal.fire({
                    position: 'center',
                    icon: 'error',
                    text: data.resultMessage,
                    showConfirmButton: true
                });
            }
        },
        error: function () {
            Swal.fire({
                position: 'center',
                icon: 'error',
                text: "Sunucu hatası, lütfen tekrar deneyin.",
                showConfirmButton: true
            });
        }
    });
}

function btn_forgotPassword() {
    sessionStorage.removeItem('emailForNewPassword');
    sessionStorage.setItem("emailForNewPassword", $("#loginEmail").val());
    window.location.href = "/SifremiUnuttum";
}
