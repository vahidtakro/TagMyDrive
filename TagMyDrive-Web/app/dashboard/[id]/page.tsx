'use client'

import { useEffect, useState } from 'react'
import { useRouter, useParams } from 'next/navigation'
import axios from 'axios'

interface DiskDetail {
  id: number; name: string; description: string; disk_type: string; disk_path: string; created_at: string
}

interface QrCode {
  id: number; qr_code_url: string; html_export_filename: string; created_at: string; expires_at: string; is_active: boolean; download_count: number; last_accessed: string
}

interface GdriveLink {
  id: number; google_file_id: string; google_file_name: string; google_drive_url: string; html_file_name: string; uploaded_at: string
}

export default function DiskDetailPage() {
  const params = useParams()
  const diskId = params.id as string
  const router = useRouter()
  const [disk, setDisk] = useState<DiskDetail | null>(null)
  const [qrCodes, setQrCodes] = useState<QrCode[]>([])
  const [gdriveLinks, setGdriveLinks] = useState<GdriveLink[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [generatingQR, setGeneratingQR] = useState(false)

  const token = typeof window !== 'undefined' ? localStorage.getItem('authToken') : null
  const authHeader = token ? { Authorization: `Bearer ${token}` } : {}

  useEffect(() => {
	if (!token) { router.push('/login'); return }
	loadDisk()
  }, [diskId])

  const loadDisk = async () => {
	try {
	  const [diskRes, qrRes, gdriveRes] = await Promise.all([
		axios.get(`/api/disks/${diskId}`, { headers: authHeader }),
		axios.get(`/api/qr-codes/${diskId}`, { headers: authHeader }),
		axios.get(`/api/google-drive/${diskId}`, { headers: authHeader }),
	  ])
	  setDisk(diskRes.data.disk)
	  setQrCodes(qrRes.data.qrCodes || [])
	  setGdriveLinks(gdriveRes.data.links || [])
	} catch (err: any) {
	  if (err.response?.status === 401) { router.push('/login'); return }
	  setError(err.response?.data?.message || 'Failed to load disk')
	} finally { setLoading(false) }
  }

  const handleGenerateQR = async () => {
	setGeneratingQR(true)
	try {
	  await axios.post(`/api/qr-codes/${diskId}`, {}, { headers: authHeader })
	  loadDisk()
	} catch (err: any) {
	  setError(err.response?.data?.message || 'Failed to generate QR code')
	} finally { setGeneratingQR(false) }
  }

  const handleDeactivateQR = async (qrId: number) => {
	try {
	  await axios.delete(`/api/qr-codes/${diskId}?qrId=${qrId}`, { headers: authHeader })
	  loadDisk()
	} catch (err: any) {
	  setError(err.response?.data?.message || 'Failed to deactivate QR code')
	}
  }

  if (loading) {
	return (
	  <div className="flex items-center justify-center min-h-screen bg-gray-50">
		<div className="text-gray-500">Loading disk details...</div>
	  </div>
	)
  }

  if (!disk) {
	return (
	  <div className="flex items-center justify-center min-h-screen bg-gray-50">
		<div className="text-center">
		  <p className="text-gray-500 text-lg">Disk not found</p>
		  <button onClick={() => router.push('/dashboard')} className="mt-4 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
			Back to Dashboard
		  </button>
		</div>
	  </div>
	)
  }

  return (
	<div className="min-h-screen bg-gray-50">
	  {/* Header */}
	  <div className="bg-white shadow-sm border-b">
		<div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
		  <div className="flex items-center gap-4">
			<button onClick={() => router.push('/dashboard')} className="text-gray-500 hover:text-gray-700">
			  <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" /></svg>
			</button>
			<div>
			  <h1 className="text-xl font-bold text-gray-900">{disk.name}</h1>
			  <p className="text-sm text-gray-500 capitalize">{disk.disk_type} &middot; {disk.description || 'No description'}</p>
			</div>
		  </div>
		</div>
	  </div>

	  <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
		{error && (
		  <div className="mb-6 p-4 bg-red-50 text-red-700 rounded-lg border border-red-200">
			{error}
			<button onClick={() => setError('')} className="ml-2 font-bold">&times;</button>
		  </div>
		)}

		{/* QR Codes Section */}
		<div className="bg-white rounded-xl border mb-6">
		  <div className="p-6 border-b flex justify-between items-center">
			<div>
			  <h2 className="text-lg font-bold text-gray-900">QR Codes</h2>
			  <p className="text-sm text-gray-500">{qrCodes.filter(q => q.is_active).length} active codes</p>
			</div>
			<button
			  onClick={handleGenerateQR}
			  disabled={generatingQR}
			  className="px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition disabled:opacity-50"
			>
			  {generatingQR ? 'Generating...' : '+ Generate QR'}
			</button>
		  </div>

		  {qrCodes.length === 0 ? (
			<div className="p-12 text-center text-gray-400">
			  <div className="text-4xl mb-3">📱</div>
			  <p>No QR codes yet</p>
			</div>
		  ) : (
			<div className="divide-y">
			  {qrCodes.map((qr) => (
				<div key={qr.id} className={`p-4 sm:px-6 flex items-center gap-4 ${!qr.is_active ? 'opacity-50' : ''}`}>
				  <img
					src={`/api/qr-codes/${diskId}/image?qrId=${qr.id}&size=80`}
					alt="QR"
					className="w-16 h-16 bg-white border rounded-lg p-0.5 flex-shrink-0"
					onError={(e) => { (e.target as HTMLImageElement).src = 'data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" width="64" height="64"><text x="50%" y="50%" text-anchor="middle" dy=".3em" font-size="10" fill="%23999">QR</text></svg>' }}
				  />
				  <div className="flex-1 min-w-0">
					<div className="flex items-center gap-2">
					  <p className="text-sm font-medium text-gray-900 truncate">{qr.qr_code_url}</p>
					  {!qr.is_active && <span className="text-xs text-red-500">Inactive</span>}
					</div>
					<p className="text-xs text-gray-500">
					  Created {new Date(qr.created_at).toLocaleDateString()} &middot; {qr.download_count} downloads
					  {qr.expires_at && ` · Expires ${new Date(qr.expires_at).toLocaleDateString()}`}
					</p>
				  </div>
				  <div className="flex items-center gap-2 flex-shrink-0">
					<a
					  href={qr.qr_code_url}
					  target="_blank"
					  rel="noopener noreferrer"
					  className="px-3 py-1.5 text-xs bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition"
					>
					  Open
					</a>
					{qr.is_active && (
					  <button
						onClick={() => handleDeactivateQR(qr.id)}
						className="px-3 py-1.5 text-xs bg-red-50 text-red-600 border border-red-200 rounded-lg hover:bg-red-100 transition"
					  >
						Deactivate
					  </button>
					)}
				  </div>
				</div>
			  ))}
			</div>
		  )}
		</div>

		{/* Google Drive Links Section */}
		<div className="bg-white rounded-xl border">
		  <div className="p-6 border-b">
			<h2 className="text-lg font-bold text-gray-900">Google Drive Links</h2>
			<p className="text-sm text-gray-500">Files uploaded from the desktop app</p>
		  </div>

		  {gdriveLinks.length === 0 ? (
			<div className="p-12 text-center text-gray-400">
			  <div className="text-4xl mb-3">☁️</div>
			  <p>No Google Drive links yet</p>
			  <p className="text-sm mt-1">Upload a snapshot from the desktop app to see links here</p>
			</div>
		  ) : (
			<div className="divide-y">
			  {gdriveLinks.map((link) => (
				<div key={link.id} className="p-4 sm:px-6 flex items-center gap-4">
				  <div className="w-10 h-10 bg-green-50 rounded-lg flex items-center justify-center flex-shrink-0">
					<span className="text-green-600 text-lg">📄</span>
				  </div>
				  <div className="flex-1 min-w-0">
					<p className="text-sm font-medium text-gray-900 truncate">{link.google_file_name || link.html_file_name || 'Unnamed file'}</p>
					<p className="text-xs text-gray-500">Uploaded {new Date(link.uploaded_at).toLocaleDateString()}</p>
				  </div>
				  <a
					href={link.google_drive_url}
					target="_blank"
					rel="noopener noreferrer"
					className="px-4 py-2 text-sm bg-green-50 text-green-700 border border-green-200 rounded-lg hover:bg-green-100 transition flex-shrink-0 font-medium"
				  >
					Open in Drive
				  </a>
				</div>
			  ))}
			</div>
		  )}
		</div>
	  </div>
	</div>
  )
}
