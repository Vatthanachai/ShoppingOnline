import { Card, CardContent } from "@/components/ui/card";
import { AddressDialog } from "@/components/addresses/address-dialog";
import { DeleteAddressDialog } from "@/components/addresses/delete-address-dialog";
import { withAuthRedirect } from "@/lib/session";

export default async function AddressesPage() {
  const { data: addresses } = await withAuthRedirect((client) =>
    client.getAddressesPage({ page_index: 1, page_limit: 25 }),
  );

  return (
    <div className="mx-auto flex w-full max-w-2xl flex-1 flex-col gap-6 py-8">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">ที่อยู่จัดส่ง</h1>
        <AddressDialog mode="create" />
      </div>

      {addresses.length === 0 ? (
        <Card>
          <CardContent className="py-8 text-center text-sm text-muted-foreground">
            ยังไม่มีที่อยู่จัดส่ง กดปุ่ม &quot;เพิ่มที่อยู่ใหม่&quot; เพื่อเริ่มต้น
          </CardContent>
        </Card>
      ) : (
        <div className="flex flex-col gap-3">
          {addresses.map((address) => (
            <Card key={address.shipping_address_id}>
              <CardContent className="flex items-start justify-between gap-4">
                <div className="text-sm">
                  <p className="font-medium">{address.address_line1}</p>
                  {address.address_line2 && <p className="text-muted-foreground">{address.address_line2}</p>}
                  <p className="text-muted-foreground">
                    {[address.city, address.state, address.postal_code].filter(Boolean).join(" ")}
                  </p>
                  <p className="text-muted-foreground">{address.country}</p>
                </div>
                <div className="flex shrink-0 gap-2">
                  <AddressDialog mode="edit" address={address} />
                  <DeleteAddressDialog addressId={address.shipping_address_id} />
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}
