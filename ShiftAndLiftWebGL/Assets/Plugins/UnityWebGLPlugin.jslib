/**
 * Bu dosya eklenecek oyunun Plugin klasöründe bulunmalıdır.
 */

mergeInto(LibraryManager.library, {
    SendResultToWebGL: function (resultJsonString) {
        dispatchEvent(
            new CustomEvent("UnityWebGLResultDataSent", {
                detail: JSON.parse(UTF8ToString(resultJsonString)),
            })
        );
    },
});
