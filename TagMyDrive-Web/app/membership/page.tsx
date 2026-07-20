'use client'

import { useEffect, useState } from 'react'
import { useRouter } from 'next/navigation'
import axios from 'axios'

interface Membership {
  id: number
  name: string
  description: string
  max_disks: number
  price_monthly: number
}

export default function MembershipPage() {
  const [memberships, setMemberships] = useState<Membership[]>([])
  const [currentMembershipId, setCurrentMembershipId] = useState<number>(1)
  const [cryptoEnabled, setCryptoEnabled] = useState(false)
  const [walletAddress, setWalletAddress] = useState('')
  const [txHash, setTxHash] = useState('')
  const [selectedPlan, setSelectedPlan] = useState<number | null>(null)
  const [verifying, setVerifying] = useState(false)
  const [verifyResult, setVerifyResult] = useState<{ success: boolean; message: string; confirmations?: number; required?: number } | null>(null)
  const [loading, setLoading] = useState(true)
  const router = useRouter()

  const token = typeof window !== 'undefined' ? localStorage.getItem('authToken') : null
  const authHeader = token ? { Authorization: `Bearer ${token}` } : {}

  useEffect(() => {
	if (!token) { router.push('/login'); return }
	loadMemberships()
  }, [])

  const loadMemberships = async () => {
	try {
	  const res = await axios.get('/api/memberships', { headers: authHeader })
	  setMemberships(res.data.memberships || [])
	  setCurrentMembershipId(res.data.userMembershipId || 1)
	  setCryptoEnabled(res.data.cryptoEnabled || false)
	  setWalletAddress(res.data.walletAddress || '')
	} catch (err: any) {
	  if (err.response?.status === 401) { router.push('/login'); return }
	} finally { setLoading(false) }
  }

  const handleVerifyPayment = async () => {
	if (!txHash.trim() || !selectedPlan) return
	setVerifying(true)
	setVerifyResult(null)
	try {
	  const res = await axios.post('/api/memberships/upgrade', {
		txHash: txHash.trim(),
		membershipId: selectedPlan,
	  }, { headers: authHeader })
	  setVerifyResult({ success: true, message: res.data.message })
	  setCurrentMembershipId(selectedPlan)
	  // Update localStorage user data
	  const userStr = localStorage.getItem('user')
	  if (userStr) {
		const user = JSON.parse(userStr)
		user.membershipId = selectedPlan
		localStorage.setItem('user', JSON.stringify(user))
	  }
	} catch (err: any) {
	  const data = err.response?.data
	  setVerifyResult({
		success: false,
		message: data?.message || 'Verification failed',
		confirmations: data?.confirmations,
		required: data?.required,
	  })
	} finally { setVerifying(false) }
  }

  if (loading) {
	return (
	  <div className="flex items-center justify-center min-h-screen bg-gray-50">
		<div className="text-gray-500">Loading memberships...</div>
	  </div>
	)
  }

  return (
	<div className="min-h-screen bg-gray-50">
	  {/* Header */}
	  <div className="bg-white shadow-sm border-b">
		<div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
		  <div className="flex items-center gap-4">
			<button onClick={() => router.push('/dashboard')} className="text-gray-500 hover:text-gray-700">
			  <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" /></svg>
			</button>
			<h1 className="text-2xl font-bold text-gray-900">Membership Plans</h1>
		  </div>
		</div>
	  </div>

	  <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
		{/* Plan Cards */}
		<div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
		  {memberships.map((m) => {
			const isCurrent = m.id === currentMembershipId
			const isUpgrade = m.id > currentMembershipId
			return (
			  <div
				key={m.id}
				className={`bg-white rounded-xl border-2 p-6 transition cursor-pointer ${
				  selectedPlan === m.id ? 'border-blue-500 shadow-lg' :
				  isCurrent ? 'border-green-400 shadow-sm' : 'border-gray-200 hover:border-gray-300'
				}`}
				onClick={() => { if (isUpgrade && m.price_monthly > 0) setSelectedPlan(m.id) }}
			  >
				{isCurrent && (
				  <div className="text-xs font-semibold text-green-700 bg-green-100 px-2 py-1 rounded-full inline-block mb-3">
					Current Plan
				  </div>
				)}
				<h3 className="text-xl font-bold text-gray-900">{m.name}</h3>
				<p className="text-gray-500 text-sm mt-1">{m.description}</p>
				<div className="mt-4">
				  <span className="text-3xl font-bold text-gray-900">
					{m.price_monthly > 0 ? `$${m.price_monthly}` : 'Free'}
				  </span>
				  {m.price_monthly > 0 && <span className="text-gray-500 text-sm">/month</span>}
				</div>
				<div className="mt-4 space-y-2 text-sm text-gray-600">
				  <div className="flex items-center gap-2">
					<span className="text-green-500">✓</span>
					{m.max_disks >= 999 ? 'Unlimited disks' : `Up to ${m.max_disks} disks`}
				  </div>
				  <div className="flex items-center gap-2">
					<span className="text-green-500">✓</span>
					QR Code generation
				  </div>
				 	{m.id >= 2 && (
					<div className="flex items-center gap-2">
					  <span className="text-green-500">✓</span>
					  Google Drive upload
					</div>
				  )}
				</div>
				{isUpgrade && m.price_monthly > 0 && (
				  <div className="mt-4">
					<button
					  className={`w-full py-2 rounded-lg text-sm font-medium transition ${
						selectedPlan === m.id
						  ? 'bg-blue-600 text-white'
						  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
					  }`}
					>
					  {selectedPlan === m.id ? 'Selected' : 'Upgrade'}
					</button>
				  </div>
				)}
			  </div>
			)
		  })}
		</div>

		{/* Crypto Payment Section */}
		{selectedPlan && cryptoEnabled && (
		  <div className="bg-white rounded-xl border p-6">
			<h3 className="text-lg font-bold text-gray-900 mb-4">Crypto Payment (USDT BEP20)</h3>

			<div className="bg-gray-50 rounded-lg p-4 mb-4">
			  <p className="text-sm text-gray-600 mb-2">Send USDT (BEP20) to this wallet address:</p>
			  <div className="flex items-center gap-2">
				<code className="text-sm bg-white border rounded px-3 py-2 flex-1 break-all font-mono">{walletAddress}</code>
				<button
				  onClick={() => navigator.clipboard.writeText(walletAddress)}
				  className="px-3 py-2 text-sm bg-gray-200 rounded-lg hover:bg-gray-300 transition flex-shrink-0"
				>
				  Copy
				</button>
			  </div>
			  <p className="text-xs text-gray-500 mt-2">
				Amount: <strong>${memberships.find(m => m.id === selectedPlan)?.price_monthly} USDT</strong> (BEP20 network only)
			  </p>
			</div>

			<div className="mb-4">
			  <label className="block text-sm font-medium text-gray-700 mb-1">Transaction Hash</label>
			  <input
				value={txHash}
				onChange={(e) => setTxHash(e.target.value)}
				placeholder="0x..."
				className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm font-mono focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
			  />
			</div>

			<button
			  onClick={handleVerifyPayment}
			  disabled={verifying || !txHash.trim()}
			  className="w-full py-2.5 bg-blue-600 text-white font-medium rounded-lg hover:bg-blue-700 transition disabled:opacity-50 disabled:cursor-not-allowed"
			>
			  {verifying ? 'Verifying Transaction...' : 'Verify & Upgrade'}
			</button>

			{verifyResult && (
			  <div className={`mt-4 p-4 rounded-lg text-sm ${verifyResult.success ? 'bg-green-50 text-green-700 border border-green-200' : 'bg-red-50 text-red-700 border border-red-200'}`}>
				<p className="font-medium">{verifyResult.message}</p>
				{verifyResult.confirmations !== undefined && verifyResult.required !== undefined && (
				  <p className="mt-1 text-xs">
					Confirmations: {verifyResult.confirmations} / {verifyResult.required}
				  </p>
				)}
			  </div>
			)}

			<p className="text-xs text-gray-400 mt-4">
			  Payment is verified automatically via BSCScan. You need 12+ confirmations. Do not close this page during verification.
			</p>
		  </div>
		)}

		{selectedPlan && !cryptoEnabled && (
		  <div className="bg-yellow-50 border border-yellow-200 rounded-xl p-6 text-center">
			<p className="text-yellow-700 font-medium">Crypto payments are not configured on this server.</p>
			<p className="text-sm text-yellow-600 mt-1">Contact the administrator to enable membership upgrades.</p>
		  </div>
		)}
	  </div>
	</div>
  )
}
