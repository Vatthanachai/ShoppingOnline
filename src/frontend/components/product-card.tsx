import Link from "next/link";

import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { formatCurrency } from "@/lib/format";
import type { Product } from "@/lib/types";

export function ProductCard({ product }: { product: Product }) {
  return (
    <Link href={`/products/${product.product_id}`} className="block h-full">
      <Card className="h-full transition-shadow hover:shadow-md">
        <CardHeader>
          <Badge variant="secondary" className="w-fit">
            {product.product_category_name}
          </Badge>
          <CardTitle className="line-clamp-2">{product.product_name}</CardTitle>
        </CardHeader>
        <CardContent className="flex flex-1 flex-col gap-1 text-sm text-muted-foreground">
          <p className="line-clamp-2">{product.description}</p>
          <p>ผู้ขาย: {product.vendor_name}</p>
        </CardContent>
        <CardFooter className="font-medium">{formatCurrency(product.min_price)}</CardFooter>
      </Card>
    </Link>
  );
}
