import type { ReactNode } from "react";

import { redirect } from "next/navigation";

import { AdminNav } from "@/components/admin/admin-nav";
import { getCurrentProfile } from "@/lib/session";

export default async function AdminLayout({ children }: { children: ReactNode }) {
  const profile = await getCurrentProfile();

  if (!profile || profile.role !== "Admin") {
    redirect("/");
  }

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-2xl font-semibold">ระบบจัดการหลังบ้าน</h1>
      <AdminNav />
      {children}
    </div>
  );
}
