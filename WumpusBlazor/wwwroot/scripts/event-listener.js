window.JsFunctions = {
    addKeyboardListenerEvent: function () {
        let serializeKeyEvent = function (evt) {
            if (evt) {
                return {
                    key: evt.key,
                    code: evt.keyCode.toString(),
                    location: evt.location,
                    repeat: evt.repeat,
                    ctrlKey: evt.ctrlKey,
                    shiftKey: evt.shiftKey,
                    altKey: evt.altKey,
                    metaKey: evt.metaKey,
                    type: evt.type
                };
            }
        };

        window.document.addEventListener('keydown', function (evt) {
            DotNet.invokeMethodAsync('WumpusBlazor', 'HandleKeyDown', serializeKeyEvent(evt))
        });
    }
};

window.ElementFunctions = {
    blurElement: function(element) {
        element.blur();
    }
}
