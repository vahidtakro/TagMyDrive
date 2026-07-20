import { NextRequest, NextResponse } from 'next/server'
import { verifyToken } from '@/lib/auth'
import { query } from '@/lib/db'

export async function GET(
  request: NextRequest,
  { params }: { params: { diskId: string } }
) {
  try {
	const tokenData = await verifyToken(request)
	if (!tokenData || !('userId' in tokenData)) {
	  return NextResponse.json({ message: 'Unauthorized' }, { status: 401 })
	}

	const userId = (tokenData as any).userId
	const diskId = parseInt(params.diskId)

	const diskResults: any[] = await query(
	  'SELECT id FROM disks WHERE id = ? AND user_id = ? AND is_active = TRUE',
	  [diskId, userId]
	)

	if (diskResults.length === 0) {
	  return NextResponse.json({ message: 'Disk not found' }, { status: 404 })
	}

	const qrCodes = await query(
	  `SELECT id, qr_code_url, html_export_filename, created_at, expires_at,
			  is_active, download_count, last_accessed
	   FROM qr_codes WHERE disk_id = ? AND user_id = ? ORDER BY created_at DESC`,
	  [diskId, userId]
	)

	return NextResponse.json({ qrCodes }, { status: 200 })
  } catch (error: any) {
	return NextResponse.json({ message: 'Failed to fetch QR codes' }, { status: 500 })
  }
}

export async function POST(
  request: NextRequest,
  { params }: { params: { diskId: string } }
) {
  try {
	const tokenData = await verifyToken(request)
	if (!tokenData || !('userId' in tokenData)) {
	  return NextResponse.json({ message: 'Unauthorized' }, { status: 401 })
	}

	const userId = (tokenData as any).userId
	const diskId = parseInt(params.diskId)
	const { qrCodeUrl, htmlExportFilename } = await request.json()

	const diskResults: any[] = await query(
	  'SELECT id FROM disks WHERE id = ? AND user_id = ? AND is_active = TRUE',
	  [diskId, userId]
	)

	if (diskResults.length === 0) {
	  return NextResponse.json({ message: 'Disk not found' }, { status: 404 })
	}

	let url = qrCodeUrl

	if (!url) {
	  const gdriveLinks: any[] = await query(
		'SELECT google_drive_url FROM google_drive_links WHERE disk_id = ? AND user_id = ? AND is_active = TRUE ORDER BY uploaded_at DESC LIMIT 1',
		[diskId, userId]
	  )
	  if (gdriveLinks.length > 0 && gdriveLinks[0].google_drive_url) {
		url = gdriveLinks[0].google_drive_url
	  } else {
		url = `${process.env.NEXT_PUBLIC_API_URL || 'http://localhost:3000'}/view/${diskId}`
	  }
	}

	await query(
	  `INSERT INTO qr_codes (disk_id, user_id, qr_code_url, html_export_filename, expires_at)
	   VALUES (?, ?, ?, ?, DATE_ADD(NOW(), INTERVAL 1 YEAR))`,
	  [diskId, userId, url, htmlExportFilename || '']
	)

	return NextResponse.json({ message: 'QR code created successfully' }, { status: 201 })
  } catch (error: any) {
	return NextResponse.json({ message: 'Failed to create QR code' }, { status: 500 })
  }
}

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
	const qrId = searchParams.get('qrId')

	if (!qrId) {
	  return NextResponse.json({ message: 'QR code ID required' }, { status: 400 })
	}

	const results: any[] = await query(
	  'UPDATE qr_codes SET is_active = FALSE WHERE id = ? AND disk_id = ? AND user_id = ?',
	  [parseInt(qrId), parseInt(params.diskId), userId]
	)

	return NextResponse.json({ message: 'QR code deactivated' }, { status: 200 })
  } catch (error: any) {
	return NextResponse.json({ message: 'Failed to delete QR code' }, { status: 500 })
  }
}
