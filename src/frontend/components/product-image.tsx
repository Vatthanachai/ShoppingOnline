"use client";

import { useEffect, useRef, useState } from "react";
import { ImageOff } from "lucide-react";

import { cn } from "@/lib/utils";

type ProductImageProps = {
  src: string | null | undefined;
  alt: string;
  className?: string;
};

/**
 * Renders a product's image, falling back to a placeholder icon when there's no
 * image_path or the given URL fails to load (e.g. the seeded demo products, whose
 * image_path values don't point to any real file yet).
 */
export function ProductImage({ src, alt, className }: ProductImageProps) {
  const [failed, setFailed] = useState(false);
  const imgRef = useRef<HTMLImageElement>(null);

  useEffect(() => {
    // The <img> starts loading from the server-rendered HTML before React hydrates and
    // attaches onError below, so a failure that happens that early fires an event nobody's
    // listening to yet. Catch that case once we're mounted: complete + naturalWidth 0 means
    // it already failed.
    if (imgRef.current?.complete && imgRef.current.naturalWidth === 0) {
      setFailed(true);
    }
  }, [src]);

  if (!src || failed) {
    return (
      <div
        className={cn(
          "flex items-center justify-center bg-muted text-muted-foreground",
          className,
        )}
      >
        <ImageOff className="size-8" />
      </div>
    );
  }

  return (
    // eslint-disable-next-line @next/next/no-img-element -- image_path can be any absolute/relative URL an admin typed in, not known ahead of time for next/image's domain allowlist.
    <img
      ref={imgRef}
      src={src}
      alt={alt}
      onError={() => setFailed(true)}
      className={cn("object-cover", className)}
    />
  );
}
