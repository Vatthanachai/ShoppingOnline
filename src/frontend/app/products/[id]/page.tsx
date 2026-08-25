import { notFound } from "next/navigation";

import { Badge } from "@/components/ui/badge";
import { AddToCartForm } from "@/components/add-to-cart-form";
import { ApiError } from "@/lib/api-client";
import { getApiClient } from "@/lib/session";

export default async function ProductDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  const productId = Number(id);
  const client = await getApiClient();

  let product;
  try {
    product = await client.getProduct(productId);
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) {
      notFound();
    }
    throw error;
  }

  const { data: stocks } = await client.getStocksPage({ product_id: productId, page_limit: 100 });

  return (
    <div className="flex flex-col gap-6">
      <div className="flex flex-col gap-2">
        <Badge variant="secondary" className="w-fit">
          {product.product_category_name}
        </Badge>
        <h1 className="text-2xl font-semibold">{product.product_name}</h1>
        <p className="text-sm text-muted-foreground">รหัสสินค้า: {product.product_code}</p>
        {product.description && <p className="text-muted-foreground">{product.description}</p>}
      </div>

      <div className="rounded-xl border p-4">
        <h2 className="mb-4 text-lg font-semibold">เลือกซื้อจากผู้ขาย</h2>
        <AddToCartForm product={product} stocks={stocks} />
      </div>
    </div>
  );
}
