$(function () {
    $('body').on('submit', 'form', function () {
        if ($(this).valid()) {
            // Find the panel to hold this partial; in case of modals or partials it could be different from site-content and we may need to add selectors
            var panel = $(this).closest('.site-content');

            // TODO show error when there's no panel

            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                success: function (result) {
                    panel.html(result);
                },
                error: function () {
                    // TODO show error
                }
            });
        }

        return false;
    });
});