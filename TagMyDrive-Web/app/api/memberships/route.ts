import { NextRequest, NextResponse } from 'next/server'
import { verifyToken } from '@/lib/auth'
import { query } from '@/lib/db'

export async function GET(request: NextRequest) {
  try {
	const tokenData = await verifyToken(request)

	const memberships = await query(
	  'SELECT id, name, description, max_disks, price_monthly FROM memberships ORDER BY max_disks'
	)

	let userMembership = null
	if (tokenData && 'userId' in tokenData) {
	  const userId = (tokenData as any).userId
	  const users: any[] = await query(
		'SELECT membership_id FROM users WHERE id = ?',
		[userId]
	  )
	  if (users.length > 0) {
		userMembership = users[0].membership_id
	  }
	}

	const walletAddress = process.env.CRYPTO_WALLET_USDT || ''

	return NextResponse.json({
	  memberships,
	  userMembershipId: userMembership,
	  cryptoEnabled: !!walletAddress,
	  walletAddress,
	}, { status: 200 })
  } catch (error: any) {
	return NextResponse.json({ message: 'Failed to fetch memberships' }, { status: 500 })
  }
}
