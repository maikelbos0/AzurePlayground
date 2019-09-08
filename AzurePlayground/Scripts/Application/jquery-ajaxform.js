﻿$(function () {
    $('body').on('submit', 'form', function () {
        // Find the panel to hold this partial; in case of modals or partials it could be different from site-content and we may need to add selectors
        var panel = $(this).closest('.site-content');
        var submitButtons = $(this).find('button[type="submit"]');

        if (panel.length === 0) {
            toastr.error("An error occurred locating content panel; please contact support to resolve this issue.");
        }
        else if ($(this).valid()) {
            submitButtons.prop('disabled', true);

            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                success: function (result) {
                    panel.html(result);
                },
                error: function () {
                    toastr.error("An error occurred processing data; please try again.");
                    submitButtons.prop('disabled', false);
                }
            });
        }

        return false;
    });
});