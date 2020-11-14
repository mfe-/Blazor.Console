// This file is to show how a library package may provide JavaScript interop features
// wrapped in a .NET API

window.BlazorConsole = {
    scrollToBottom: function () {
        window.scrollTo(0, document.body.scrollHeight);
    },
    setFocusToElement: function (elementId) {
        const element = document.getElementById(elementId);
        element.focus();
    }
};
