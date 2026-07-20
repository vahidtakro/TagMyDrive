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

	const diskResults: any[] = await query(
	  'SELECT id FROM disks WHERE id = ? AND user_id = ? AND is_active = TRUE',
	  [diskId, userId]
	)

	if (diskResults.length === 0) {
	  return NextResponse.json({ message: 'Disk not found' }, { status: 404 })
	}

	const [qrCount]: any[] = await query(
	  'SELECT COUNT(*) as count FROM qr_codes WHERE disk_id = ? AND is_active = TRUE',
	  [diskId]
	)

	const [gdriveCount]: any[] = await query(
	  'SELECT COUNT(*) as count FROM google_drive_links WHERE disk_id = ? AND is_active = TRUE',
	  [diskId]
	)

	const [totalDownloads]: any[] = await query(
	  'SELECT COALESCE(SUM(download_count), 0) as total FROM qr_codes WHERE disk_id = ? AND is_active = TRUE',
	  [diskId]
	)

	const gdriveLinks: any[] = await query(
	  `SELECT id, google_file_id, google_file_name, google_drive_url, html_file_name, uploaded_at
	   FROM google_drive_links WHERE disk_id = ? AND is_active = TRUE ORDER BY uploaded_at DESC`,
	  [diskId]
	)

	const latestQr: any[] = await query(
	  `SELECT id, qr_code_url, created_at, download_count
	   FROM qr_codes WHERE disk_id = ? AND is_active = TRUE ORDER BY created_at DESC LIMIT 1`,
	  [diskId]
	)

	return NextResponse.json({
	  qrCount: qrCount.count,
	  gdriveCount: gdriveCount.count,
	  totalDownloads: totalDownloads.total,
	  gdriveLinks,
	  latestQr: latestQr[0] || null,
	}, { status: 200 })
  } catch (error: any) {
	return NextResponse.json({ message: 'Failed to fetch disk stats' }, { status: 500 })
  }
}
