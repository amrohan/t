#!/bin/bash
# install.sh
#
# A robust installer for Termix that handles PATH configuration for Bash and Zsh.
# Usage:
#   Install/Update: curl -fsSL https://raw.githubusercontent.com/amrohan/t/main/install.sh | bash
#   Uninstall:      curl -fsSL https://raw.githubusercontent.com/amrohan/t/main/install.sh | bash -s uninstall

# --- Configuration ---
REPO="amrohan/t"
INSTALL_DIR="$HOME/.local/bin"
EXE_NAME="termix"
# ---

# Stop on any error
set -e

# --- Utility Functions ---
# A function to detect the user's default shell profile file.
# This is more reliable than checking version variables inside a script.
detect_profile() {
	local shell_path
	shell_path=$(echo "$SHELL" | awk -F'/' '{print $NF}')

	if [ "$shell_path" = "zsh" ]; then
		[ -f "$HOME/.zshrc" ] && echo "$HOME/.zshrc"
	elif [ "$shell_path" = "bash" ]; then
		# For Bash, we check in a specific order. .bashrc is common on Linux.
		# .bash_profile is common on macOS for login shells.
		if [ -f "$HOME/.bashrc" ]; then
			echo "$HOME/.bashrc"
		elif [ -f "$HOME/.bash_profile" ]; then
			echo "$HOME/.bash_profile"
		fi
	fi
}

# --- Main Functions ---
setup_path() {
	local profile_file
	profile_file=$(detect_profile)

	local path_line="export PATH=\"\$PATH:$INSTALL_DIR\""
	local comment="# Added by Termix installer"

	if [ -z "$profile_file" ]; then
		echo ""
		echo "Could not find a standard profile file (.zshrc or .bashrc)."
		echo "Please add the following directory to your PATH manually:"
		echo "  $INSTALL_DIR"
		return
	fi

	# Prevent adding duplicate entries
	if grep -q "PATH=.*$INSTALL_DIR" "$profile_file"; then
		echo "✅ Termix directory is already in your PATH."
		return
	fi

	echo ""
	read -p "May we add the Termix directory to your PATH in '$profile_file'? (y/n) " -n 1 -r
	echo "" # Move to a new line
	if [[ $REPLY =~ ^[Yy]$ ]]; then
		echo "" >>"$profile_file"
		echo "$comment" >>"$profile_file"
		echo "$path_line" >>"$profile_file"
		echo ""
		echo "✅ PATH was configured. Please restart your terminal or run 'source $profile_file' to use '$EXE_NAME'."
	else
		echo "Skipping PATH configuration. You will need to add '$INSTALL_DIR' to your PATH manually."
	fi
}

install_termix() {
	if ! command -v jq &>/dev/null; then
		echo "Error: 'jq' is not installed, but it's required. Please install it first (e.g., on macOS: 'brew install jq')." >&2
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

	echo "Downloading latest version..."
	TEMP_DIR=$(mktemp -d)
	curl -L -s -o "$TEMP_DIR/$ASSET_SUFFIX" "$DOWNLOAD_URL"

	mkdir -p "$INSTALL_DIR"

	if [[ "$ASSET_SUFFIX" == *.zip ]]; then unzip -q -o "$TEMP_DIR/$ASSET_SUFFIX" -d "$INSTALL_DIR"; else tar -xzf "$TEMP_DIR/$ASSET_SUFFIX" -C "$INSTALL_DIR"; fi

	rm -r "$TEMP_DIR"
	EXE_PATH="$INSTALL_DIR/$EXE_NAME"
	chmod +x "$EXE_PATH"

	if [ "$PLATFORM" == "osx" ]; then xattr -d com.apple.quarantine "$EXE_PATH" 2>/dev/null || true; fi

	echo "✅ Termix was installed successfully to $EXE_PATH"

	case ":$PATH:" in
	*":$INSTALL_DIR:"*)
		echo "Directory is already in your PATH. Run '$EXE_NAME' to start."
		;;
	*)
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
	else
		echo "Termix is not found at $EXE_PATH. Nothing to do."
	fi

	# Attempt to clean up the user's shell profile
	local profile_file
	profile_file=$(detect_profile)
	if [ -n "$profile_file" ] && grep -q "# Added by Termix installer" "$profile_file"; then
		echo ""
		read -p "Found a Termix entry in '$profile_file'. May we remove it? (y/n) " -n 1 -r
		echo ""
		if [[ $REPLY =~ ^[Yy]$ ]]; then
			# Use sed to remove the comment and the line after it, creating a backup first.
			sed -i.bak -e '/# Added by Termix installer/,+1d' "$profile_file"
			echo "✅ Removed PATH entry from $profile_file. A backup was created at ${profile_file}.bak."
		fi
	fi
}

# --- Main Logic ---
if [ "$1" == "uninstall" ]; then
	uninstall_termix
else
	install_termix
fi
