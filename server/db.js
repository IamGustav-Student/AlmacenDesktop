const { Pool } = require('pg');
require('dotenv').config();

const connectionString = process.env.DATABASE_URL;
let pool = null;
let useMock = false;

// Base de datos en memoria para desarrollo si no hay PostgreSQL conectado
const mockDB = {
  licencias: [
    {
      id: 1,
      email_cliente: "demo@hexastrategy.com",
      clave_activacion: "VDMX-DEMO-TEST-2026",
      machine_fingerprint: null,
      estado: "ACTIVO",
      fecha_creacion: new Date(),
      fecha_vencimiento: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000) // 30 días en el futuro
    }
  ]
};

if (!connectionString) {
  console.warn("⚠️ ADVERTENCIA: La variable de entorno DATABASE_URL no está definida. Corriendo en MODO MOCK (Base de datos en memoria).");
  useMock = true;
} else {
  try {
    pool = new Pool({
      connectionString: connectionString,
      ssl: connectionString.includes('localhost') ? false : { rejectUnauthorized: false }
    });
  } catch (err) {
    console.error("❌ Error al crear el Pool de Postgres, cayendo en modo MOCK:", err.message);
    useMock = true;
  }
}

// Inicialización de la base de datos (PostgreSQL)
async function inicializarDB() {
  if (useMock) return;
  const queryCreateTable = `
    CREATE TABLE IF NOT EXISTS licencias (
      id SERIAL PRIMARY KEY,
      email_cliente VARCHAR(255) UNIQUE NOT NULL,
      clave_activacion VARCHAR(50) UNIQUE NOT NULL,
      machine_fingerprint VARCHAR(255),
      estado VARCHAR(50) NOT NULL DEFAULT 'ACTIVO',
      fecha_creacion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
      fecha_vencimiento TIMESTAMP NOT NULL
    );
  `;
  try {
    const client = await pool.connect();
    await client.query(queryCreateTable);
    client.release();
    console.log("✅ Database initialized: 'licencias' table verified/created.");
  } catch (err) {
    console.error("❌ Error al inicializar la base de datos real, cayendo en modo MOCK:", err.message);
    useMock = true;
  }
}

inicializarDB();

module.exports = {
  query: async (text, params) => {
    if (useMock) {
      console.log(`[MOCK DB] Query: ${text} | Params:`, params);
      
      // Simular select por email y clave
      if (text.includes('SELECT') && text.includes('email_cliente = $1 AND clave_activacion = $2')) {
        const [email, clave] = params;
        const found = mockDB.licencias.find(l => l.email_cliente === email && l.clave_activacion === clave);
        return { rows: found ? [found] : [] };
      }

      // Simular select por email solo
      if (text.includes('SELECT') && text.includes('email_cliente = $1')) {
        const [email] = params;
        const found = mockDB.licencias.find(l => l.email_cliente === email);
        return { rows: found ? [found] : [] };
      }

      // Simular update del machine_fingerprint
      if (text.includes('UPDATE') && text.includes('SET machine_fingerprint = $1 WHERE email_cliente = $2')) {
        const [fingerprint, email] = params;
        const found = mockDB.licencias.find(l => l.email_cliente === email);
        if (found) found.machine_fingerprint = fingerprint;
        return { rows: [] };
      }

      // Simular update del estado y fecha de vencimiento (Mercado Pago o expiración)
      if (text.includes('UPDATE') && text.includes("SET estado = 'ACTIVO', fecha_vencimiento = $1, machine_fingerprint = NULL WHERE email_cliente = $2")) {
        const [vencimiento, email] = params;
        const found = mockDB.licencias.find(l => l.email_cliente === email);
        if (found) {
          found.estado = 'ACTIVO';
          found.fecha_vencimiento = vencimiento;
          found.machine_fingerprint = null;
        }
        return { rows: [] };
      }

      if (text.includes('UPDATE') && text.includes("SET estado = 'ACTIVO', fecha_vencimiento = $1 WHERE email_cliente = $2")) {
        const [vencimiento, email] = params;
        const found = mockDB.licencias.find(l => l.email_cliente === email);
        if (found) {
          found.estado = 'ACTIVO';
          found.fecha_vencimiento = vencimiento;
        }
        return { rows: [] };
      }

      if (text.includes('UPDATE') && text.includes("SET estado = 'SUSPENDIDO' WHERE email_cliente = $1")) {
        const [email] = params;
        const found = mockDB.licencias.find(l => l.email_cliente === email);
        if (found) found.estado = 'SUSPENDIDO';
        return { rows: [] };
      }

      // Simular insert de nueva licencia
      if (text.includes('INSERT INTO licencias')) {
        const [email, clave, estado, vencimiento] = params;
        const nueva = {
          id: mockDB.licencias.length + 1,
          email_cliente: email,
          clave_activacion: clave,
          machine_fingerprint: null,
          estado: estado,
          fecha_creacion: new Date(),
          fecha_vencimiento: vencimiento
        };
        mockDB.licencias.push(nueva);
        return { rows: [] };
      }

      return { rows: [] };
    }

    // Usar la base de datos real Postgres
    return pool.query(text, params);
  },
  pool
};
