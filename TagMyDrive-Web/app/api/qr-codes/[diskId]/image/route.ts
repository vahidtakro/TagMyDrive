import { NextRequest, NextResponse } from 'next/server'
import { query } from '@/lib/db'

export async function GET(
  request: NextRequest,
  { params }: { params: { diskId: string } }
) {
  try {
	const diskId = parseInt(params.diskId)
	const { searchParams } = new URL(request.url)
	const qrId = searchParams.get('qrId')
	const size = parseInt(searchParams.get('size') || '300')

	let url: string | null = null

	if (qrId) {
	  const results: any[] = await query(
		'SELECT qr_code_url FROM qr_codes WHERE id = ? AND disk_id = ? AND is_active = TRUE',
		[parseInt(qrId), diskId]
	  )
	  if (results.length > 0) url = results[0].qr_code_url
	} else {
	  const results: any[] = await query(
		'SELECT qr_code_url FROM qr_codes WHERE disk_id = ? AND is_active = TRUE ORDER BY created_at DESC LIMIT 1',
		[diskId]
	  )
	  if (results.length > 0) url = results[0].qr_code_url
	}

	if (!url) {
	  return NextResponse.json({ message: 'No active QR code found' }, { status: 404 })
	}

	const QRCode = (await import('qrcode')).default
	const svg = await QRCode.toString(url, { type: 'svg', width: size, margin: 2 })

	return new NextResponse(svg, {
	  status: 200,
	  headers: {
		'Content-Type': 'image/svg+xml',
		'Cache-Control': 'public, max-age=3600',
	  },
	})
  } catch (error: any) {
	return NextResponse.json({ message: 'Failed to generate QR image' }, { status: 500 })
  }
}
