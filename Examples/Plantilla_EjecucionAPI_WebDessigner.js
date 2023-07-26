/********************************************************************
			Declarción de Variables Globales 
********************************************************************/

var Date_Init = '';//Setear este dato en el código del formulario
var Date_End = '';//Setear este dato en el código del formulario

var vVirtualCC = icc.ui.getApplication().getVCCName();
//Fecha de generación del Reporte
var FechaGeneracion = '';

//Datos para el reporte
//Url del API para exportar la tabla del reporte en pdf
var urlExportPDF = 'http://IP_MW:PUERTO/api/Pdf'
//Nombre del Reporte
var nameReport = 'pruebaExportacionPDF'
//Titulo del reporte
var titleReport = 'PDF REPORTE'

var pageSize = 'A3-horizontal' //Opciones -> (A1-horizontal, A2-horizontal, A3-horizontal, A4-horizontal, A5-horizontal, letter-horizontal, tabloid-horizontal
        //                                            A1-vertical, A2-vertical, A3-vertical, A4-vertical, A5-vertical, letter-vertical, tabloid-vertical)


/*

Aquí va el código del formulario

*/


/********************************************************************
					Declarción de Funciones
********************************************************************/

//Obtiene la fecha y hora actual (YYYY-MM-DD HH:MM:SS)
function getFormattedDateTime() {
	const now = new Date();

	const year = now.getFullYear();
	const month = String(now.getMonth() + 1).padStart(2, '0');
	const day = String(now.getDate()).padStart(2, '0');
	const hour = String(now.getHours()).padStart(2, '0');
	const minute = String(now.getMinutes()).padStart(2, '0');
	const second = String(now.getSeconds()).padStart(2, '0');

	return `${year}-${month}-${day} ${hour}:${minute}:${second}`;
}


//Funciòn exporta tabla en PDF (Observaciòn: es necesario el API "ExportReport")
function exportarPDF() {
	// Mapear datos de la tabla
	const tableData = [];
	const tableHeaders = [];

	// Obtener encabezados de la tabla
	const headerCells = document.querySelectorAll('.hdrcell');
	headerCells.forEach(cell => tableHeaders.push(cell.innerText));

	// Obtener filas de datos de la tabla
	const dataRows = document.querySelectorAll('.objbox tr');
	dataRows.forEach(row => {
		const rowData = [];
		const dataCells = row.querySelectorAll('td');
		dataCells.forEach(cell => rowData.push(cell.innerText));
		tableData.push(rowData);
	});

	// Create the JSON object for the API request
	const request = {
		"nombreArchivo": nameReport,
		"tituloReporte": titleReport,
		"vcc": vVirtualCC,
		"fechaGeneracion": getFormattedDateTime(),
		"periodo": Date_Init + " 00:00:00  -  " + Date_End + " 23:59:59",
		"TamañoPagina": pageSize,
		"MargenIzquierdo": 20,
		"MargenDerecho": 20,
		"MargenSuperior": 30,
		"MargenInferior": 30,
		"FuenteTitulo": "Helvetica-Bold",
		"FuenteCeldasEncabezado": "Helvetica-Bold",
		"FuenteCeldasContenido": "Helvetica",
		"TamañoLetraTitulo": 20,
		"TamañoLetraCeldasEncabezado": 8,
		"TamañoLetraCeldasContenido": 8,
		"datosTabla": []
	};

	// Crear el array JSON para la propiedad "datosTabla"
	for (let i = 0; i < tableHeaders.length; i++) {
		request.datosTabla.push({
			"nameEncabezado": tableHeaders[i],
			"DatosColumna": tableData.map(row => row[i])
		});
	}

	console.log("Data", JSON.stringify(request));

	const blob = new Blob([JSON.stringify(result)], { type: 'application/pdf' });


	try {

		icc.ui.getApplication().ajaxCall('POST', "json", urlExportPDF, JSON.stringify(request), {
			"Content-Type": "application/json"
		}, {}, function callback(status, result) {
			if (status) {
				// Convertir el contenido en Base64 y descargar el PDF
				downloadPDF(result.base64Content, nameReport + 'pdf');
				console.log('PDF exportado correctamente.');
				icc.ui.displayMessage('PDF exportado correctamente.');
				//console.log('result: ', result.base64Content);
			} else {
				console.error('Error en la solicitud: ' + status);
				console.log('No se pudo exportar el PDF.');
				icc.ui.displayMessage('No se pudo exportar el PDF.');
				icc.ui.displayMessage('STATUS: ' + status);
				console.log('result: ', result);
				return false;
			}
		});
	} catch (e) {
		console.error('Error:', e);
		icc.ui.displayMessage('No se pudo exportar el PDF.');
		return false;
	}
}

//Función para convertir de base64 a PDF y descargarlo
function downloadPDF(base64Content, fileName) {
	// Decodificar el contenido Base64
	const byteCharacters = atob(base64Content);

	// Convertir el contenido en un array de bytes
	const byteNumbers = new Array(byteCharacters.length);
	for (let i = 0; i < byteCharacters.length; i++) {
		byteNumbers[i] = byteCharacters.charCodeAt(i);
	}
	const byteArray = new Uint8Array(byteNumbers);

	// Crear el Blob con el contenido y el tipo de archivo PDF
	const pdfBlob = new Blob([byteArray], { type: 'application/pdf' });

	// Crear un objeto URL para el Blob
	const pdfURL = URL.createObjectURL(pdfBlob);

	// Crear un enlace para descargar el archivo
	const a = document.createElement('a');
	a.href = pdfURL;
	a.download = fileName;
	a.style.display = 'none';

	// Agregar el enlace al documento y simular un clic para descargar el archivo
	document.body.appendChild(a);
	a.click();

	// Liberar el objeto URL
	URL.revokeObjectURL(pdfURL);
}