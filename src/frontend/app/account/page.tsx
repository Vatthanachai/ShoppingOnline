import Link from "next/link";

import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import { ProfileForm } from "@/components/account/profile-form";
import { SignOutButton } from "@/components/account/sign-out-button";
import { DeactivateDialog } from "@/components/account/deactivate-dialog";
import { withAuthRedirect } from "@/lib/session";

export default async function AccountPage() {
  const profile = await withAuthRedirect((client) => client.getProfile());

  return (
    <div className="mx-auto flex w-full max-w-2xl flex-1 flex-col gap-6 py-8">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">บัญชีของฉัน</h1>
        <SignOutButton />
      </div>

      <Card>
        <CardHeader>
          <CardTitle>ข้อมูลบัญชี</CardTitle>
          <CardDescription>
            <span className="mr-2">{profile.email}</span>
            <Badge variant="secondary">{profile.role === "Admin" ? "ผู้ดูแลระบบ" : "ลูกค้า"}</Badge>
          </CardDescription>
        </CardHeader>
        <CardContent className="flex flex-col gap-4">
          <ProfileForm profile={profile} />

          <Separator />

          <div className="flex items-center justify-between">
            <div className="text-sm text-muted-foreground">ต้องการเปลี่ยนรหัสผ่านใช่หรือไม่?</div>
            <Button variant="outline" nativeButton={false} render={<Link href="/change-password" />}>
              เปลี่ยนรหัสผ่าน
            </Button>
          </div>

          <div className="flex items-center justify-between">
            <div className="text-sm text-muted-foreground">จัดการที่อยู่จัดส่งของคุณ</div>
            <Button variant="outline" nativeButton={false} render={<Link href="/account/addresses" />}>
              ที่อยู่จัดส่ง
            </Button>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="text-destructive">โซนอันตราย</CardTitle>
          <CardDescription>การปิดใช้งานบัญชีจะทำให้คุณไม่สามารถเข้าสู่ระบบได้อีก</CardDescription>
        </CardHeader>
        <CardContent>
          <DeactivateDialog />
        </CardContent>
      </Card>
    </div>
  );
}
