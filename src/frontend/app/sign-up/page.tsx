import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { SignUpForm } from "@/components/auth/sign-up-form";

export default function SignUpPage() {
  return (
    <div className="mx-auto flex w-full max-w-sm flex-1 flex-col justify-center py-8">
      <Card>
        <CardHeader>
          <CardTitle>สมัครสมาชิก</CardTitle>
          <CardDescription>
            กรอกข้อมูลเพื่อสร้างบัญชีใหม่ ระบบจะส่งรหัสผ่านไปยังอีเมลของคุณ
          </CardDescription>
        </CardHeader>
        <CardContent>
          <SignUpForm />
        </CardContent>
      </Card>
    </div>
  );
}
