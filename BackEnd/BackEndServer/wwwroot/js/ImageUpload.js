/* Script for pages that have a Image Upload UI element.
    Modified from: https://bootsnipp.com/snippets/eNbOa
*/

// Changes throughout, including renaming the HTML image element to image_preview

$(document).ready(function () {

    $('#clear_image_preview_button').hide();

    $('#clear_image_preview_button').click(function () {
        // Not sure how to clear the file upload input...
        //var input = $('#image_input');
        //input.replaceWith(input.val('').clone(true));
        $('#image_preview').hide();
        $('#clear_image_preview_button').hide();
    });

    $(document).on('change', '.btn-file :file', function () {
        var input = $(this),
            label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
        input.trigger('fileselect', [label]);
    });

    $('.btn-file :file').on('fileselect', function (event, label) {

        var input = $(this).parents('.input-group').find(':text'),
            log = label;

        // Added a check for label != "" because without this, pressing cancel removes the filename from the page.
        if (input.length && label != "") {
            input.val(log);
        } else {
            if (log) alert(log);
        }

    });
    function readURL(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('#image_preview').attr('src', e.target.result);
                // Added a border to the image preview.
                $('#image_preview').css('border', '2px solid black');
                // Show the newly added Clear Image Preview button.
                $('#clear_image_preview_button').show();
                // Show the image preview as it may be hidden.
                $('#image_preview').show();
            }

            reader.readAsDataURL(input.files[0]);
        }
    }

    $("#image_input").change(function () {
        readURL(this);
    });
});
