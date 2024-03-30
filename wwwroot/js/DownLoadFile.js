window.downloadFileFromBase64 = function (filename, base64) {
    // Convertir la cadena base64 en un Blob
    var byteCharacters = atob(base64);
    var byteNumbers = new Array(byteCharacters.length);
    for (var i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    var byteArray = new Uint8Array(byteNumbers);
    var blob = new Blob([byteArray], { type: 'image/png' }); 

    // Crear un enlace temporal
    var link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = filename;

    // Simular clic en el enlace para iniciar la descarga
    document.body.appendChild(link);
    link.click();

    // Limpiar el enlace del DOM
    document.body.removeChild(link);
};


function downloadFileFromByte(fileName, byteArray) {
    var blob = new Blob([new Uint8Array(byteArray)], { type: 'image/png' });

    var link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = fileName;

    link.click();

    window.URL.revokeObjectURL(link.href);
}

function downloadFileZip(fileName, byteArray) {
    var blob = new Blob([new Uint8Array(byteArray)], { type: 'application/zip' });

    var link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = fileName;

    link.click();

    window.URL.revokeObjectURL(link.href);
}


