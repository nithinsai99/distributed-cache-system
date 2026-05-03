#!/usr/bin/env bash
# Simple script to issue many requests quickly and show how many 429 responses are returned.
# Usage: bash tests/hit_rate_limit.sh <URL> <COUNT>

URL=${1:-http://localhost:5001/api/cache/testkey}
COUNT=${2:-200}

if ! command -v curl >/dev/null 2>&1; then
  echo "curl is required"
  exit 1
fi

echo "Sending $COUNT requests to $URL"

TMPDIR=$(mktemp -d)
OUTFILE="$TMPDIR/statuses.txt"
rm -f "$OUTFILE"

for i in $(seq 1 "$COUNT"); do
  # fire requests in background to increase concurrency
  curl -s -o /dev/null -w "%{http_code}\n" "$URL" >> "$OUTFILE" &
done

wait

echo "Requests complete. Summary:" 
sort "$OUTFILE" | uniq -c | sort -nr

# Count 429 occurrences
429COUNT=$(grep -c "^429$" "$OUTFILE" || true)
echo "429 responses: $429COUNT / $COUNT"

rm -rf "$TMPDIR"

exit 0
