#!/usr/bin/env bash
# Run on Ubuntu server after: sudo docker load -i sersalud019.tar
set -euo pipefail

TAG="${TAG:-sersalud:19.00}"
CONTAINER="${CONTAINER:-sersalud}"
PORT="${PORT:-80}"
TAR_FILE="${1:-sersalud019.tar}"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TAR_PATH="$SCRIPT_DIR/$TAR_FILE"

if [[ -f "$TAR_PATH" ]]; then
  echo "Loading image from $TAR_PATH ..."
  sudo docker load -i "$TAR_PATH"
fi

if ! sudo docker image inspect "$TAG" >/dev/null 2>&1; then
  echo "ERROR: image $TAG not found. Run: sudo docker load -i sersalud019.tar" >&2
  exit 1
fi

sudo docker rm -f "$CONTAINER" 2>/dev/null || true

echo "Starting $CONTAINER on port $PORT ..."
sudo docker run -d \
  --name "$CONTAINER" \
  -p "${PORT}:80" \
  --restart unless-stopped \
  "$TAG"

echo ""
echo "OK. Check: sudo docker ps"
echo "Logs:    sudo docker logs -f $CONTAINER"
echo "URL:     http://$(curl -s http://169.254.169.254/latest/meta-data/public-ipv4 2>/dev/null || hostname -I | awk '{print $1}'):${PORT}/"
