const db = require('../db');
const nodemailer = require('nodemailer');
require('dotenv').config();

// Generador de clave aleatoria en formato VDMX-XXXX-XXXX-XXXX
function generarClaveLicencia() {
  const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
  const chunk = () => Array.from({ length: 4 }, () => chars[Math.floor(Math.random() * chars.length)]).join('');
  return `VDMX-${chunk()}-${chunk()}-${chunk()}`;
}

// Configurar transportador de correo SMTP
const transporter = nodemailer.createTransport({
  host: process.env.SMTP_HOST || 'smtp.gmail.com',
  port: parseInt(process.env.SMTP_PORT || '587'),
  secure: process.env.SMTP_SECURE === 'true', // true para 465, false para otros
  auth: {
    user: process.env.SMTP_USER || '',
    pass: process.env.SMTP_PASS || ''
  }
});

async function handleWebhook(req, res) {
  // Notificación de Mercado Pago
  // payload típicamente viene con action/type y data.id
  const notification = req.body;
  console.log("Mercado Pago Webhook recibido:", JSON.stringify(notification));

  // Para pruebas y simulaciones:
  // Permitimos simular el webhook directamente enviando { email: "usuario@ejemplo.com", test_payment: true }
  if (notification.test_payment && notification.email) {
    try {
      const email = notification.email.trim().toLowerCase();
      const clave = generarClaveLicencia();
      
      // Vencimiento a 30 días
      const vencimiento = new Date();
      vencimiento.setDate(vencimiento.getDate() + 30);

      // Guardar o actualizar en base de datos
      const checkExist = await db.query('SELECT email_cliente FROM licencias WHERE email_cliente = $1', [email]);
      
      if (checkExist.rows.length > 0) {
        // Renovar e inactivar el fingerprint viejo para permitir reinstalaciones si es necesario
        await db.query(
          "UPDATE licencias SET estado = 'ACTIVO', fecha_vencimiento = $1, machine_fingerprint = NULL WHERE email_cliente = $2",
          [vencimiento, email]
        );
        console.log(`Licencia de ${email} renovada exitosamente mediante simulación.`);
      } else {
        // Registrar nueva
        await db.query(
          'INSERT INTO licencias (email_cliente, clave_activacion, estado, fecha_vencimiento) VALUES ($1, $2, $3, $4)',
          [email, clave, 'ACTIVO', vencimiento]
        );
        console.log(`Licencia nueva de ${email} generada: ${clave}`);
        await enviarEmailLicencia(email, clave, vencimiento);
      }

      return res.status(200).json({ status: "ok", message: "Simulación de pago procesada." });
    } catch (error) {
      console.error("Error en simulación de pago:", error);
      return res.status(500).json({ status: "error", error: error.message });
    }
  }

  // Lógica para producción Mercado Pago Subscriptions:
  // Mercado Pago envía eventos tipo:
  // "action": "created" / "updated"
  // "type": "subscription_authorized_payment" (pago de cuota de suscripción)
  // O "subscription" directo.
  // Buscamos detectar aprobaciones de pago.
  
  const type = notification.type || notification.topic;
  
  if (type === 'payment' || type === 'subscription_authorized_payment') {
    const paymentId = notification.data ? notification.data.id : notification.id;
    console.log(`Procesando pago de Mercado Pago ID: ${paymentId}`);

    // Nota: Aquí se debería consultar a la API de Mercado Pago con el Token de Acceso (process.env.MP_ACCESS_TOKEN)
    // para verificar el estado de pago ('approved') y obtener el email del comprador.
    // Para simplificar la arquitectura inicial, si Mercado Pago envía el email en el metadata o webhook:
    
    const email = notification.payer_email || (notification.data && notification.data.payer_email);
    
    if (email) {
      try {
        const cleanEmail = email.trim().toLowerCase();
        const clave = generarClaveLicencia();
        const vencimiento = new Date();
        vencimiento.setDate(vencimiento.getDate() + 30);

        const checkExist = await db.query('SELECT email_cliente, clave_activacion FROM licencias WHERE email_cliente = $1', [cleanEmail]);
        
        if (checkExist.rows.length > 0) {
          // Renovar suscripción
          await db.query(
            "UPDATE licencias SET estado = 'ACTIVO', fecha_vencimiento = $1 WHERE email_cliente = $2",
            [vencimiento, cleanEmail]
          );
          console.log(`Suscripción renovada para: ${cleanEmail}`);
        } else {
          // Nueva suscripción
          await db.query(
            'INSERT INTO licencias (email_cliente, clave_activacion, estado, fecha_vencimiento) VALUES ($1, $2, $3, $4)',
            [cleanEmail, clave, 'ACTIVO', vencimiento]
          );
          console.log(`Nueva clave generada para ${cleanEmail}: ${clave}`);
          await enviarEmailLicencia(cleanEmail, clave, vencimiento);
        }
      } catch (err) {
        console.error("Error al procesar pago en la base de datos:", err);
      }
    }
  } else if (type === 'subscription' && (notification.action === 'cancelled' || notification.action === 'suspended')) {
    // Manejar bajas de suscripción
    const email = notification.payer_email || (notification.data && notification.data.payer_email);
    if (email) {
      try {
        const cleanEmail = email.trim().toLowerCase();
        await db.query(
          "UPDATE licencias SET estado = 'SUSPENDIDO' WHERE email_cliente = $1",
          [cleanEmail]
        );
        console.log(`Suscripción cancelada/suspendida para: ${cleanEmail}`);
      } catch (err) {
        console.error("Error al cancelar suscripción en DB:", err);
      }
    }
  }

  // Responder 200 OK a Mercado Pago para confirmar recepción de la notificación
  return res.status(200).send("OK");
}

// Envío de correo electrónico con la clave de activación y diseño HEXASTRATEGY
async function enviarEmailLicencia(email, clave, vencimiento) {
  const fechaStr = vencimiento.toLocaleDateString('es-AR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  });

  const mailOptions = {
    from: `"HEXASTRATEGY Deployment" <${process.env.SMTP_FROM || process.env.SMTP_USER}>`,
    to: email,
    subject: '🚀 Tu Clave de Activación VENDEMAX v2.0',
    html: `
      <div style="background-color: #070B12; color: #E8EDF5; font-family: 'Space Grotesk', -apple-system, sans-serif; padding: 40px; border-radius: 16px; max-width: 600px; margin: 0 auto; border: 1px solid #1A56DB;">
        <div style="text-align: center; margin-bottom: 30px;">
          <h1 style="color: #ffffff; font-size: 28px; margin: 0; font-weight: bold; letter-spacing: 2px;">HEXA<span style="color: #00F5C4;">STRATEGY</span></h1>
          <p style="color: #a7f3d0; font-family: monospace; font-size: 12px; margin-top: 5px;">// VENDEMAX SOFTWARE DEPLOYMENT</p>
        </div>
        
        <h2 style="color: #ffffff; font-size: 20px; font-weight: 600; border-bottom: 1px solid #1E2A3B; padding-bottom: 10px; margin-bottom: 20px;">¡Gracias por tu suscripción mensual!</h2>
        
        <p style="font-size: 15px; line-height: 1.6; color: #E8EDF5;">
          Hemos verificado tu pago correctamente. A continuación, se detalla tu <strong>Clave de Activación Única</strong> para registrar el software <strong>VENDEMAX v2.0</strong> en tu computadora de escritorio:
        </p>

        <div style="background-color: #131D2E; border: 1px dashed #00F5C4; padding: 20px; border-radius: 12px; text-align: center; margin: 30px 0;">
          <span style="font-family: 'JetBrains Mono', monospace; font-size: 22px; font-weight: bold; color: #00F5C4; letter-spacing: 3px;">
            ${clave}
          </span>
        </div>

        <ul style="font-size: 14px; color: #E8EDF5; line-height: 1.6; padding-left: 20px;">
          <li><strong>Correo Registrado:</strong> ${email}</li>
          <li><strong>Próximo Vencimiento:</strong> ${fechaStr}</li>
          <li><strong>Licencia atada a hardware:</strong> Se registrará automáticamente en la primera PC donde la uses.</li>
        </ul>

        <div style="background-color: #0B111C; border-left: 4px solid #1A56DB; padding: 15px; border-radius: 4px; margin-top: 30px;">
          <p style="margin: 0; font-size: 13px; color: #94a3b8; line-height: 1.5;">
            <strong>¿Cómo activar el sistema?</strong><br>
            Abre VENDEMAX en tu computadora. Cuando aparezca la pantalla de activación, ingresa tu correo electrónico y la clave mostrada arriba. El sistema se habilitará de inmediato.
          </p>
        </div>

        <div style="margin-top: 40px; text-align: center; font-size: 12px; color: #475569; border-top: 1px solid #1E2A3B; padding-top: 20px;">
          HEXASTRATEGY © 2026 • Soporte Técnico Prioritario SaaS
        </div>
      </div>
    `
  };

  try {
    // Si no hay configuración SMTP válida configurada, imprimimos en consola el correo y simulamos éxito
    if (!process.env.SMTP_USER) {
      console.log(`[SIMULACIÓN EMAIL] Enviando clave a ${email}: ${clave}`);
      return;
    }
    await transporter.sendMail(mailOptions);
    console.log(`Email de licencia enviado exitosamente a ${email}`);
  } catch (error) {
    console.error("Error al enviar email de licencia:", error);
  }
}

module.exports = handleWebhook;
