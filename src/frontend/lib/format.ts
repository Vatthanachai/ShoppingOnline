const currencyFormatter = new Intl.NumberFormat("th-TH", {
  style: "currency",
  currency: "THB",
});

const dateFormatter = new Intl.DateTimeFormat("th-TH", {
  dateStyle: "medium",
  timeStyle: "short",
});

export function formatCurrency(amount: number | null | undefined): string {
  if (amount === null || amount === undefined) return "-";
  return currencyFormatter.format(amount);
}

export function formatDate(value: string): string {
  return dateFormatter.format(new Date(value));
}

const STATUS_LABEL_TH: Record<string, string> = {
  Pending: "รอดำเนินการ",
  Confirmed: "ยืนยันแล้ว",
  Shipped: "จัดส่งแล้ว",
  Delivered: "ส่งถึงแล้ว",
  Cancelled: "ยกเลิกแล้ว",
};

export function formatOrderStatus(status: string): string {
  return STATUS_LABEL_TH[status] ?? status;
}
