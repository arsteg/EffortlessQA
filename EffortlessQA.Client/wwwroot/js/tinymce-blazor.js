window.tinyMCEBlazor = {
    initEditor: function (textareaId, dotNetHelper) {
        tinymce.init({
            selector: `#${textareaId}`,
            height: 400,
            plugins: 'image code link lists table',
            toolbar: 'undo redo | formatselect | bold italic underline | alignleft aligncenter alignright | bullist numlist outdent indent | link image | code',
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
