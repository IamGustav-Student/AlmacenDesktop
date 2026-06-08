const express = require('express');
const validateLicense = require('./api/validate');
const handleWebhook = require('./api/webhook');
require('dotenv').config();

const app = express();
const PORT = process.env.PORT || 3000;

// Configurar parser JSON para los endpoints
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// RUTA INICIAL (Verificación de Servidor)
app.get('/', (req, res) => {
  res.status(200).json({
    status: "active",
    service: "VENDEMAX Licensing API Backend",
    developer: "HEXASTRATEGY",
    timestamp: new Date().toISOString()
  });
});

// ENDPOINT DE VALIDACIÓN (POS -> Server)
app.post('/api/licencia/validar', validateLicense);

// ENDPOINT DE WEBHOOK (Mercado Pago / Simulación -> Server)
app.post('/api/webhook', handleWebhook);

// Levantar el servidor sólo si se ejecuta directamente (desarrollo local)
if (require.main === module) {
  app.listen(PORT, () => {
    console.log(`VENDEMAX Licensing Server listening on port ${PORT}`);
  });
}

module.exports = app;
