import type { Metadata } from 'next'
import { Titillium_Web } from 'next/font/google'
import './globals.css'

const tittlium = Titillium_Web({
  weight: ["200", "400", "600", "700"],
  subsets: ['latin-ext']
});

export const metadata: Metadata = {
  title: 'Carsties',
  description: 'Generated by create next app',
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <html lang="en">
      <body className={tittlium.className}>{children}</body>
    </html>
  )
}