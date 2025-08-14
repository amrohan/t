#!/bin/bash
# install.sh
#
# Usage:
#   Install/Update: curl -fsSL https://raw.githubusercontent.com/amrohan/t/main/install.sh | bash
#   Uninstall:      curl -fsSL https://raw.githubusercontent.com/amrohan/t/main/install.sh | bash -s uninstall

# --- Configuration ---
REPO="amrohan/t"
INSTALL_DIR="$HOME/.local/bin"
EXE_NAME="termix"
# ---------------------

set -e

# --- Functions ---

install_termix() {
	# Check for jq, the JSON parser
	if ! command -v jq &>/dev/null; then
		echo "Error: 'jq' is not installed, but it's required for this script." >&2
		echo "Please install it first." >&2
		echo "On macOS: brew install jq" >&2
		echo "On Debian/Ubuntu: sudo apt-get install jq" >&2
		exit 1
	fi

	echo "Starting Termix installation..."

	# Detect OS and architecture
	OS="$(uname -s)"
	ARCH="$(uname -m)"

	case $OS in
	Linux) PLATFORM="linux" ;;
	Darwin) PLATFORM="osx" ;;
	*)
		echo "Error: Unsupported OS '$OS'."
		exit 1
		;;
	esac

	case $ARCH in
	x86_64) ARCH="x64" ;;
	arm64 | aarch64) ARCH="arm64" ;;
	*)
		echo "Error: Unsupported architecture '$ARCH'."
		exit 1
		;;
	esac

	# Determine the asset suffix (e.g., osx-arm64.zip)
	if [ "$PLATFORM" == "linux" ]; then
		ASSET_SUFFIX="${PLATFORM}-${ARCH}.tar.gz"
	else
		ASSET_SUFFIX="${PLATFORM}-${ARCH}.zip"
	fi

	# Use jq to reliably parse the JSON and get the download URL
	API_URL="https://api.github.com/repos/$REPO/releases/latest"
	DOWNLOAD_URL=$(curl -s "$API_URL" | jq -r ".assets[] | select(.name | endswith(\"$ASSET_SUFFIX\")) | .browser_download_url")

	if [ -z "$DOWNLOAD_URL" ]; then
		echo "Error: Could not find a release asset for your system ($ASSET_SUFFIX)." >&2
		exit 1
	fi

	echo "Downloading from $DOWNLOAD_URL"

	TEMP_DIR=$(mktemp -d)
	TEMP_FILE="$TEMP_DIR/$ASSET_SUFFIX"
	curl -L -o "$TEMP_FILE" "$DOWNLOAD_URL"

	mkdir -p "$INSTALL_DIR"

	if [[ "$ASSET_SUFFIX" == *.zip ]]; then
		unzip -o "$TEMP_FILE" -d "$INSTALL_DIR"
	else
		tar -xzf "$TEMP_FILE" -C "$INSTALL_DIR"
	fi

	rm -r "$TEMP_DIR"
	EXE_PATH="$INSTALL_DIR/$EXE_NAME"
	chmod +x "$EXE_PATH"

	if [ "$PLATFORM" == "osx" ]; then
		echo "Removing quarantine attribute on macOS to prevent security warning..."
		xattr -d com.apple.quarantine "$EXE_PATH" 2>/dev/null || true
	fi

	echo ""
	echo "✅ Termix was installed successfully to $EXE_PATH"

	case ":$PATH:" in
	*":$INSTALL_DIR:"*)
		echo "Run 'termix' to start."
		;;
	*)
		echo ""
		echo "IMPORTANT: To complete the installation, add the following directory to your PATH:"
		echo "  $INSTALL_DIR"
		echo "Then restart your terminal."
		;;
	esac
}

uninstall_termix() {
	echo "Starting Termix uninstallation..."
	EXE_PATH="$INSTALL_DIR/$EXE_NAME"
	if [ -f "$EXE_PATH" ]; then
		rm -f "$EXE_PATH"
		echo "✅ Termix has been uninstalled from $EXE_PATH"
	else
		echo "Termix is not found at $EXE_PATH. Nothing to do."
	fi
}

# --- Main Logic ---
if [ "$1" == "uninstall" ]; then
	uninstall_termix
else
	install_termix
fi
