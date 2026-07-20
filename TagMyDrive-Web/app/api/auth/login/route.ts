import { NextRequest, NextResponse } from 'next/server'
import bcrypt from 'bcryptjs'
import { query } from '@/lib/db'
import { generateToken } from '@/lib/auth'

export async function POST(request: NextRequest) {
  try {
	const { username, password } = await request.json()

	if (!username || !password) {
	  return NextResponse.json(
		{ message: 'Username and password are required' },
		{ status: 400 }
	  )
	}

	// Query user from database
	const results: any[] = await query(
	  'SELECT id, username, email, password_hash, first_name, last_name, membership_id FROM users WHERE (username = ? OR email = ?) AND is_active = TRUE',
	  [username, username]
	)

	if (results.length === 0) {
	  return NextResponse.json(
		{ message: 'Invalid username or password' },
		{ status: 401 }
	  )
	}

	const user = results[0]

	// Verify password
	const passwordMatch = await bcrypt.compare(password, user.password_hash)
	if (!passwordMatch) {
	  return NextResponse.json(
		{ message: 'Invalid username or password' },
		{ status: 401 }
	  )
	}

	// Generate token
	const token = generateToken(user.id)

	return NextResponse.json(
	  {
		token,
		user: {
		  id: user.id,
		  username: user.username,
		  email: user.email,
		  firstName: user.first_name,
		  lastName: user.last_name,
		  membershipId: user.membership_id,
		},
	  },
	  { status: 200 }
	)
  } catch (error: any) {
	return NextResponse.json(
	  { message: 'Login failed: ' + error.message },
	  { status: 500 }
	)
  }
}
