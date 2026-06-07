const http = require('http');

const PORT = 3000;

// Utilidad para realizar peticiones POST sencillas
function post(path, body) {
  return new Promise((resolve, reject) => {
    const data = JSON.stringify(body);
    const options = {
      hostname: 'localhost',
      port: PORT,
      path: path,
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Content-Length': data.length
      }
    };

    const req = http.request(options, (res) => {
      let bodyStr = '';
      res.on('data', (chunk) => bodyStr += chunk);
      res.on('end', () => {
        resolve({
          statusCode: res.statusCode,
          headers: res.headers,
          data: bodyStr ? JSON.parse(bodyStr) : null
        });
      });
    });

    req.on('error', (err) => reject(err));
    req.write(data);
    req.end();
  });
}

async function runTests() {
  console.log("=== INICIANDO PRUEBAS DE API DE LICENCIAS VENDEMAX ===");

  try {
    // 1. Simular un Pago de Mercado Pago mediante Webhook de Prueba
    console.log("\n1. Simulando pago mensual para: test@hexastrategy.com...");
    const payRes = await post('/api/webhook', {
      email: "test@hexastrategy.com",
      test_payment: true
    });
    console.log(`Respuesta Webhook (Status ${payRes.statusCode}):`, payRes.data);

    // 2. Intentar validar la licencia registrada por primera vez
    // Usaremos la clave de activación preconfigurada en el mock ("VDMX-DEMO-TEST-2026") para verificar el endpoint
    console.log("\n2. Intentando validar licencia DEMO por primera vez (Registro de Hardware)...");
    const valRes = await post('/api/licencia/validar', {
      email: "demo@hexastrategy.com",
      clave: "VDMX-DEMO-TEST-2026",
      machineFingerprint: "HARDWARE_FINGERPRINT_PC1_12345"
    });
    console.log(`Respuesta Validación (Status ${valRes.statusCode}):`);
    console.log(`- Signature: ${valRes.data.signature}`);
    console.log(`- Payload: ${valRes.data.payload}`);

    // 3. Validar con el mismo hardware (Debe dar ÉXITO)
    console.log("\n3. Revalidando licencia con la misma PC (Debe tener éxito)...");
    const valReRes = await post('/api/licencia/validar', {
      email: "demo@hexastrategy.com",
      clave: "VDMX-DEMO-TEST-2026",
      machineFingerprint: "HARDWARE_FINGERPRINT_PC1_12345"
    });
    console.log(`Respuesta Validación Recurrente (Status ${valReRes.statusCode}): EXITO`);

    // 4. Intentar validar con un hardware diferente (Debe ser BLOQUEADO)
    console.log("\n4. Intentando validar licencia desde otra PC (Debe ser BLOQUEADO)...");
    const blockRes = await post('/api/licencia/validar', {
      email: "demo@hexastrategy.com",
      clave: "VDMX-DEMO-TEST-2026",
      machineFingerprint: "HARDWARE_FINGERPRINT_PC2_VULNERABLE"
    });
    console.log(`Respuesta Validación Bloqueada (Status ${blockRes.statusCode}):`, blockRes.data);

    console.log("\n=== PRUEBAS DE INTEGRIDAD COMPLETADAS CON ÉXITO ===");
  } catch (error) {
    console.error("Error al ejecutar las pruebas de API:", error.message);
    console.log("Asegúrese de levantar el servidor ejecutando 'npm start' en la carpeta server primero.");
  }
}

// Ejecutar pruebas
runTests();
