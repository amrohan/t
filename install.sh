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

	# Determine the asset pattern (now handles both zip and tar.gz)
	if [ "$PLATFORM" == "linux" ]; then
		ASSET_PATTERN="${PLATFORM}-${ARCH}.tar.gz"
	else
		ASSET_PATTERN="${PLATFORM}-${ARCH}.zip"
	fi

	DOWNLOAD_URL=$(curl -s "https://api.github.com/repos/$REPO/releases/latest" | grep "browser_download_url.*${ASSET_PATTERN}" | cut -d '"' -f 4)

	if [ -z "$DOWNLOAD_URL" ]; then
		echo "Error: Could not find a release asset for your system ($ASSET_PATTERN)."
		exit 1
	fi

	echo "Downloading from $DOWNLOAD_URL"

	# Create a temporary directory to handle archives safely
	TEMP_DIR=$(mktemp -d)
	TEMP_FILE="$TEMP_DIR/$ASSET_PATTERN"

	curl -L -o "$TEMP_FILE" "$DOWNLOAD_URL"

	mkdir -p "$INSTALL_DIR"

	# Extract based on file type
	if [[ "$ASSET_PATTERN" == *.zip ]]; then
		unzip -o "$TEMP_FILE" -d "$INSTALL_DIR"
	else
		tar -xzf "$TEMP_FILE" -C "$INSTALL_DIR"
	fi

	rm -r "$TEMP_DIR"

	EXE_PATH="$INSTALL_DIR/$EXE_NAME"
	chmod +x "$EXE_PATH"

	# Remove quarantine attribute on macOS to bypass Gatekeeper warning
	if [ "$PLATFORM" == "osx" ]; then
		echo "Removing quarantine attribute on macOS to prevent security warning..."
		xattr -d com.apple.quarantine "$EXE_PATH" 2>/dev/null || echo "Could not remove quarantine attribute. Manual approval may be needed."
	fi

	echo ""
	echo "✅ Termix was installed successfully to $EXE_PATH"

	# Check if INSTALL_DIR is in PATH
	case ":$PATH:" in
	*":$INSTALL_DIR:"*)
		echo "Run 'termix' to start."
		;;
	*)
		echo ""
		echo "IMPORTANT: To complete the installation, add the following directory to your PATH:"
		echo "  $INSTALL_DIR"
		echo "You can do this by adding the following line to your shell profile (~/.bashrc, ~/.zshrc, etc.):"
		echo "  export PATH=\"\$PATH:$INSTALL_DIR\""
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
		echo "Please note: If you added '$INSTALL_DIR' to your shell's PATH, you will need to remove it manually."
	else
		echo "Termix is not found at $EXE_PATH. Nothing to do."
	fi
}

# --- Main Logic ---

# Check the first argument passed to the script.
# 'bash -s uninstall' passes 'uninstall' as $1.
if [ "$1" == "uninstall" ]; then
	uninstall_termix
else
	install_termix
fi
