import { NextRequest, NextResponse } from 'next/server'
import { verifyToken } from '@/lib/auth'
import { query } from '@/lib/db'

export async function GET(request: NextRequest) {
  try {
	const tokenData = await verifyToken(request)
	if (!tokenData || !('userId' in tokenData)) {
	  return NextResponse.json({ message: 'Unauthorized' }, { status: 401 })
	}

	const userId = (tokenData as any).userId

	const disks = await query(
	  'SELECT id, name, description, disk_path, disk_type, image_url, created_at, updated_at FROM disks WHERE user_id = ? AND is_active = TRUE ORDER BY created_at DESC',
	  [userId]
	)

	return NextResponse.json({ disks }, { status: 200 })
  } catch (error: any) {
	return NextResponse.json({ message: 'Failed to fetch disks' }, { status: 500 })
  }
}

export async function POST(request: NextRequest) {
  try {
	const tokenData = await verifyToken(request)
	if (!tokenData || !('userId' in tokenData)) {
	  return NextResponse.json({ message: 'Unauthorized' }, { status: 401 })
	}

	const userId = (tokenData as any).userId
	const body = await request.json()
	const { name, description, diskPath, diskType, imageUrl } = body

	if (!name) {
	  return NextResponse.json({ message: 'Disk name is required' }, { status: 400 })
	}

	if (imageUrl) {
	  await query(
		'INSERT INTO disks (user_id, name, description, disk_path, disk_type, image_url) VALUES (?, ?, ?, ?, ?, ?)',
		[userId, name, description || '', diskPath || '', diskType || 'local', imageUrl]
	  )
	} else {
	  await query(
		'INSERT INTO disks (user_id, name, description, disk_path, disk_type) VALUES (?, ?, ?, ?, ?)',
		[userId, name, description || '', diskPath || '', diskType || 'local']
	  )
	}

	return NextResponse.json({ message: 'Disk created successfully' }, { status: 201 })
  } catch (error: any) {
	if (error?.message?.includes('Duplicate')) {
	  return NextResponse.json({ message: 'A disk with this name already exists' }, { status: 400 })
	}
	return NextResponse.json({ message: 'Failed to create disk' }, { status: 500 })
  }
}
