import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";

import { CartProvider } from "@/components/cart-provider";
import { Header } from "@/components/header";
import { getCurrentProfile } from "@/lib/session";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "ShoppingOnline",
  description: "ร้านค้าออนไลน์ ShoppingOnline",
};

export default async function RootLayout({ children }: LayoutProps<"/">) {
  const profile = await getCurrentProfile();

  return (
    <html
      lang="th"
      className={`${geistSans.variable} ${geistMono.variable} h-full antialiased`}
    >
      <body className="min-h-full flex flex-col" suppressHydrationWarning>
        <CartProvider>
          <Header profile={profile} />
          <main className="mx-auto flex w-full max-w-5xl flex-1 flex-col px-4 py-6">{children}</main>
        </CartProvider>
      </body>
    </html>
  );
}
