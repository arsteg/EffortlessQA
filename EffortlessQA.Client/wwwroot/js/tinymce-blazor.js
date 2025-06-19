window.tinyMCEBlazor = {
    initEditor: function (textareaId, dotNetHelper) {
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
            images_upload_url: '/api/upload/image', // 👈 Your backend endpoint
            automatic_uploads: true,
            images_upload_handler: function (blobInfo, success, failure) {
                const formData = new FormData();
                formData.append('file', blobInfo.blob(), blobInfo.filename());

                fetch('/api/upload/image', {
                    method: 'POST',
                    body: formData
                })
                    .then(res => res.json())
                    .then(json => {
                        if (json.location) success(json.location);
                        else failure("Upload failed.");
                    })
                    .catch(err => failure("Upload error: " + err));
            },
            ai_request: (request, respondWith) =>
                respondWith.string(() => Promise.reject('See docs to implement AI Assistant')),
            setup: function (editor) {
                editor.on('change', function () {
                    dotNetHelper.invokeMethodAsync('OnEditorChange', editor.getContent());
                });
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
