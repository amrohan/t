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

# Function to automatically handle PATH setup
setup_path() {
	PROFILE_FILE=""
	# Detect the user's shell profile file
	if [ -n "$BASH_VERSION" ]; then
		if [ -f "$HOME/.bashrc" ]; then
			PROFILE_FILE="$HOME/.bashrc"
		elif [ -f "$HOME/.bash_profile" ]; then
			PROFILE_FILE="$HOME/.bash_profile"
		fi
	elif [ -n "$ZSH_VERSION" ]; then
		if [ -f "$HOME/.zshrc" ]; then
			PROFILE_FILE="$HOME/.zshrc"
		fi
	fi

	# If a profile file is found, ask the user to modify it
	if [ -n "$PROFILE_FILE" ]; then
		echo ""
		# Use 'read -p' to prompt the user
		read -p "May we add the termix directory to your PATH in '$PROFILE_FILE'? (y/n) " -n 1 -r
		echo # Move to a new line
		if [[ $REPLY =~ ^[Yy]$ ]]; then
			# Append the export command to the profile file
			echo "" >>"$PROFILE_FILE"
			echo "# Added by Termix installer" >>"$PROFILE_FILE"
			echo "export PATH=\"\$PATH:$INSTALL_DIR\"" >>"$PROFILE_FILE"
			echo ""
			echo "✅ PATH was configured. Please restart your terminal or run 'source $PROFILE_FILE' to use 'termix'."
		else
			echo "Skipping PATH configuration. You will need to add '$INSTALL_DIR' to your PATH manually."
		fi
	else
		# Fallback message if no common profile file was found
		echo ""
		echo "IMPORTANT: To complete the installation, add the following directory to your PATH:"
		echo "  $INSTALL_DIR"
		echo "Then restart your terminal."
	fi
}

install_termix() {
	if ! command -v jq &>/dev/null; then
		echo "Error: 'jq' is not installed, but it's required. Please install it first (e.g., 'brew install jq')." >&2
		exit 1
	fi

	echo "Starting Termix installation..."

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

	if [ "$PLATFORM" == "linux" ]; then ASSET_SUFFIX="${PLATFORM}-${ARCH}.tar.gz"; else ASSET_SUFFIX="${PLATFORM}-${ARCH}.zip"; fi

	API_URL="https://api.github.com/repos/$REPO/releases/latest"
	DOWNLOAD_URL=$(curl -s "$API_URL" | jq -r ".assets[] | select(.name | endswith(\"$ASSET_SUFFIX\")) | .browser_download_url")

	if [ -z "$DOWNLOAD_URL" ]; then
		echo "Error: Could not find a release asset for your system ($ASSET_SUFFIX)." >&2
		exit 1
	fi

	echo "Downloading from $DOWNLOAD_URL"

	TEMP_DIR=$(mktemp -d)
	TEMP_FILE="$TEMP_DIR/$ASSET_SUFFIX"
	curl -L -s -o "$TEMP_FILE" "$DOWNLOAD_URL" # Use -s for silent download

	mkdir -p "$INSTALL_DIR"

	if [[ "$ASSET_SUFFIX" == *.zip ]]; then unzip -q -o "$TEMP_FILE" -d "$INSTALL_DIR"; else tar -xzf "$TEMP_FILE" -C "$INSTALL_DIR"; fi

	rm -r "$TEMP_DIR"
	EXE_PATH="$INSTALL_DIR/$EXE_NAME"
	chmod +x "$EXE_PATH"

	if [ "$PLATFORM" == "osx" ]; then xattr -d com.apple.quarantine "$EXE_PATH" 2>/dev/null || true; fi

	echo "✅ Termix was installed successfully to $EXE_PATH"

	# Check if INSTALL_DIR is in PATH and call the setup function if it isn't
	case ":$PATH:" in
	*":$INSTALL_DIR:"*)
		echo "Directory is already in your PATH. Run 'termix' to start."
		;;
	*)
		# Call the new function to handle PATH setup
		setup_path
		;;
	esac
}

uninstall_termix() {
	echo "Starting Termix uninstallation..."
	EXE_PATH="$INSTALL_DIR/$EXE_NAME"
	if [ -f "$EXE_PATH" ]; then
		rm -f "$EXE_PATH"
		echo "✅ Termix has been uninstalled from $EXE_PATH"
		echo "Note: If the installer modified your shell profile, you may want to remove the corresponding 'export PATH' line manually."
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
