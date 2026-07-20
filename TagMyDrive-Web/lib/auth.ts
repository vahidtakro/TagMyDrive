import jwt, { JwtPayload } from 'jsonwebtoken'
import { NextRequest } from 'next/server'

export async function verifyToken(
  request: NextRequest
): Promise<{ userId: number } | null> {
  const authHeader = request.headers.get('authorization')
  if (!authHeader || !authHeader.startsWith('Bearer ')) {
    return null
  }

  const token = authHeader.substring(7)
  try {
    const decoded = jwt.verify(token, process.env.JWT_SECRET || 'secret')
    if (typeof decoded === 'object' && decoded && 'userId' in decoded) {
      return decoded as JwtPayload & { userId: number }
    }
    return null
  } catch {
    return null
  }
}

export function generateToken(userId: number) {
  return jwt.sign(
    { userId },
    process.env.JWT_SECRET || 'secret',
    { expiresIn: '7d' }
  )
}
