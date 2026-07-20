'use client'

import { useState } from 'react'
import { useRouter } from 'next/navigation'
import axios from 'axios'
import Link from 'next/link'

export default function RegisterPage() {
  const [formData, setFormData] = useState({
	username: '',
	email: '',
	firstName: '',
	lastName: '',
	password: '',
	confirmPassword: '',
  })
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')
  const router = useRouter()

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
	setFormData({
	  ...formData,
	  [e.target.name]: e.target.value,
	})
  }

  const handleRegister = async (e: React.FormEvent) => {
	e.preventDefault()
	setLoading(true)
	setError('')

	if (formData.password !== formData.confirmPassword) {
	  setError('Passwords do not match')
	  setLoading(false)
	  return
	}

	try {
	  const response = await axios.post('/api/auth/register', {
		username: formData.username,
		email: formData.email,
		firstName: formData.firstName,
		lastName: formData.lastName,
		password: formData.password,
	  })

	  if (response.data.userId) {
		router.push('/login?registered=true')
	  }
	} catch (err: any) {
	  setError(err.response?.data?.message || 'Registration failed')
	} finally {
	  setLoading(false)
	}
  }

  return (
	<div className="min-h-screen bg-gradient-to-br from-blue-600 to-blue-800 flex items-center justify-center py-8">
	  <div className="bg-white rounded-lg shadow-xl p-8 w-full max-w-md">
		<h1 className="text-3xl font-bold text-center mb-6 text-gray-800">TagMyDrive</h1>
		<h2 className="text-xl font-semibold text-center mb-6 text-gray-600">Create Account</h2>

		{error && (
		  <div className="mb-4 p-3 bg-red-100 text-red-700 rounded">
			{error}
		  </div>
		)}

		<form onSubmit={handleRegister} className="space-y-3">
		  <div>
			<label className="block text-gray-700 font-semibold mb-1">Username</label>
			<input
			  type="text"
			  name="username"
			  value={formData.username}
			  onChange={handleChange}
			  placeholder="Choose a username"
			  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
			  required
			/>
		  </div>

		  <div>
			<label className="block text-gray-700 font-semibold mb-1">Email</label>
			<input
			  type="email"
			  name="email"
			  value={formData.email}
			  onChange={handleChange}
			  placeholder="Enter your email"
			  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
			  required
			/>
		  </div>

		  <div className="grid grid-cols-2 gap-2">
			<div>
			  <label className="block text-gray-700 font-semibold mb-1">First Name</label>
			  <input
				type="text"
				name="firstName"
				value={formData.firstName}
				onChange={handleChange}
				placeholder="First name"
				className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
			  />
			</div>
			<div>
			  <label className="block text-gray-700 font-semibold mb-1">Last Name</label>
			  <input
				type="text"
				name="lastName"
				value={formData.lastName}
				onChange={handleChange}
				placeholder="Last name"
				className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
			  />
			</div>
		  </div>

		  <div>
			<label className="block text-gray-700 font-semibold mb-1">Password</label>
			<input
			  type="password"
			  name="password"
			  value={formData.password}
			  onChange={handleChange}
			  placeholder="Enter password (min 6 chars)"
			  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
			  required
			/>
		  </div>

		  <div>
			<label className="block text-gray-700 font-semibold mb-1">Confirm Password</label>
			<input
			  type="password"
			  name="confirmPassword"
			  value={formData.confirmPassword}
			  onChange={handleChange}
			  placeholder="Confirm password"
			  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
			  required
			/>
		  </div>

		  <button
			type="submit"
			disabled={loading}
			className="w-full py-2 bg-blue-600 text-white font-bold rounded-lg hover:bg-blue-700 transition disabled:opacity-50 mt-4"
		  >
			{loading ? 'Creating account...' : 'Register'}
		  </button>
		</form>

		<p className="text-center mt-6 text-gray-600">
		  Already have an account?{' '}
		  <Link href="/login" className="text-blue-600 font-bold hover:underline">
			Login here
		  </Link>
		</p>
	  </div>
	</div>
  )
}
