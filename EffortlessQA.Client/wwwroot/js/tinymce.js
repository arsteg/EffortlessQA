window.initializeTinyMce = function (editorId, initialContent, dotNetRef, entityId, fieldName) {
    tinymce.init({
        selector: `#${editorId}`,
        plugins: 'image link lists table paste wordcount advlist autolink charmap code emoticons fullscreen hr media pagebreak preview searchreplace visualblocks visualchars template',
        toolbar: 'undo redo | formatselect | bold italic underline strikethrough | forecolor backcolor | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image table | fontsizeselect | removeformat | code preview fullscreen',
        menubar: 'file edit view insert format tools table help',
        height: 500,
        content_style: 'body { font-family: Calibri, sans-serif; font-size: 11pt; margin: 1cm; } p { margin: 0; line-height: 1.15; }',
        paste_as_text: false,
        paste_data_images: true,
        fontsize_formats: '8pt 9pt 10pt 11pt 12pt 14pt 18pt 24pt 36pt',
        style_formats: [
            { title: 'Heading 1', block: 'h1' },
            { title: 'Heading 2', block: 'h2' },
            { title: 'Heading 3', block: 'h3' },
            { title: 'Normal', block: 'p' }
        ],
        templates: [
            { title: 'Default Template', description: 'Standard format', content: '<p><strong>Title:</strong> [Title]</p><p><strong>Description:</strong> [Description]</p>' }
        ],
        images_upload_url: `/api/v1/common/images/upload?entityId=${encodeURIComponent(entityId)}&fieldName=${encodeURIComponent(fieldName)}`,
        images_upload_handler: function (blobInfo, success, failure) {
            var xhr, formData;
            xhr = new XMLHttpRequest();
            xhr.withCredentials = true;
            xhr.open('POST', `/api/v1/common/images/upload?entityId=${encodeURIComponent(entityId)}&fieldName=${encodeURIComponent(fieldName)}`);

            xhr.onload = function () {
                if (xhr.status < 200 || xhr.status >= 300) {
                    failure('HTTP Error: ' + xhr.status);
                    return;
                }
                var json = JSON.parse(xhr.responseText);
                if (!json || typeof json.location != 'string') {
                    failure('Invalid JSON: ' + xhr.responseText);
                    return;
                }
                success(json.location);
            };

            xhr.onerror = function () {
                failure('Image upload failed due to a network error.');
            };

            formData = new FormData();
            formData.append('file', blobInfo.blob(), blobInfo.filename());
            xhr.send(formData);
        },
        setup: function (editor) {
            editor.on('Change', function () {
                dotNetRef.invokeMethodAsync('UpdateValue', editor.getContent());
            });
            editor.on('init', function () {
                editor.setContent(initialContent || '');
            });
        }
    });
};

window.setTinyMceImageUrl = function (editorId, url) {
    tinymce.get(editorId).insertContent(`<img src="${url}" />`);
};

window.showTinyMceError = function (editorId, message) {
    tinymce.get(editorId).notificationManager.open({
        text: message,
        type: 'error'
    });
};

window.destroyTinyMce = function (editorId) {
    const editor = tinymce.get(editorId);
    if (editor) {
        editor.remove();
    }
};