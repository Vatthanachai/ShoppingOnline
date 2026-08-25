import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { ChangePasswordForm } from "@/components/auth/change-password-form";
import { mustChangePassword } from "@/lib/session";

export default async function ChangePasswordPage() {
  const forced = await mustChangePassword();

  return (
    <div className="mx-auto flex w-full max-w-sm flex-1 flex-col justify-center py-8">
      <Card>
        <CardHeader>
          <CardTitle>เปลี่ยนรหัสผ่าน</CardTitle>
          <CardDescription>กรอกรหัสผ่านปัจจุบันและรหัสผ่านใหม่ของคุณ</CardDescription>
        </CardHeader>
        <CardContent>
          <ChangePasswordForm forced={forced} />
        </CardContent>
      </Card>
    </div>
  );
}
