window.initializeEditor = function (editorElement, initialContent, dotNetRef) {
    editorElement.innerHTML = initialContent;
    editorElement.addEventListener('input', function () {
        dotNetRef.invokeMethodAsync('UpdateValue', editorElement.innerHTML)
            .catch(err => console.error('Error invoking UpdateValue:', err));
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
                    console.log('FileReader loaded, sending pasted image to Blazor');
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
    editorElement.addEventListener('dragover', function (e) {
        e.preventDefault();
        e.dataTransfer.dropEffect = 'copy'; // Indicate a copy operation
        editorElement.style.border = '2px dashed #007bff'; // Visual feedback
    });
    editorElement.addEventListener('dragleave', function (e) {
        e.preventDefault();
        editorElement.style.border = ''; // Reset border
    });
    editorElement.addEventListener('drop', function (e) {
        e.preventDefault();
        editorElement.style.border = ''; // Reset border
        console.log('Drop event triggered');
        const files = e.dataTransfer.files;
        console.log('Dropped files:', files.length);
        if (files.length > 0) {
            const file = files[0];
            if (file.type.startsWith('image/')) {
                if (file.size > 5 * 1024 * 1024) {
                    console.log('Dropped file exceeds 5MB:', file.size);
                    dotNetRef.invokeMethodAsync('ShowError', 'Dropped file exceeds the 5MB size limit.');
                    return;
                }
                console.log('Dropped image detected:', file.type);
                const reader = new FileReader();
                reader.onload = function (event) {
                    console.log('FileReader loaded, sending dropped image to Blazor');
                    const blob = new Uint8Array(event.target.result);
                    dotNetRef.invokeMethodAsync('HandlePasteImage', blob, file.type, file.name || `dropped-image.${file.type.split('/')[1]}`)
                        .catch(err => console.error('Error invoking HandlePasteImage:', err));
                };
                reader.onerror = function (err) {
                    console.error('FileReader error:', err);
                    dotNetRef.invokeMethodAsync('ShowError', 'Failed to read dropped image');
                };
                reader.readAsArrayBuffer(file);
            } else {
                console.log('Dropped file is not an image:', file.type);
                dotNetRef.invokeMethodAsync('ShowError', 'Dropped file is not a valid image.');
            }
        }
    });
};

window.focusEditor = function (editorElement) {
    editorElement.focus();
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
    try {
        let range;
        const sel = window.getSelection();
        if (sel.rangeCount > 0) {
            range = sel.getRangeAt(0);
        } else {
            range = document.createRange();
            range.selectNodeContents(editorElement);
            range.collapse(false);
            sel.removeAllRanges();
            sel.addRange(range);
        }
        range.deleteContents();
        const div = document.createElement('div');
        div.innerHTML = imgTag;
        range.insertNode(div.firstChild);
        editorElement.focus();
        // Trigger input event to ensure Blazor updates
        const inputEvent = new Event('input', { bubbles: true });
        editorElement.dispatchEvent(inputEvent);
    } catch (err) {
        console.error('Error in insertImage:', err);
        throw err;
    }
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
    alert(message); // Replace with MudBlazor Snackbar or other notification
};

window.destroyEditor = function (editorElement) {
    editorElement.removeEventListener('input', null);
    editorElement.removeEventListener('paste', null);
    editorElement.removeEventListener('dragover', null);
    editorElement.removeEventListener('dragleave', null);
    editorElement.removeEventListener('drop', null);
    editorElement.innerHTML = '';
};