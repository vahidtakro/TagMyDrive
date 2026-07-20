import { NextRequest, NextResponse } from 'next/server'
import { verifyToken } from '@/lib/auth'
import { query } from '@/lib/db'

export async function GET(
  request: NextRequest,
  { params }: { params: { id: string } }
) {
  try {
	const tokenData = await verifyToken(request)
	if (!tokenData || !('userId' in tokenData)) {
	  return NextResponse.json({ message: 'Unauthorized' }, { status: 401 })
	}

	const userId = (tokenData as any).userId
	const diskId = parseInt(params.id)

	const results: any[] = await query(
	  'SELECT id, name, description, disk_path, disk_type, image_url, created_at, updated_at FROM disks WHERE id = ? AND user_id = ? AND is_active = TRUE',
	  [diskId, userId]
	)

	if (results.length === 0) {
	  return NextResponse.json({ message: 'Disk not found' }, { status: 404 })
	}

	return NextResponse.json({ disk: results[0] }, { status: 200 })
  } catch (error: any) {
	return NextResponse.json({ message: 'Error loading disk' }, { status: 500 })
  }
}

export async function PUT(
  request: NextRequest,
  { params }: { params: { id: string } }
) {
  try {
	const tokenData = await verifyToken(request)
	if (!tokenData || !('userId' in tokenData)) {
	  return NextResponse.json({ message: 'Unauthorized' }, { status: 401 })
	}

	const userId = (tokenData as any).userId
	const diskId = parseInt(params.id)
	const body = await request.json()
	const { name, description, imageUrl, removeImage } = body

	const parts: string[] = []
	const values: any[] = []

	if (name !== undefined) { parts.push('name = ?'); values.push(name) }
	if (description !== undefined) { parts.push('description = ?'); values.push(description) }
	if (removeImage === true) { parts.push('image_url = NULL') }
	if (imageUrl !== undefined && imageUrl !== null) { parts.push('image_url = ?'); values.push(imageUrl) }

	if (parts.length > 0) {
	  parts.push('updated_at = NOW()')
	  values.push(diskId, userId)
	  await query(
		`UPDATE disks SET ${parts.join(', ')} WHERE id = ? AND user_id = ?`,
		values
	  )
	}

	return NextResponse.json({ message: 'Disk updated successfully' }, { status: 200 })
  } catch (error: any) {
	if (error?.message?.includes('Duplicate')) {
	  return NextResponse.json({ message: 'A disk with this name already exists' }, { status: 400 })
	}
	return NextResponse.json({ message: 'Failed to update disk' }, { status: 500 })
  }
}

export async function DELETE(
  request: NextRequest,
  { params }: { params: { id: string } }
) {
  try {
	const tokenData = await verifyToken(request)
	if (!tokenData || !('userId' in tokenData)) {
	  return NextResponse.json({ message: 'Unauthorized' }, { status: 401 })
	}

	const userId = (tokenData as any).userId
	const diskId = parseInt(params.id)

	await query(
	  'UPDATE disks SET is_active = FALSE, updated_at = NOW() WHERE id = ? AND user_id = ?',
	  [diskId, userId]
	)

	return NextResponse.json({ message: 'Disk deleted successfully' }, { status: 200 })
  } catch (error: any) {
	return NextResponse.json({ message: 'Failed to delete disk' }, { status: 500 })
  }
}
