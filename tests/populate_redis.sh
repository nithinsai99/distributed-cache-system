#!/usr/bin/env bash
# Script to populate Redis with dummy data via POST requests
# Usage: bash tests/populate_redis.sh

API_URL="http://localhost:5001/api/cache"

echo "Populating Redis with dummy data..."
echo ""

# Store user data
echo "Creating user cache entries..."
curl -s -X POST "$API_URL/user:1" \
  -H "Content-Type: application/json" \
  -d '{"id":1,"name":"Alice","email":"alice@example.com","status":"active"}' > /dev/null
echo "✓ Stored user:1"

curl -s -X POST "$API_URL/user:2" \
  -H "Content-Type: application/json" \
  -d '{"id":2,"name":"Bob","email":"bob@example.com","status":"active"}' > /dev/null
echo "✓ Stored user:2"

curl -s -X POST "$API_URL/user:3" \
  -H "Content-Type: application/json" \
  -d '{"id":3,"name":"Charlie","email":"charlie@example.com","status":"inactive"}' > /dev/null
echo "✓ Stored user:3"

# Store product data
echo ""
echo "Creating product cache entries..."
curl -s -X POST "$API_URL/product:101" \
  -H "Content-Type: application/json" \
  -d '{"id":101,"name":"Laptop","price":999.99,"stock":15}' > /dev/null
echo "✓ Stored product:101"

curl -s -X POST "$API_URL/product:102" \
  -H "Content-Type: application/json" \
  -d '{"id":102,"name":"Mouse","price":29.99,"stock":150}' > /dev/null
echo "✓ Stored product:102"

curl -s -X POST "$API_URL/product:103" \
  -H "Content-Type: application/json" \
  -d '{"id":103,"name":"Keyboard","price":89.99,"stock":75}' > /dev/null
echo "✓ Stored product:103"

# Store session data
echo ""
echo "Creating session cache entries..."
curl -s -X POST "$API_URL/session:sess_abc123" \
  -H "Content-Type: application/json" \
  -d '{"sessionId":"sess_abc123","userId":1,"loginTime":"2026-05-03T22:35:00Z","lastActivity":"2026-05-03T22:45:30Z"}' > /dev/null
echo "✓ Stored session:sess_abc123"

curl -s -X POST "$API_URL/session:sess_def456" \
  -H "Content-Type: application/json" \
  -d '{"sessionId":"sess_def456","userId":2,"loginTime":"2026-05-03T22:30:00Z","lastActivity":"2026-05-03T22:46:15Z"}' > /dev/null
echo "✓ Stored session:sess_def456"

# Store simple string data
echo ""
echo "Creating simple cache entries..."
curl -s -X POST "$API_URL/config:theme" \
  -H "Content-Type: application/json" \
  -d '"dark"' > /dev/null
echo "✓ Stored config:theme"

curl -s -X POST "$API_URL/stats:visits:today" \
  -H "Content-Type: application/json" \
  -d '{"count":1547,"timestamp":"2026-05-03T22:50:00Z"}' > /dev/null
echo "✓ Stored stats:visits:today"

echo ""
echo "✅ All dummy data populated!"
echo ""
echo "Next steps:"
echo "1. Open Redis Commander: http://localhost:8081"
echo "2. View all stored keys and their values"
echo "3. Try GET requests to retrieve the data:"
echo ""
echo "   curl http://localhost:5001/api/cache/user:1"
echo "   curl http://localhost:5001/api/cache/product:101"
echo "   curl http://localhost:5001/api/cache/session:sess_abc123"
echo ""

exit 0
