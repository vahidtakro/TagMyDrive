import type { Metadata } from 'next'
import './globals.css'

export const metadata: Metadata = {
  title: 'TagMyDrive - Disk Management & QR Code Generator',
  description: 'Manage your disks and generate QR codes with TagMyDrive',
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
	<html lang="en">
	  <body className="bg-gray-50">
		{children}
	  </body>
	</html>
  )
}
