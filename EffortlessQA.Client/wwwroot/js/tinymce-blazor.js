window.tinyMCEBlazor = {
    initEditor: function (textareaId, dotNetHelper, entityId, fieldName, authToken) {
        tinymce.init({
            selector: `#${textareaId}`,
            height: 500,
            plugins: [
                'anchor', 'autolink', 'charmap', 'codesample', 'emoticons', 'image', 'link', 'lists', 'media', 'searchreplace', 'table',
                'visualblocks', 'wordcount', 'checklist', 'mediaembed', 'casechange', 'formatpainter', 'pageembed', 'a11ychecker',
                'tinymcespellchecker', 'permanentpen', 'powerpaste', 'advtable', 'advcode', 'editimage', 'advtemplate', 'mentions',
                'tinycomments', 'tableofcontents', 'footnotes', 'mergetags', 'autocorrect', 'typography', 'inlinecss', 'markdown'
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
            images_upload_url: `https://localhost:7196/api/v1/common/images/upload`,
            automatic_uploads: true,
            paste_data_images: true,
            images_upload_handler: function (blobInfo, success, failure, progress) {
                const formData = new FormData();
                formData.append('file', blobInfo.blob(), blobInfo.filename());

                fetch(`https://localhost:7196/api/v1/common/images/upload`, {
                    method: 'POST',
                    headers: {
                        'Authorization': `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJlMDlhZDc5MS00MGQxLTQ5MzctODI5Yi1lNGM5Y2Q3ZTYyN2YiLCJlbWFpbCI6Im1vaGRyYWZpb25saW5lQGdtYWlsLmNvbSIsInRlbmFudElkIjoiNTUwYjA2ZjQ3ZDI4NDkxMGJhM2MyNzE1MGU1MmFhMTgiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsImV4cCI6MTc4MjI5Njc5MywiaXNzIjoiRWZmb3J0bGVzc1FBIiwiYXVkIjoiRWZmb3J0bGVzc1FBVXNlcnMifQ.5PrfCfRYGd9DQyx2a27b2by7gCftasrfSFsYEgq4Vrw`,
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
            setup: function (editor) {
                editor.on('change', function () {
                    dotNetHelper.invokeMethodAsync('OnEditorChange', editor.getContent());
                });
                editor.on('init', function () {
                    editor.setContent('');
                });
            }
        });
    },
    setContent: function (textareaId, content) {
        const editor = tinymce.get(textareaId);
        if (editor) {
            editor.setContent(content || '');
        }
    },
    getContent: function (textareaId) {
        const editor = tinymce.get(textareaId);
        return editor ? editor.getContent() : '';
    }
};