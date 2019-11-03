function getPasswordStrength(password) {
    if (password === null || password === undefined || password.toString().trim() === '') {
        return 0;
    }

    password = password.toString().trim();

    let score = 0;

    if (hasLowerCaseLetters(password)) { score++; }
    if (hasUpperCaseLetters(password)) { score++; }
    if (hasDigits(password)) { score++; }
    if (hasSpecialCharacters(password)) { score++; }

    if (password.length < 6) {
        return Math.min(score, 1);
    }
    else if (password.length < 8) {
        return Math.min(score, 2);
    }
    else {
        return score;
    }
}

function hasLowerCaseLetters(password) {
    for (var i = 0; i < password.length; i++) {
        let c = password.charAt(i);

        if (c === c.toLowerCase() && c !== c.toUpperCase()) {
            return true;
        }
    }

    return false;
}

function hasUpperCaseLetters(password) {
    for (var i = 0; i < password.length; i++) {
        let c = password.charAt(i);

        if (c !== c.toLowerCase() && c === c.toUpperCase()) {
            return true;
        }
    }

    return false;
}

function hasDigits(password) {
    return /\d/.test(password);
}

function hasSpecialCharacters(password) {
    for (var i = 0; i < password.length; i++) {
        let c = password.charAt(i);

        if (c === c.toLowerCase() && c === c.toUpperCase() && !/\d/.test(c)) {
            return true;
        }
    }

    return false;
}

$(function () {
    $('input.password-strength').each(function () {
        let base = $(this);
        let meter = $('<div>', { class: 'small site-password-meter' }).hide();

        base.after(meter);

        base.on('keyup', function () {            
            switch (getPasswordStrength(base.val())) {
                case 0:
                    meter.hide();
                    break;
                case 1:
                case 2:
                    meter.removeClass('site-password-meter-2 site-password-meter-3');
                    meter.text('Weak');
                    meter.addClass('site-password-meter-1');
                    meter.show();
                    break;
                case 3:
                    meter.removeClass('site-password-meter-1 site-password-meter-3');
                    meter.text('Moderate');
                    meter.addClass('site-password-meter-2');
                    meter.show();
                    break;
                case 4:
                    meter.removeClass('site-password-meter-1 site-password-meter-2');
                    meter.text('Strong');
                    meter.addClass('site-password-meter-3');
                    meter.show();
                    break;
            }
            
        });
    });
});