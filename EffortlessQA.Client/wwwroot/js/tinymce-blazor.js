window.tinyMCEBlazor = {
    initEditor: function (textareaId, dotNetHelper, entityId, fieldName, authToken, baseUrl) {
        tinymce.init({
            selector: `#${textareaId}`,
            height: 500,
            plugins: [
                'anchor', 'autolink', 'charmap', 'codesample', 'emoticons', 'image', 'link', 'lists', 'media', 'searchreplace', 'table',
                'visualblocks', 'wordcount', 'checklist', 'mediaembed', 'casechange', 'formatpainter', 'pageembed', 'a11ychecker',
                'tinymcespellchecker', 'permanentpen', 'powerpaste', 'advtable', 'advcode', 'editimage', 'advtemplate', 'ai', 'mentions',
                'tinycomments', 'tableofcontents', 'footnotes', 'mergetags', 'autocorrect', 'typography', 'inlinecss', 'markdown',
                'importword', 'exportword', 'exportpdf'
            ],
            toolbar: 'undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | ' +
                'link image media table mergetags | addcomment showcomments | spellcheckdialog a11ycheck typography | ' +
                'align lineheight | checklist numlist bullist indent outdent | emoticons charmap | removeformat',
            tinycomments_mode: 'embedded',
            tinycomments_author: 'Author name',
            mergetags_list: [
                { value: 'First.Name', title: 'First Name' },
                { value: 'Email', title: 'Email' }
            ],
            images_upload_url: `${baseUrl}common/images/upload?entityId=${encodeURIComponent(entityId)}&fieldName=${encodeURIComponent(fieldName)}`,
            automatic_uploads: true,
            paste_data_images: true,
            //paste_data_images: false,
            images_upload_handler: function (blobInfo, success, failure, progress) {
                const formData = new FormData();
                formData.append('file', blobInfo.blob(), blobInfo.filename());

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