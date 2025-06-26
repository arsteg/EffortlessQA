window.tinyMCEBlazor = {
    initEditor: function (textareaId, dotNetHelper, entityId, fieldName, authToken, baseUrl) {
        tinymce.init({
            selector: `#${textareaId}`,
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
            images_upload_url: `${baseUrl}common/images/upload?entityId=${encodeURIComponent(entityId)}&fieldName=${encodeURIComponent(fieldName)}`,
            automatic_uploads: true,
            paste_data_images: true,

            images_upload_handler: function (blobInfo, success, failure, progress) {
                const formData = new FormData();
                formData.append('file', blobInfo.blob(), blobInfo.filename());

                //let tempImageId = 'temp_' + Date.now();
                //let editor = tinymce.get(editorId);
                //let tempImage = editor.dom.select('img[src*="blob:"],img[src*="data:image"]').pop();
                //if (tempImage) {
                //    tempImage.setAttribute('data-temp-id', tempImageId);
                //    dotNetObjRef.invokeMethodAsync('OnImagePasted', tempImageId);
                //}

                fetch(`${baseUrl}common/images/upload?entityId=${encodeURIComponent(entityId)}&fieldName=${encodeURIComponent(fieldName)}`, {
                    method: 'POST',
                    headers: {
                        'Authorization': `Bearer ${authToken}`,
                        'Accept': '*/*'
                    },
                    body: formData,
                })
                    .then(res => {
                        if (!res.ok) {
                            throw new Error(`HTTP error: ${res.status} - ${res.statusText}`);
                        }
                        return res.json();
                    })
                    .then(json => {
                        if (json && json.location) {
                            console.log('Image uploaded successfully:', json.location);
                            success(json.location);
                            dotNetHelper.invokeMethodAsync('OnImageUploaded', json.location);
                        } else {
                            console.error('Invalid response:', json);
                            if (typeof failure === 'function') {
                                failure('Invalid response: No location provided');
                                dotNetHelper.invokeMethodAsync('OnEditorError', 'Image upload failed: No location provided');
                            }
                        }
                    })
                    .catch(err => {
                        console.error('Image upload error:', err.message);
                        if (typeof failure === 'function') {
                            failure('Upload error: ' + err.message);
                            dotNetHelper.invokeMethodAsync('OnEditorError', 'Image upload failed: ' + err.message);
                        }
                    });
            },
            ai_request: (request, respondWith) =>
                respondWith.string(() => Promise.reject('See docs to implement AI Assistant')),
            setup: function (editor) {
                editor.on('change', function () {
                    dotNetHelper.invokeMethodAsync('OnEditorChange', editor.getContent());
                });
                //editor.on('init', function () {
                //    editor.setContent();
                //});
            }
        });
    },
    setContent: function (textareaId, content) {
        tinymce.get(textareaId)?.setContent(content);
    },
    getContent: function (textareaId) {
        return tinymce.get(textareaId)?.getContent();
    }
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