var DateInicio = ''
var DateFin = ''

var Date_Init = '';
var Date_End = '';


var vVirtualCC = icc.ui.getApplication().getVCCName();
//Fecha de generación del Reporte
var FechaGeneracion = '';

var pageSize = 'A3-horizontal' //Opciones -> (A1-horizontal, A2-horizontal, A3-horizontal, A4-horizontal, A5-horizontal, letter-horizontal, tabloid-horizontal
        //                                            A1-vertical, A2-vertical, A3-vertical, A4-vertical, A5-vertical, letter-vertical, tabloid-vertical)


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
	const data = {
		"nombreArchivo": nameReport,
		"tituloReporte": titleReport,
		"vcc": vVirtualCC,
		"fechaGeneracion": getFormattedDateTime(),
		"periodo": Date_Init + "00:00:00  -  " + Date_End + "23:59:59",
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
		data.datosTabla.push({
			"nameEncabezado": tableHeaders[i],
			"DatosColumna": tableData.map(row => row[i])
		});
	}

	console.log("Data", JSON.stringify(data));

	// Send the POST request to the API
	fetch(urlExportPDF, {
		method: 'POST',
		headers: {
			'Accept': 'application/pdf',
			'Content-Type': 'application/json'
		},
		body: JSON.stringify(data)
	})
		.then(response => {
			if (!response.ok) {
				throw new Error('Error en la solicitud: ' + response.status + ' ' + response.statusText);
			}
			return response.blob();
		})
		.then(blob => {
			// Convert the blob to a downloadable PDF
			const url = URL.createObjectURL(blob);
			const a = document.createElement('a');
			a.href = url;
			a.download = 'pruebaExportacionPDF.pdf';
			a.click();
			console.log('PDF exportado correctamente.');
		})
		.catch(error => {
			console.error('Error:', error);
			console.log('No se pudo exportar el PDF.');
		});
}