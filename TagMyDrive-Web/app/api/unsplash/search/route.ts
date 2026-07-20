import { NextRequest, NextResponse } from 'next/server'

export async function GET(request: NextRequest) {
  try {
	const { searchParams } = new URL(request.url)
	const q = searchParams.get('q')
	const page = searchParams.get('page') || '1'
	const perPage = searchParams.get('per_page') || '12'

	if (!q) {
	  return NextResponse.json({ message: 'Search query is required' }, { status: 400 })
	}

	const accessKey = process.env.UNSPLASH_ACCESS_KEY

	if (!accessKey) {
	  return NextResponse.json({
		message: 'Unsplash API key not configured. Set UNSPLASH_ACCESS_KEY in .env.local.',
		results: [],
		total: 0,
	  }, { status: 200 })
	}

	const url = `https://api.unsplash.com/search/photos?query=${encodeURIComponent(q)}&page=${page}&per_page=${perPage}&orientation=landscape`

	const response = await fetch(url, {
	  headers: { Authorization: `Client-ID ${accessKey}` },
	})

	if (!response.ok) {
	  return NextResponse.json({ message: 'Unsplash API error' }, { status: response.status })
	}

	const data = await response.json()

	const results = data.results.map((photo: any) => ({
	  id: photo.id,
	  thumb: photo.urls.thumb,
	  small: photo.urls.small,
	  regular: photo.urls.regular,
	  full: photo.urls.full,
	  alt: photo.alt_description || photo.description || 'Disk image',
	  author: photo.user.name,
	  authorUrl: photo.user.links.html,
	}))

	return NextResponse.json({
	  results,
	  total: data.total,
	  totalPages: data.total_pages,
	}, { status: 200 })
  } catch (error: any) {
	return NextResponse.json({ message: 'Failed to search images' }, { status: 500 })
  }
}
