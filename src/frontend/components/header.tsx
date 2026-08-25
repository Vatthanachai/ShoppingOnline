"use client";

import Link from "next/link";
import { ShoppingCart, User } from "lucide-react";

import { Button } from "@/components/ui/button";
import { useCart } from "@/components/cart-provider";
import type { Profile } from "@/lib/types";

export function Header({ profile }: { profile: Profile | null }) {
  const { totalItems } = useCart();

  return (
    <header className="border-b bg-background sticky top-0 z-10">
      <div className="mx-auto flex max-w-5xl items-center justify-between gap-4 px-4 py-3">
        <Link href="/" className="text-lg font-semibold">
          ShoppingOnline
        </Link>

        <nav className="flex items-center gap-2 text-sm">
          <Button variant="ghost" nativeButton={false} render={<Link href="/products" />}>
            สินค้า
          </Button>

          <Button
            variant="ghost"
            className="relative"
            nativeButton={false}
            render={<Link href="/cart" />}
          >
            <ShoppingCart className="size-4" />
            ตะกร้า
            {totalItems > 0 && (
              <span className="absolute -right-1 -top-1 flex size-5 items-center justify-center rounded-full bg-primary text-[11px] text-primary-foreground">
                {totalItems}
              </span>
            )}
          </Button>

          {profile ? (
            <>
              <Button variant="ghost" nativeButton={false} render={<Link href="/orders" />}>
                คำสั่งซื้อ
              </Button>
              <Button variant="ghost" nativeButton={false} render={<Link href="/account" />}>
                <User className="size-4" />
                {profile.name}
              </Button>
            </>
          ) : (
            <Button variant="default" nativeButton={false} render={<Link href="/sign-in" />}>
              เข้าสู่ระบบ
            </Button>
          )}
        </nav>
      </div>
    </header>
  );
}
