import { NextRequest, NextResponse } from 'next/server'
import { verifyToken } from '@/lib/auth'
import { query } from '@/lib/db'

// GET Google Drive links for a disk
export async function GET(
  request: NextRequest,
  { params }: { params: { diskId: string } }
) {
  try {
	const tokenData = await verifyToken(request)
	if (!tokenData || !('userId' in tokenData)) {
	  return NextResponse.json(
		{ message: 'Unauthorized' },
		{ status: 401 }
	  )
	}

	const userId = (tokenData as any).userId
	const diskId = parseInt(params.diskId)

	// Verify user owns this disk
	const diskResults: any[] = await query(
	  'SELECT id FROM disks WHERE id = ? AND user_id = ?',
	  [diskId, userId]
	)

	if (diskResults.length === 0) {
	  return NextResponse.json(
		{ message: 'Disk not found' },
		{ status: 404 }
	  )
	}

	const links = await query(
	  'SELECT id, google_file_id, google_file_name, google_drive_url, uploaded_at FROM google_drive_links WHERE disk_id = ? AND is_active = TRUE ORDER BY uploaded_at DESC',
	  [diskId]
	)

	return NextResponse.json({ links }, { status: 200 })
  } catch (error: any) {
	return NextResponse.json(
	  { message: 'Error: ' + error.message },
	  { status: 500 }
	)
  }
}

// Save Google Drive link
export async function POST(
  request: NextRequest,
  { params }: { params: { diskId: string } }
) {
  try {
	const tokenData = await verifyToken(request)
	if (!tokenData || !('userId' in tokenData)) {
	  return NextResponse.json(
		{ message: 'Unauthorized' },
		{ status: 401 }
	  )
	}

	const userId = (tokenData as any).userId
	const diskId = parseInt(params.diskId)
	const { googleFileId, googleFileName, googleDriveUrl, htmlFileName } = await request.json()

	// Verify user owns this disk
	const diskResults: any[] = await query(
	  'SELECT id FROM disks WHERE id = ? AND user_id = ?',
	  [diskId, userId]
	)

	if (diskResults.length === 0) {
	  return NextResponse.json(
		{ message: 'Disk not found' },
		{ status: 404 }
	  )
	}

	await query(
	  'INSERT INTO google_drive_links (disk_id, user_id, google_file_id, google_file_name, google_drive_url, html_file_name) VALUES (?, ?, ?, ?, ?, ?)',
	  [diskId, userId, googleFileId, googleFileName, googleDriveUrl, htmlFileName]
	)

	return NextResponse.json(
	  { message: 'Google Drive link saved successfully' },
	  { status: 201 }
	)
  } catch (error: any) {
	return NextResponse.json(
	  { message: 'Error: ' + error.message },
	  { status: 500 }
	)
  }
}

// Deactivate Google Drive link
export async function DELETE(
  request: NextRequest,
  { params }: { params: { diskId: string } }
) {
  try {
	const tokenData = await verifyToken(request)
	if (!tokenData || !('userId' in tokenData)) {
	  return NextResponse.json({ message: 'Unauthorized' }, { status: 401 })
	}

	const userId = (tokenData as any).userId
	const { searchParams } = new URL(request.url)
	const linkId = searchParams.get('linkId')

	if (!linkId) {
	  return NextResponse.json({ message: 'Link ID required' }, { status: 400 })
	}

	await query(
	  'UPDATE google_drive_links SET is_active = FALSE WHERE id = ? AND disk_id = ? AND user_id = ?',
	  [parseInt(linkId), parseInt(params.diskId), userId]
	)

	return NextResponse.json({ message: 'Link deactivated' }, { status: 200 })
  } catch (error: any) {
	return NextResponse.json({ message: 'Failed to deactivate link' }, { status: 500 })
  }
}
