import Link from "next/link";

import { Button } from "@/components/ui/button";
import { CheckoutForm } from "@/components/checkout-form";
import { withAuthRedirect } from "@/lib/session";

export default async function CheckoutPage() {
  const { data: addresses } = await withAuthRedirect((client) =>
    client.getAddressesPage({ page_index: 1, page_limit: 100 }),
  );

  if (addresses.length === 0) {
    return (
      <div className="flex flex-col items-center gap-4 py-16 text-center">
        <h1 className="text-2xl font-semibold">ยังไม่มีที่อยู่จัดส่ง</h1>
        <p className="text-muted-foreground">กรุณาเพิ่มที่อยู่จัดส่งก่อนสั่งซื้อ</p>
        <Button nativeButton={false} render={<Link href="/account/addresses" />}>
          เพิ่มที่อยู่จัดส่ง
        </Button>
      </div>
    );
  }

  return <CheckoutForm addresses={addresses} />;
}
