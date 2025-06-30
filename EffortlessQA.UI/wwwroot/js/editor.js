window.initializeEditor = function (editorElement, initialContent, dotNetRef, entityId, fieldName) {
    editorElement.innerHTML = initialContent;
    editorElement.addEventListener('input', function () {
        dotNetRef.invokeMethodAsync('UpdateValue', editorElement.innerHTML);
    });
    editorElement.addEventListener('paste', function (e) {
        e.preventDefault();
        console.log('Paste event triggered');
        const items = (e.clipboardData || window.clipboardData).items;
        console.log('Clipboard items:', items.length);
        for (let i = 0; i < items.length; i++) {
            if (items[i].type.indexOf('image') !== -1) {
                console.log('Image detected in clipboard:', items[i].type);
                const file = items[i].getAsFile();
                const reader = new FileReader();
                reader.onload = function (event) {
                    console.log('FileReader loaded, sending to Blazor');
                    const blob = new Uint8Array(event.target.result);
                    dotNetRef.invokeMethodAsync('HandlePasteImage', blob, file.type, file.name || `pasted-image.${file.type.split('/')[1]}`)
                        .catch(err => console.error('Error invoking HandlePasteImage:', err));
                };
                reader.onerror = function (err) {
                    console.error('FileReader error:', err);
                    dotNetRef.invokeMethodAsync('ShowError', 'Failed to read pasted image');
                };
                reader.readAsArrayBuffer(file);
                break; // Process only the first image
            }
        }
    });
};

window.setEditorContentAndCursor = function (editorElement, content) {
    editorElement.innerHTML = content;
    const range = document.createRange();
    const sel = window.getSelection();
    editorElement.focus();
    range.selectNodeContents(editorElement);
    range.collapse(false);
    sel.removeAllRanges();
    sel.addRange(range);
};

window.getEditorContent = function (editorElement) {
    return editorElement.innerHTML;
};

window.execCommand = function (command, showUI, value) {
    document.execCommand(command, showUI, value);
};

window.insertImage = function (editorElement, imgTag) {
    console.log('Inserting image:', imgTag);
    const range = window.getSelection().getRangeAt(0);
    range.deleteContents();
    const div = document.createElement('div');
    div.innerHTML = imgTag;
    range.insertNode(div.firstChild);
};

window.saveCursorPosition = function (editorElement) {
    const sel = window.getSelection();
    if (sel.rangeCount > 0) {
        editorElement._savedRange = sel.getRangeAt(0);
    }
};

window.restoreCursorPosition = function (editorElement) {
    if (editorElement._savedRange) {
        const sel = window.getSelection();
        sel.removeAllRanges();
        sel.addRange(editorElement._savedRange);
        editorElement.focus();
    }
};

window.showEditorError = function (editorElement, message) {
    console.error('Editor error:', message);
    alert(message); // Replace with a better notification system if needed
};

window.destroyEditor = function (editorElement) {
    editorElement.removeEventListener('input', null);
    editorElement.removeEventListener('paste', null);
    editorElement.innerHTML = '';
};