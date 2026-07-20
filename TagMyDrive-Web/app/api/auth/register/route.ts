import { NextRequest, NextResponse } from 'next/server'
import bcrypt from 'bcryptjs'
import { query } from '@/lib/db'

export async function POST(request: NextRequest) {
  try {
	const { username, email, firstName, lastName, password } = await request.json()

	if (!username || !email || !password) {
	  return NextResponse.json(
		{ message: 'Username, email, and password are required' },
		{ status: 400 }
	  )
	}

	if (password.length < 6) {
	  return NextResponse.json(
		{ message: 'Password must be at least 6 characters' },
		{ status: 400 }
	  )
	}

	// Check if user already exists
	const existing: any[] = await query(
	  'SELECT id FROM users WHERE username = ? OR email = ?',
	  [username, email]
	)

	if (existing.length > 0) {
	  return NextResponse.json(
		{ message: 'Username or email already exists' },
		{ status: 400 }
	  )
	}

	// Hash password
	const passwordHash = await bcrypt.hash(password, 10)

	// Create user (default to Free membership - ID 1)
	await query(
	  'INSERT INTO users (username, email, password_hash, first_name, last_name, membership_id) VALUES (?, ?, ?, ?, ?, 1)',
	  [username, email, passwordHash, firstName || '', lastName || '']
	)

	// Get the new user ID
	const newUser: any[] = await query(
	  'SELECT id FROM users WHERE username = ?',
	  [username]
	)

	return NextResponse.json(
	  {
		userId: newUser[0].id,
		message: 'Registration successful',
	  },
	  { status: 201 }
	)
  } catch (error: any) {
	return NextResponse.json(
	  { message: 'Registration failed: ' + error.message },
	  { status: 500 }
	)
  }
}
