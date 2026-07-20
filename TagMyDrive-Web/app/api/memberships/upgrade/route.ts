import { NextRequest, NextResponse } from 'next/server'
import { verifyToken } from '@/lib/auth'
import { query } from '@/lib/db'

const USDT_BEP20_CONTRACT = '0x55d398326f99059fF775485246999027B3197955'
const REQUIRED_CONFIRMATIONS = 12
const WALLET_ADDRESS = process.env.CRYPTO_WALLET_USDT || ''
const BSCSCAN_API_KEY = process.env.BSCSCAN_API_KEY || ''

interface MembershipPrice {
  id: number
  name: string
  price_monthly: number
}

export async function POST(request: NextRequest) {
  try {
	const tokenData = await verifyToken(request)
	if (!tokenData || !('userId' in tokenData)) {
	  return NextResponse.json({ message: 'Unauthorized' }, { status: 401 })
	}

	const userId = (tokenData as any).userId
	const { txHash, membershipId } = await request.json()

	if (!txHash || !membershipId) {
	  return NextResponse.json(
		{ message: 'Transaction hash and membership ID are required' },
		{ status: 400 }
	  )
	}

	if (!WALLET_ADDRESS || !BSCSCAN_API_KEY) {
	  return NextResponse.json(
		{ message: 'Crypto payments are not configured on this server' },
		{ status: 503 }
	  )
	}

	// Get membership price
	const memberships: MembershipPrice[] = await query(
	  'SELECT id, name, price_monthly FROM memberships WHERE id = ?',
	  [membershipId]
	)

	if (memberships.length === 0) {
	  return NextResponse.json({ message: 'Invalid membership' }, { status: 400 })
	}

	const membership = memberships[0]
	if (membership.price_monthly <= 0) {
	  return NextResponse.json({ message: 'Free tier does not require payment' }, { status: 400 })
	}

	// Check if this tx hash was already used
	const existingPayments: any[] = await query(
		"SELECT id FROM crypto_payments WHERE transaction_hash = ? AND status = 'confirmed'",
		[txHash]
	)

	if (existingPayments.length > 0) {
	  return NextResponse.json({ message: 'This transaction has already been used' }, { status: 400 })
	}

	// Fetch tx receipt from BSCScan
	const receiptUrl = `https://api.bscscan.com/api?module=proxy&action=eth_getTransactionByHash&txhash=${txHash}&apikey=${BSCSCAN_API_KEY}`
	const receiptRes = await fetch(receiptUrl)
	const receiptData = await receiptRes.json()

	if (receiptData.result?.blockNumber === null || receiptData.result?.blockNumber === undefined) {
	  return NextResponse.json(
		{ message: 'Transaction not found or not yet mined' },
		{ status: 400 }
	  )
	}

	const txBlockNumber = parseInt(receiptData.result.blockNumber, 16)

	// Get current block number for confirmation count
	const blockUrl = `https://api.bscscan.com/api?module=proxy&action=eth_blockNumber&apikey=${BSCSCAN_API_KEY}`
	const blockRes = await fetch(blockUrl)
	const blockData = await blockRes.json()
	const currentBlock = parseInt(blockData.result, 16)
	const confirmations = currentBlock - txBlockNumber

	if (confirmations < REQUIRED_CONFIRMATIONS) {
	  // Record as pending
	  await query(
		`INSERT INTO crypto_payments (user_id, membership_id, coin, wallet_address, amount, status, transaction_hash, confirmations)
		 VALUES (?, ?, 'USDT', ?, ?, 'awaiting_verification', ?, ?)
		 ON DUPLICATE KEY UPDATE confirmations = ?, status = 'awaiting_verification'`,
		[userId, membershipId, WALLET_ADDRESS, membership.price_monthly, txHash, confirmations, confirmations]
	  )

	  return NextResponse.json(
		{
		  message: `Transaction found but needs more confirmations (${confirmations}/${REQUIRED_CONFIRMATIONS})`,
		  confirmations,
		  required: REQUIRED_CONFIRMATIONS,
		  status: 'awaiting_verification',
		},
		{ status: 200 }
	  )
	}

	// Verify USDT transfer via internal transactions
	const internalUrl = `https://api.bscscan.com/api?module=account&action=tokentx&contractaddress=${USDT_BEP20_CONTRACT}&address=${WALLET_ADDRESS}&apikey=${BSCSCAN_API_KEY}`
	const internalRes = await fetch(internalUrl)
	const internalData = await internalRes.json()

	if (internalData.status !== '1' || !internalData.result) {
	  await query(
		`INSERT INTO crypto_payments (user_id, membership_id, coin, wallet_address, amount, status, transaction_hash, confirmations)
		 VALUES (?, ?, 'USDT', ?, ?, 'rejected', ?, ?)`,
		[userId, membershipId, WALLET_ADDRESS, membership.price_monthly, txHash, confirmations]
	  )
	  return NextResponse.json({ message: 'Could not verify USDT transfer' }, { status: 400 })
	}

	// Find the matching transfer
	const matchingTransfer = internalData.result.find((tx: any) => {
	  const value = parseFloat(tx.value) / Math.pow(10, parseInt(tx.tokenDecimal))
	  return (
		tx.hash.toLowerCase() === txHash.toLowerCase() &&
		tx.to?.toLowerCase() === WALLET_ADDRESS.toLowerCase() &&
		value >= membership.price_monthly
	  )
	})

	if (!matchingTransfer) {
	  await query(
		`INSERT INTO crypto_payments (user_id, membership_id, coin, wallet_address, amount, status, transaction_hash, confirmations)
		 VALUES (?, ?, 'USDT', ?, ?, 'rejected', ?, ?)`,
		[userId, membershipId, WALLET_ADDRESS, membership.price_monthly, txHash, confirmations]
	  )
	  return NextResponse.json(
		{ message: 'No matching USDT transfer found for the required amount' },
		{ status: 400 }
	  )
	}

	// Payment verified — upgrade membership
	await query('UPDATE users SET membership_id = ? WHERE id = ?', [membershipId, userId])

	await query(
	  `INSERT INTO crypto_payments (user_id, membership_id, coin, wallet_address, amount, status, transaction_hash, confirmations, verified_at)
	   VALUES (?, ?, 'USDT', ?, ?, 'confirmed', ?, ?, NOW())`,
	  [userId, membershipId, WALLET_ADDRESS, membership.price_monthly, txHash, confirmations]
	  )

	return NextResponse.json({
	  message: `Payment verified! Membership upgraded to ${membership.name}`,
	  membership: { id: membership.id, name: membership.name },
	}, { status: 200 })
  } catch (error: any) {
	return NextResponse.json({ message: 'Payment verification failed' }, { status: 500 })
  }
}
