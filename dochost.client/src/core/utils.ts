export const blobToBase64 = function (blob: Blob) {
    return new Promise((resolve: (value: string | ArrayBuffer | null) => void) => {
        const reader = new FileReader();
        reader.onload = function () {
            const dataUrl = reader.result;
            resolve(dataUrl);
        };
        reader.readAsDataURL(blob);
    });
}