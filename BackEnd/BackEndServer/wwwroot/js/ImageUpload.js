/* Script for pages that have a Image Upload UI element.
    Modified from: https://bootsnipp.com/snippets/eNbOa
*/

$(document).ready(function () {

    // On Load, hide the clear button. Cannot set the element to hidden by default as it can no longer be accessed once hidden.
    $('#clear_image_preview_button').hide();

    // test
    $('#browse_button').val('test');

    // On Click of the Clear button:
    $('#clear_image_preview_button').click(function () {
        // Clear the file input.
        $('#image_input').val('');
        // Clear the file name label.
        $('#file_name').val('');
        // Hide the image preview, no need clear the source.
        $('#image_preview').hide();
        // Hide the clear button.
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

        if (input.length) {
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
