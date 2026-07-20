'use client'

import { useEffect, useState, useRef } from 'react'
import { useRouter } from 'next/navigation'
import axios from 'axios'

interface UserProfile {
  id: number; username: string; email: string; firstName: string; lastName: string
  membershipId: number; membershipName: string; maxDisks: number; diskCount: number
}

interface Disk {
  id: number; name: string; description: string; disk_type: string; disk_path: string
  image_url: string | null; created_at: string
}

interface DiskStats {
  qrCount: number; gdriveCount: number; totalDownloads: number
  gdriveLinks: { id: number; google_file_name: string; google_drive_url: string; uploaded_at: string }[]
  latestQr: { id: number; qr_code_url: string; created_at: string; download_count: number } | null
}

interface UnsplashPhoto {
  id: string; thumb: string; small: string; regular: string; full: string
  alt: string; author: string; authorUrl: string
}

export default function DashboardPage() {
  const [user, setUser] = useState<UserProfile | null>(null)
  const [disks, setDisks] = useState<Disk[]>([])
  const [diskStats, setDiskStats] = useState<Record<number, DiskStats>>({})
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [showAddDisk, setShowAddDisk] = useState(false)
  const [newDisk, setNewDisk] = useState({ name: '', description: '', diskType: 'local', diskPath: '' })
  const [newDiskImageUrl, setNewDiskImageUrl] = useState('')
  const [newDiskImagePreview, setNewDiskImagePreview] = useState<string | null>(null)
  const [addingDisk, setAddingDisk] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState<number | null>(null)
  const [deleting, setDeleting] = useState<number | null>(null)
  const [expandedDisk, setExpandedDisk] = useState<number | null>(null)
  const [editingDisk, setEditingDisk] = useState<number | null>(null)
  const [editForm, setEditForm] = useState({ name: '', description: '', imageUrl: '' })
  const [savingEdit, setSavingEdit] = useState(false)
  const [showUnsplashSearch, setShowUnsplashSearch] = useState<'add' | 'edit' | null>(null)
  const [unsplashQuery, setUnsplashQuery] = useState('')
  const [unsplashResults, setUnsplashResults] = useState<UnsplashPhoto[]>([])
  const [unsplashLoading, setUnsplashLoading] = useState(false)
  const [unsplashPage, setUnsplashPage] = useState(1)
  const router = useRouter()

  const token = typeof window !== 'undefined' ? localStorage.getItem('authToken') : null
  const authHeader = token ? { Authorization: `Bearer ${token}` } : {}

  useEffect(() => {
	if (!token) { router.push('/login'); return }
	loadDashboard()
  }, [])

  const loadDashboard = async () => {
	try {
	  const [profileRes, disksRes] = await Promise.all([
		axios.get('/api/user/profile', { headers: authHeader }),
		axios.get('/api/disks', { headers: authHeader }),
	  ])
	  setUser(profileRes.data.user)
	  setDisks(disksRes.data.disks || [])

	  const stats: Record<number, DiskStats> = {}
	  for (const disk of (disksRes.data.disks || [])) {
		try {
		  const statsRes = await axios.get(`/api/disks/${disk.id}/stats`, { headers: authHeader })
		  stats[disk.id] = statsRes.data
		} catch { stats[disk.id] = { qrCount: 0, gdriveCount: 0, totalDownloads: 0, gdriveLinks: [], latestQr: null } }
	  }
	  setDiskStats(stats)
	} catch (err: any) {
	  if (err.response?.status === 401) { router.push('/login'); return }
	  setError(err.response?.data?.message || 'Error loading dashboard')
	} finally { setLoading(false) }
  }

  const searchUnsplash = async (query: string, page: number = 1) => {
	if (!query.trim()) return
	setUnsplashLoading(true)
	try {
	  const res = await axios.get(`/api/unsplash/search?q=${encodeURIComponent(query)}&page=${page}&per_page=12`)
	  if (page === 1) setUnsplashResults(res.data.results || [])
	  else setUnsplashResults(prev => [...prev, ...(res.data.results || [])])
	  setUnsplashPage(page)
	} catch { } finally { setUnsplashLoading(false) }
  }

  const handleAddDisk = async () => {
	if (!newDisk.name.trim()) return
	setAddingDisk(true)
	try {
	  await axios.post('/api/disks', {
		name: newDisk.name, description: newDisk.description,
		diskType: newDisk.diskType, diskPath: newDisk.diskPath,
		imageUrl: newDiskImageUrl || undefined,
	  }, { headers: authHeader })
	  setNewDisk({ name: '', description: '', diskType: 'local', diskPath: '' })
	  setNewDiskImageUrl(''); setNewDiskImagePreview(null)
	  setShowAddDisk(false)
	  loadDashboard()
	} catch (err: any) {
	  setError(err.response?.data?.message || 'Failed to add disk')
	} finally { setAddingDisk(false) }
  }

  const handleDeleteDisk = async (diskId: number) => {
	setDeleting(diskId)
	try {
	  await axios.delete(`/api/disks/${diskId}`, { headers: authHeader })
	  setDeleteConfirm(null); loadDashboard()
	} catch (err: any) {
	  setError(err.response?.data?.message || 'Failed to delete disk')
	} finally { setDeleting(null) }
  }

  const handleSaveEdit = async (diskId: number) => {
	if (!editForm.name.trim()) return
	setSavingEdit(true)
	try {
	  const body: any = { name: editForm.name, description: editForm.description }
	  if (editForm.imageUrl) body.imageUrl = editForm.imageUrl
	  else body.removeImage = true
	  await axios.put(`/api/disks/${diskId}`, body, { headers: authHeader })
	  setEditingDisk(null)
	  loadDashboard()
	} catch (err: any) {
	  setError(err.response?.data?.message || 'Failed to update disk')
	} finally { setSavingEdit(false) }
  }

  const startEditing = (disk: Disk) => {
	setEditingDisk(disk.id)
	setEditForm({ name: disk.name, description: disk.description || '', imageUrl: disk.image_url || '' })
  }

  const handleLogout = () => {
	localStorage.removeItem('authToken')
	localStorage.removeItem('user')
	router.push('/')
  }

  const handleRegenerateQR = async (diskId: number) => {
	try {
	  await axios.post(`/api/qr-codes/${diskId}`, {}, { headers: authHeader })
	  loadDashboard()
	} catch (err: any) {
	  setError(err.response?.data?.message || 'Failed to generate QR code')
	}
  }

  const selectUnsplashImage = (photo: UnsplashPhoto, mode: 'add' | 'edit') => {
	if (mode === 'add') {
	  setNewDiskImageUrl(photo.regular)
	  setNewDiskImagePreview(photo.thumb)
	} else {
	  setEditForm(prev => ({ ...prev, imageUrl: photo.regular }))
	}
	setShowUnsplashSearch(null)
	setUnsplashResults([])
	setUnsplashQuery('')
  }

  if (loading) {
	return <div className="flex items-center justify-center min-h-screen bg-gray-50"><div className="text-gray-500 text-lg">Loading dashboard...</div></div>
  }

  const atDiskLimit = user && user.diskCount >= user.maxDisks

  return (
	<div className="min-h-screen bg-gray-50">
	  {/* Header */}
	  <div className="bg-white shadow-sm border-b">
		<div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
		  <div className="flex justify-between items-center py-4">
			<div>
			  <h1 className="text-2xl font-bold text-gray-900">TagMyDrive</h1>
			  <p className="text-sm text-gray-500">Welcome back, {user?.firstName || user?.username}</p>
			</div>
			<div className="flex items-center gap-3">
			  <button onClick={() => router.push('/membership')} className="px-4 py-2 text-sm font-medium text-blue-600 hover:text-blue-800 border border-blue-300 rounded-lg hover:bg-blue-50 transition">
				{user?.membershipName || 'Free'} Plan
			  </button>
			  <button onClick={handleLogout} className="px-4 py-2 text-sm font-medium text-gray-600 hover:text-gray-800 border border-gray-300 rounded-lg hover:bg-gray-50 transition">
				Logout
			  </button>
			</div>
		  </div>
		</div>
	  </div>

	  <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
		{error && (
		  <div className="mb-6 p-4 bg-red-50 text-red-700 rounded-lg border border-red-200 flex justify-between items-center">
			<span>{error}</span>
			<button onClick={() => setError('')} className="text-red-500 hover:text-red-700 font-bold">&times;</button>
		  </div>
		)}

		{/* Stats Bar */}
		<div className="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-8">
		  <div className="bg-white rounded-lg shadow-sm border p-4">
			<div className="text-sm text-gray-500">Disks</div>
			<div className="text-2xl font-bold text-gray-900">{user?.diskCount || 0} <span className="text-sm font-normal text-gray-400">/ {user?.maxDisks || 2}</span></div>
			<div className="mt-2 w-full bg-gray-200 rounded-full h-1.5">
			  <div className="bg-blue-600 h-1.5 rounded-full transition-all" style={{ width: `${Math.min(((user?.diskCount || 0) / (user?.maxDisks || 2)) * 100, 100)}%` }} />
			</div>
		  </div>
		  <div className="bg-white rounded-lg shadow-sm border p-4">
			<div className="text-sm text-gray-500">Plan</div>
			<div className="text-2xl font-bold text-gray-900">{user?.membershipName || 'Free'}</div>
			{atDiskLimit && <button onClick={() => router.push('/membership')} className="mt-2 text-sm text-blue-600 hover:underline font-medium">Upgrade to add more disks</button>}
		  </div>
		  <div className="bg-white rounded-lg shadow-sm border p-4">
			<div className="text-sm text-gray-500">Total QR Downloads</div>
			<div className="text-2xl font-bold text-gray-900">{Object.values(diskStats).reduce((sum, s) => sum + (s.totalDownloads || 0), 0)}</div>
		  </div>
		</div>

		{/* Disks Section */}
		<div className="bg-white rounded-lg shadow-sm border">
		  <div className="p-6 border-b flex justify-between items-center">
			<h2 className="text-lg font-bold text-gray-900">My Disks</h2>
			<button onClick={() => setShowAddDisk(!showAddDisk)} disabled={!!atDiskLimit && !showAddDisk}
			  className="px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition disabled:opacity-50 disabled:cursor-not-allowed">
			  {showAddDisk ? 'Cancel' : '+ Add Disk'}
			</button>
		  </div>

		  {/* Add Disk Form */}
		  {showAddDisk && (
			<div className="p-6 border-b bg-gray-50">
			  <div className="flex gap-6">
				{/* Image preview */}
				<div className="flex-shrink-0">
				  <div className="w-28 h-28 border-2 border-dashed border-gray-300 rounded-lg flex flex-col items-center justify-center overflow-hidden bg-white">
					{newDiskImagePreview ? (
					  <img src={newDiskImagePreview} alt="" className="w-full h-full object-cover" />
					) : (
					  <>
						<svg className="w-8 h-8 text-gray-400 mb-1" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" /></svg>
						<span className="text-xs text-gray-400">Disk photo</span>
					  </>
					)}
				  </div>
				  {newDiskImagePreview && (
					<button onClick={() => { setNewDiskImageUrl(''); setNewDiskImagePreview(null) }}
					  className="mt-1 text-xs text-red-500 hover:underline">Remove</button>
				  )}
				</div>
				{/* Fields */}
				<div className="flex-1 grid grid-cols-1 md:grid-cols-2 gap-4">
				  <div>
					<label className="block text-sm font-medium text-gray-700 mb-1">Disk Name *</label>
					<input value={newDisk.name} onChange={(e) => setNewDisk({ ...newDisk, name: e.target.value })}
					  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500" placeholder="My External HDD" />
				  </div>
				  <div>
					<label className="block text-sm font-medium text-gray-700 mb-1">Disk Type</label>
					<select value={newDisk.diskType} onChange={(e) => setNewDisk({ ...newDisk, diskType: e.target.value })}
					  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500">
					  <option value="local">Local</option>
					  <option value="external">External</option>
					  <option value="network">Network</option>
					</select>
				  </div>
				  <div className="md:col-span-2">
					<label className="block text-sm font-medium text-gray-700 mb-1">Description</label>
					<input value={newDisk.description} onChange={(e) => setNewDisk({ ...newDisk, description: e.target.value })}
					  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500" placeholder="Optional description" />
				  </div>
				  <div className="md:col-span-2">
					<label className="block text-sm font-medium text-gray-700 mb-1">Disk Path</label>
					<input value={newDisk.diskPath} onChange={(e) => setNewDisk({ ...newDisk, diskPath: e.target.value })}
					  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500" placeholder="C:\Users\..." />
				  </div>
				  <div className="md:col-span-2">
					<label className="block text-sm font-medium text-gray-700 mb-1">Image URL</label>
					<div className="flex gap-2">
					  <input value={newDiskImageUrl} onChange={(e) => { setNewDiskImageUrl(e.target.value); setNewDiskImagePreview(e.target.value || null) }}
						className="flex-1 px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500" placeholder="https://example.com/image.jpg" />
					  <button type="button" onClick={() => { setShowUnsplashSearch('add'); setUnsplashQuery(''); searchUnsplash('hard drive') }}
						className="px-3 py-2 bg-gray-100 text-gray-700 text-sm rounded-lg hover:bg-gray-200 transition whitespace-nowrap">Search Images</button>
					</div>
				  </div>
				</div>
			  </div>
			  <div className="mt-4 flex justify-end">
				<button onClick={handleAddDisk} disabled={addingDisk || !newDisk.name.trim()}
				  className="px-5 py-2 bg-green-600 text-white text-sm font-medium rounded-lg hover:bg-green-700 transition disabled:opacity-50">
				  {addingDisk ? 'Saving...' : 'Save Disk'}
				</button>
			  </div>
			</div>
		  )}

		  {/* Disk List */}
		  {disks.length === 0 ? (
			<div className="p-12 text-center text-gray-400">
			  <div className="text-4xl mb-3">📂</div>
			  <p className="text-lg font-medium">No disks yet</p>
			  <p className="text-sm mt-1">Add your first disk to start tracking</p>
			</div>
		  ) : (
			<div className="divide-y">
			  {disks.map((disk) => {
				const stats = diskStats[disk.id]
				const isExpanded = expandedDisk === disk.id
				const isEditing = editingDisk === disk.id
				return (
				  <div key={disk.id} className="hover:bg-gray-50 transition">
					{/* Disk Row */}
					<div className="p-4 sm:px-6 flex items-center gap-4">
					  {/* Image / Avatar */}
					  <div className="w-14 h-14 bg-gray-100 rounded-lg flex items-center justify-center flex-shrink-0 overflow-hidden cursor-pointer border"
						onClick={() => !isEditing && setExpandedDisk(isExpanded ? null : disk.id)}>
						{disk.image_url ? (
						  <img src={disk.image_url} alt={disk.name} className="w-full h-full object-cover"
							onError={(e) => { (e.target as HTMLImageElement).style.display = 'none' }} />
						) : (
						  <span className="text-2xl">💾</span>
						)}
					  </div>

					  <div className="flex-1 min-w-0 cursor-pointer" onClick={() => !isEditing && setExpandedDisk(isExpanded ? null : disk.id)}>
						{isEditing ? (
						  <div className="flex items-center gap-2" onClick={(e) => e.stopPropagation()}>
							<input value={editForm.name} onChange={(e) => setEditForm({ ...editForm, name: e.target.value })}
							  className="px-2 py-1 border border-blue-300 rounded text-sm font-semibold focus:ring-2 focus:ring-blue-500 w-48" />
							<input value={editForm.description} onChange={(e) => setEditForm({ ...editForm, description: e.target.value })}
							  className="px-2 py-1 border border-gray-300 rounded text-sm text-gray-600 focus:ring-2 focus:ring-blue-500 flex-1" placeholder="Description" />
						  </div>
						) : (
						  <>
							<div className="flex items-center gap-2">
							  <h3 className="font-semibold text-gray-900 truncate">{disk.name}</h3>
							  <span className="px-2 py-0.5 bg-gray-100 text-gray-600 text-xs rounded-full capitalize">{disk.disk_type}</span>
							  <button onClick={(e) => { e.stopPropagation(); startEditing(disk) }}
								className="text-gray-400 hover:text-blue-600 transition" title="Edit disk">
								<svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" /></svg>
							  </button>
							</div>
							<div className="flex items-center gap-4 text-sm text-gray-500 mt-0.5">
							  {disk.description && <span className="truncate max-w-[200px]">{disk.description}</span>}
							  <span>{new Date(disk.created_at).toLocaleDateString()}</span>
							</div>
						  </>
						)}
					  </div>

					  {isEditing ? (
						<div className="flex items-center gap-2 flex-shrink-0" onClick={(e) => e.stopPropagation()}>
						  <div>
							<input value={editForm.imageUrl} onChange={(e) => setEditForm({ ...editForm, imageUrl: e.target.value })}
							  className="px-2 py-1 border border-gray-300 rounded text-xs w-48" placeholder="Image URL" />
						  </div>
						  <button onClick={() => setShowUnsplashSearch('edit')}
							className="px-2 py-1 text-xs bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition">Search</button>
						  {editForm.imageUrl && (
							<button onClick={() => setEditForm(prev => ({ ...prev, imageUrl: '' }))}
							  className="px-2 py-1.5 text-xs text-red-500 hover:text-red-700">Remove</button>
						  )}
						  <button onClick={() => handleSaveEdit(disk.id)} disabled={savingEdit || !editForm.name.trim()}
							className="px-3 py-1.5 text-xs bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition disabled:opacity-50">
							{savingEdit ? '...' : 'Save'}
						  </button>
						  <button onClick={() => { setEditingDisk(null) }}
							className="px-3 py-1.5 text-xs text-gray-500 hover:text-gray-700">Cancel</button>
						</div>
					  ) : (
						<div className="hidden sm:flex items-center gap-4 text-sm text-gray-500 flex-shrink-0">
						  {stats && (
							<>
							  <div className="text-center"><div className="font-semibold text-gray-900">{stats.qrCount}</div><div className="text-xs">QR</div></div>
							  <div className="text-center"><div className="font-semibold text-gray-900">{stats.gdriveCount}</div><div className="text-xs">Links</div></div>
							  <div className="text-center"><div className="font-semibold text-gray-900">{stats.totalDownloads}</div><div className="text-xs">Downloads</div></div>
							</>
						  )}
						</div>
					  )}

					  {!isEditing && (
						<svg className={`w-5 h-5 text-gray-400 transition-transform cursor-pointer ${isExpanded ? 'rotate-180' : ''}`}
						  fill="none" viewBox="0 0 24 24" stroke="currentColor"
						  onClick={() => setExpandedDisk(isExpanded ? null : disk.id)}>
						  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
						</svg>
					  )}
					</div>

					{/* Expanded Details */}
					{isExpanded && stats && !isEditing && (
					  <div className="px-6 pb-5 bg-gray-50 border-t">
						<div className="grid grid-cols-1 lg:grid-cols-2 gap-6 pt-4">
						  {/* QR Code */}
						  <div>
							<h4 className="text-sm font-semibold text-gray-700 mb-3">QR Code</h4>
							{stats.latestQr ? (
							  <div className="flex items-start gap-4">
								<img src={`/api/qr-codes/${disk.id}/image?size=160`} alt="QR Code" className="w-32 h-32 bg-white border rounded-lg p-1" />
								<div className="text-sm">
								  <p className="text-gray-600 break-all text-xs">URL: <a href={stats.latestQr.qr_code_url} target="_blank" rel="noopener noreferrer" className="text-blue-600 hover:underline">{stats.latestQr.qr_code_url}</a></p>
								  <p className="text-gray-500 mt-1">Downloads: {stats.latestQr.download_count}</p>
								  <button onClick={(e) => { e.stopPropagation(); handleRegenerateQR(disk.id) }}
									className="mt-2 px-3 py-1.5 text-xs bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition">Regenerate QR</button>
								</div>
							  </div>
							) : (
							  <div className="text-sm text-gray-500 bg-white border rounded-lg p-4">
								<p>No QR code yet</p>
								<button onClick={(e) => { e.stopPropagation(); handleRegenerateQR(disk.id) }}
								  className="mt-2 px-3 py-1.5 text-xs bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition">Generate QR Code</button>
							  </div>
							)}
						  </div>
						  {/* Google Drive Links */}
						  <div>
							<h4 className="text-sm font-semibold text-gray-700 mb-3">Google Drive Links</h4>
							{stats.gdriveLinks.length > 0 ? (
							  <div className="space-y-2">
								{stats.gdriveLinks.map((link) => (
								  <div key={link.id} className="flex items-center justify-between bg-white border rounded-lg p-3">
									<div className="min-w-0 flex-1">
									  <p className="text-sm font-medium text-gray-900 truncate">{link.google_file_name || 'Unnamed file'}</p>
									  <p className="text-xs text-gray-500">{new Date(link.uploaded_at).toLocaleDateString()}</p>
									</div>
									<a href={link.google_drive_url} target="_blank" rel="noopener noreferrer"
									  className="ml-3 px-3 py-1 text-xs bg-green-50 text-green-700 border border-green-200 rounded-lg hover:bg-green-100 transition flex-shrink-0">Open</a>
								  </div>
								))}
							  </div>
							) : (
							  <div className="text-sm text-gray-500 bg-white border rounded-lg p-4">
								<p>No Google Drive links yet</p>
								<p className="text-xs mt-1 text-gray-400">Upload a snapshot from the desktop app</p>
							  </div>
							)}
						  </div>
						</div>
						<div className="flex gap-3 mt-5 pt-4 border-t">
						  {disk.disk_path && (
							<button onClick={(e) => { e.stopPropagation(); navigator.clipboard.writeText(disk.disk_path) }}
							  className="px-3 py-1.5 text-xs bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition">Copy Path</button>
						  )}
						  <button onClick={(e) => { e.stopPropagation(); setDeleteConfirm(disk.id) }}
							className="px-3 py-1.5 text-xs bg-red-50 text-red-600 border border-red-200 rounded-lg hover:bg-red-100 transition">Delete Disk</button>
						</div>
					  </div>
					)}
				  </div>
				)
			  })}
			</div>
		  )}
		</div>
	  </div>

	  {/* Unsplash Search Modal */}
	  {showUnsplashSearch && (
		<div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
		  <div className="bg-white rounded-lg shadow-xl w-full max-w-2xl mx-4 max-h-[80vh] flex flex-col">
			<div className="p-4 border-b flex items-center justify-between">
			  <h3 className="font-bold text-gray-900">Search Images (Unsplash)</h3>
			  <button onClick={() => { setShowUnsplashSearch(null); setUnsplashResults([]); setUnsplashQuery('') }}
				className="text-gray-400 hover:text-gray-600">&times;</button>
			</div>
			<div className="p-4 border-b">
			  <div className="flex gap-2">
				<input value={unsplashQuery} onChange={(e) => setUnsplashQuery(e.target.value)}
				  onKeyDown={(e) => { if (e.key === 'Enter') searchUnsplash(unsplashQuery) }}
				  className="flex-1 px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-blue-500" placeholder="Search for disk images..." autoFocus />
				<button onClick={() => searchUnsplash(unsplashQuery)} disabled={unsplashLoading}
				  className="px-4 py-2 bg-blue-600 text-white text-sm rounded-lg hover:bg-blue-700 transition disabled:opacity-50">
				  {unsplashLoading ? '...' : 'Search'}
				</button>
			  </div>
			</div>
			<div className="p-4 overflow-y-auto flex-1">
			  {unsplashResults.length > 0 ? (
				<>
				  <div className="grid grid-cols-3 gap-3">
					{unsplashResults.map((photo) => (
					  <div key={photo.id} className="cursor-pointer rounded-lg overflow-hidden border hover:border-blue-500 hover:shadow-md transition"
						onClick={() => selectUnsplashImage(photo, showUnsplashSearch)}>
						<img src={photo.thumb} alt={photo.alt} className="w-full h-24 object-cover" />
						<div className="p-1.5 text-xs text-gray-500 truncate">by {photo.author}</div>
					  </div>
					))}
				  </div>
				  <button onClick={() => searchUnsplash(unsplashQuery, unsplashPage + 1)} disabled={unsplashLoading}
					className="mt-3 w-full py-2 text-sm text-blue-600 hover:bg-blue-50 rounded-lg transition">
					{unsplashLoading ? 'Loading...' : 'Load more'}
				  </button>
				</>
			  ) : (
				<p className="text-sm text-gray-400 text-center py-8">Search for disk, HDD, SSD, or any image</p>
			  )}
			</div>
		  </div>
		</div>
	  )}

	  {/* Delete Confirmation Modal */}
	  {deleteConfirm && (
		<div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
		  <div className="bg-white rounded-lg shadow-xl p-6 w-full max-w-sm mx-4">
			<h3 className="text-lg font-bold text-gray-900 mb-2">Delete Disk?</h3>
			<p className="text-sm text-gray-600 mb-6">This will deactivate the disk. QR codes and Google Drive links will be preserved but hidden.</p>
			<div className="flex gap-3 justify-end">
			  <button onClick={() => setDeleteConfirm(null)} className="px-4 py-2 text-sm border border-gray-300 rounded-lg hover:bg-gray-50 transition">Cancel</button>
			  <button onClick={() => handleDeleteDisk(deleteConfirm)} disabled={deleting === deleteConfirm}
				className="px-4 py-2 text-sm bg-red-600 text-white rounded-lg hover:bg-red-700 transition disabled:opacity-50">
				{deleting === deleteConfirm ? 'Deleting...' : 'Delete'}
			  </button>
			</div>
		  </div>
		</div>
	  )}
	</div>
  )
}
