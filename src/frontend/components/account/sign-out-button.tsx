import { signOutAction } from "@/lib/actions/account-actions";
import { Button } from "@/components/ui/button";

export function SignOutButton() {
  return (
    <form action={signOutAction}>
      <Button type="submit" variant="outline">
        ออกจากระบบ
      </Button>
    </form>
  );
}
