import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { SignInForm } from "@/components/auth/sign-in-form";

export default async function SignInPage({
  searchParams,
}: {
  searchParams: Promise<{ next?: string; signed_up?: string }>;
}) {
  const params = await searchParams;
  const next = params.next && params.next.startsWith("/") ? params.next : "/account";

  return (
    <div className="mx-auto flex w-full max-w-sm flex-1 flex-col justify-center py-8">
      <Card>
        <CardHeader>
          <CardTitle>เข้าสู่ระบบ</CardTitle>
          <CardDescription>เข้าสู่ระบบด้วยอีเมลและรหัสผ่านของคุณ</CardDescription>
        </CardHeader>
        <CardContent className="flex flex-col gap-4">
          {params.signed_up && (
            <Alert>
              <AlertDescription>
                สร้างบัญชีสำเร็จ ระบบได้ส่งรหัสผ่านไปยังอีเมลของคุณแล้ว กรุณาตรวจสอบอีเมลเพื่อเข้าสู่ระบบ
              </AlertDescription>
            </Alert>
          )}
          <SignInForm next={next} />
        </CardContent>
      </Card>
    </div>
  );
}
