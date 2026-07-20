import mysql from 'mysql2/promise'

export async function getConnection() {
  return await mysql.createConnection({
	host: process.env.DB_HOST,
	port: parseInt(process.env.DB_PORT || '3306'),
	database: process.env.DB_NAME,
	user: process.env.DB_USER,
	password: process.env.DB_PASSWORD,
  })
}

export async function query<T = any>(sql: string, values?: any[]): Promise<T> {
  const connection = await getConnection()
  try {
	const [results] = await connection.execute(sql, values)
	return results as T
  } finally {
	await connection.end()
  }
}
