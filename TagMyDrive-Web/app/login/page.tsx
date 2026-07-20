'use client'

import { useState, useEffect, Suspense } from 'react'
import { useRouter, useSearchParams } from 'next/navigation'
import axios from 'axios'
import Link from 'next/link'

function LoginForm() {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')
  const router = useRouter()
  const searchParams = useSearchParams()

  useEffect(() => {
	if (searchParams.get('registered') === 'true') {
	  setSuccess('Account created successfully! Please log in.')
	}
  }, [searchParams])

  const handleLogin = async (e: React.FormEvent) => {
	e.preventDefault()
	setLoading(true)
	setError('')

	try {
	  const response = await axios.post('/api/auth/login', {
		username,
		password,
	  })

	  if (response.data.token) {
		localStorage.setItem('authToken', response.data.token)
		localStorage.setItem('user', JSON.stringify(response.data.user))
		router.push('/dashboard')
	  }
	} catch (err: any) {
	  setError(err.response?.data?.message || 'Login failed')
	} finally {
	  setLoading(false)
	}
  }

  return (
	<div className="min-h-screen bg-gradient-to-br from-blue-600 to-blue-800 flex items-center justify-center">
	  <div className="bg-white rounded-lg shadow-xl p-8 w-full max-w-md">
		<h1 className="text-3xl font-bold text-center mb-6 text-gray-800">TagMyDrive</h1>
		<h2 className="text-xl font-semibold text-center mb-6 text-gray-600">Login</h2>

		{error && (
		  <div className="mb-4 p-3 bg-red-100 text-red-700 rounded">
			{error}
		  </div>
		)}

		{success && (
		  <div className="mb-4 p-3 bg-green-100 text-green-700 rounded">
			{success}
		  </div>
		)}

		<form onSubmit={handleLogin} className="space-y-4">
		  <div>
			<label className="block text-gray-700 font-semibold mb-2">Username or Email</label>
			<input
			  type="text"
			  value={username}
			  onChange={(e) => setUsername(e.target.value)}
			  placeholder="Enter your username or email"
			  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
			  required
			/>
		  </div>

		  <div>
			<label className="block text-gray-700 font-semibold mb-2">Password</label>
			<input
			  type="password"
			  value={password}
			  onChange={(e) => setPassword(e.target.value)}
			  placeholder="Enter your password"
			  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
			  required
			/>
		  </div>

		  <button
			type="submit"
			disabled={loading}
			className="w-full py-2 bg-blue-600 text-white font-bold rounded-lg hover:bg-blue-700 transition disabled:opacity-50"
		  >
			{loading ? 'Logging in...' : 'Login'}
		  </button>
		</form>

		<p className="text-center mt-6 text-gray-600">
		  Don't have an account?{' '}
		  <Link href="/register" className="text-blue-600 font-bold hover:underline">
			Register here
		  </Link>
		</p>
	  </div>
	</div>
  )
}

export default function LoginPage() {
  return (
	<Suspense fallback={<div className="flex items-center justify-center min-h-screen bg-gray-50"><div className="text-gray-500">Loading...</div></div>}>
	  <LoginForm />
	</Suspense>
  )
}
