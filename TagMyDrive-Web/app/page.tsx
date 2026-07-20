'use client'

import { useEffect } from 'react'
import { useRouter } from 'next/navigation'

export default function Home() {
  const router = useRouter()

  useEffect(() => {
	const token = localStorage.getItem('authToken')
	if (token) router.push('/dashboard')
  }, [router])

  return (
	<div className="min-h-screen bg-gradient-to-br from-blue-600 via-blue-700 to-indigo-800 flex items-center justify-center">
	  <div className="text-center text-white px-4">
		<h1 className="text-5xl sm:text-6xl font-extrabold mb-4 tracking-tight">TagMyDrive</h1>
		<p className="text-lg sm:text-xl text-blue-100 mb-10 max-w-md mx-auto">
		  Manage your disks, generate QR codes, and share via Google Drive — all in one place.
		</p>
		<div className="space-x-4">
		  <button
			onClick={() => router.push('/login')}
			className="px-8 py-3 bg-white text-blue-700 font-bold rounded-lg hover:bg-blue-50 transition shadow-lg"
		  >
			Login
		  </button>
		  <button
			onClick={() => router.push('/register')}
			className="px-8 py-3 bg-blue-500/50 text-white font-bold rounded-lg hover:bg-blue-500/70 transition border border-white/30"
		  >
			Register
		  </button>
		</div>
		<div className="mt-16 grid grid-cols-1 sm:grid-cols-3 gap-6 max-w-2xl mx-auto text-left">
		  <div className="bg-white/10 rounded-lg p-4 backdrop-blur">
			<div className="text-2xl mb-2">📂</div>
			<h3 className="font-semibold">Disk Management</h3>
			<p className="text-sm text-blue-200 mt-1">Track and organize your hard drives with metadata</p>
		  </div>
		  <div className="bg-white/10 rounded-lg p-4 backdrop-blur">
			<div className="text-2xl mb-2">📱</div>
			<h3 className="font-semibold">QR Codes</h3>
			<p className="text-sm text-blue-200 mt-1">Generate shareable QR codes for your disk snapshots</p>
		  </div>
		  <div className="bg-white/10 rounded-lg p-4 backdrop-blur">
			<div className="text-2xl mb-2">☁️</div>
			<h3 className="font-semibold">Google Drive</h3>
			<p className="text-sm text-blue-200 mt-1">Auto-upload snapshots and share via Drive links</p>
		  </div>
		</div>
	  </div>
	</div>
  )
}
