const crypto = require('crypto');
const db = require('../db');
require('dotenv').config();

const HMAC_SECRET = process.env.LICENSE_HMAC_SECRET || 'hexastrategy_vendemax_secret_key_default';

async function validateLicense(req, res) {
  const { email, clave, machineFingerprint } = req.body;

  if (!email || !clave || !machineFingerprint) {
    return res.status(400).json({ 
      valido: false, 
      mensaje: "Faltan parámetros requeridos: email, clave y machineFingerprint." 
    });
  }

  try {
    // Buscar la licencia
    const result = await db.query(
      'SELECT email_cliente, clave_activacion, machine_fingerprint, estado, fecha_vencimiento FROM licencias WHERE email_cliente = $1 AND clave_activacion = $2',
      [email.trim().toLowerCase(), clave.trim().toUpperCase()]
    );

    if (result.rows.length === 0) {
      return res.status(404).json({
        valido: false,
        mensaje: "No se encontró ninguna licencia activa con el correo y la clave provistos."
      });
    }

    const licencia = result.rows[0];

    // Verificar si la fecha de vencimiento ya expiró
    const ahora = new Date();
    const vencimiento = new Date(licencia.fecha_vencimiento);

    if (ahora > vencimiento) {
      // Auto-suspender en base de datos si expiró
      if (licencia.estado === 'ACTIVO') {
        await db.query(
          "UPDATE licencias SET estado = 'SUSPENDIDO' WHERE email_cliente = $1",
          [licencia.email_cliente]
        );
        licencia.estado = 'SUSPENDIDO';
      }
    }

    // Verificar el estado
    if (licencia.estado !== 'ACTIVO') {
      return res.status(403).json({
        valido: false,
        mensaje: `La licencia no está activa. Estado actual: ${licencia.estado}.`
      });
    }

    // Verificar vinculación de hardware (Machine Fingerprint)
    if (!licencia.machine_fingerprint) {
      // Registrar la huella por primera vez
      await db.query(
        'UPDATE licencias SET machine_fingerprint = $1 WHERE email_cliente = $2',
        [machineFingerprint, licencia.email_cliente]
      );
      licencia.machine_fingerprint = machineFingerprint;
      console.log(`Clave ${clave} asociada exitosamente al hardware: ${machineFingerprint}`);
    } else if (licencia.machine_fingerprint !== machineFingerprint) {
      // El hardware no coincide
      return res.status(403).json({
        valido: false,
        mensaje: "Esta licencia ya se encuentra activada y vinculada a otra computadora."
      });
    }

    // Licencia válida - Generar payload de éxito y firmar con HMAC
    const payload = {
      valido: true,
      email: licencia.email_cliente,
      clave: licencia.clave_activacion,
      fechaVencimiento: vencimiento.toISOString(),
      timestamp: Date.now()
    };

    // Firmar la cadena JSON usando HMAC-SHA256
    const payloadString = JSON.stringify(payload);
    const signature = crypto
      .createHmac('sha256', HMAC_SECRET)
      .update(payloadString)
      .digest('hex');

    return res.status(200).json({
      payload: payloadString,
      signature: signature
    });

  } catch (err) {
    console.error("Error al validar licencia:", err);
    return res.status(500).json({
      valido: false,
      mensaje: "Error interno del servidor al procesar la validación."
    });
  }
}

module.exports = validateLicense;
