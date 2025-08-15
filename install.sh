#!/usr/bin/env bash
#
# A professional-grade installer for Termix.
#
# Usage:
#   Install/Update: curl -fsSL https://raw.githubusercontent.com/amrohan/t/main/install.sh | bash
#   Install specific version: curl -fsSL https://raw.githubusercontent.com/amrohan/t/main/install.sh | bash -s v0.0.6
#   Uninstall:      curl -fsSL https://raw.githubusercontent.com/amrohan/t/main/install.sh | bash -s uninstall

set -euo pipefail

# --- Configuration ---
REPO="amrohan/t"
EXE_NAME="termix"
INSTALL_DIR="$HOME/.local/bin"
# ---

# --- Color Definitions ---
if [[ -t 1 ]]; then
	Color_Off='\033[0m'
	Red='\033[0;31m'
	Green='\033[0;32m'
	Dim='\033[0;2m'
	Bold_Green='\033[1;32m'
	Bold_White='\033[1m'
else
	Color_Off=''
	Red=''
	Green=''
	Dim=''
	Bold_Green=''
	Bold_White=''
fi

# --- Helper Functions ---
error() {
	echo -e "${Red}error${Color_Off}:" "$@" >&2
	exit 1
}
info() { echo -e "${Dim}$@ ${Color_Off}"; }
info_bold() { echo -e "${Bold_White}$@ ${Color_Off}"; }
success() { echo -e "${Green}$@ ${Color_Off}"; }
tildify() { if [[ $1 = "$HOME"* ]]; then
	local r=\~/
	echo "${1/$HOME\//$r}"
else echo "$1"; fi; }

# --- Main Functions ---
install_termix() {
	info "Starting Termix installation..."

	command -v curl >/dev/null || error "'curl' is required."
	command -v unzip >/dev/null || error "'unzip' is required."
	command -v jq >/dev/null || error "'jq' is required. Please install it first (e.g., 'brew install jq')."

	local platform
	platform=$(uname -ms)
	case "$platform" in
	'Darwin x86_64') local target=osx-x64 ;;
	'Darwin arm64') local target=osx-arm64 ;; # <-- THE FIX IS HERE
	'Linux aarch64' | 'Linux arm64') local target=linux-aarch64 ;;
	'Linux x86_64') local target=linux-x64 ;;
	*) error "Unsupported platform: $platform. Please open an issue at https://github.com/$REPO/issues" ;;
	esac

	if [[ $target = osx-x64 ]] && [[ $(sysctl -n sysctl.proc_translated 2>/dev/null) = 1 ]]; then
		target=osx-arm64
		info "Rosetta 2 detected. Downloading Termix for ARM64 instead."
	fi

	local tag="${1:-latest}"
	local api_url="https://api.github.com/repos/$REPO/releases"
	local release_url="$api_url/$tag"
	if [[ $tag = "latest" ]]; then release_url="$api_url/latest"; else release_url="$api_url/tags/$tag"; fi

	if [[ $target == "linux"* ]]; then local asset_suffix="$target.tar.gz"; else local asset_suffix="$target.zip"; fi

	info "Fetching release information for tag: $tag"
	local download_url
	download_url=$(curl -s "$release_url" | jq -r ".assets[] | select(.name | endswith(\"termix-$asset_suffix\")) | .browser_download_url")

	if [ -z "$download_url" ]; then error "Could not find a release asset for your system (termix-$asset_suffix) with tag '$tag'."; fi

	mkdir -p "$INSTALL_DIR" || error "Failed to create install directory \"$INSTALL_DIR\""
	local temp_dir
	temp_dir=$(mktemp -d)
	local archive_path="$temp_dir/termix-$asset_suffix"
	local exe_path="$INSTALL_DIR/$EXE_NAME"

	info_bold "Downloading from $download_url"
	curl --fail --location --progress-bar --output "$archive_path" "$download_url" || error "Failed to download Termix."

	info "Extracting archive..."
	if [[ $asset_suffix == *.zip ]]; then unzip -oqd "$temp_dir" "$archive_path"; else tar -xzf "$archive_path" -C "$temp_dir"; fi

	mv "$temp_dir/publish/$EXE_NAME" "$exe_path" || error 'Failed to move executable.'
	chmod +x "$exe_path" || error 'Failed to set permissions.'

	if [[ $target == "osx"* ]]; then xattr -d com.apple.quarantine "$exe_path" 2>/dev/null || true; fi

	rm -r "$temp_dir"
	success "Termix was installed successfully to $Bold_Green$(tildify "$exe_path")"

	local profile_file
	profile_file=$(detect_profile)
	if [[ ":$PATH:" != *":$INSTALL_DIR:"* ]]; then
		if [ -n "$profile_file" ] && [ -w "$profile_file" ]; then
			if ! grep -q "# Added by Termix installer" "$profile_file"; then
				info "Adding '$INSTALL_DIR' to \$PATH in '$(tildify "$profile_file")'"
				echo "" >>"$profile_file"
				echo "# Added by Termix installer" >>"$profile_file"
				echo "export PATH=\"\$PATH:$INSTALL_DIR\"" >>"$profile_file"
				info "To get started, restart your terminal or run:"
				info_bold "  source $(tildify "$profile_file")"
			fi
		else
			echo ""
			info "Manually add the directory to your shell's PATH:"
			info_bold "  export PATH=\"\$PATH:$(tildify "$INSTALL_DIR")\""
		fi
	fi

	echo ""
	info "Run '$EXE_NAME' to get started!"
}

uninstall_termix() {
	info "Starting Termix uninstallation..."
	local exe_path="$INSTALL_DIR/$EXE_NAME"
	if [ -f "$exe_path" ]; then
		rm -f "$exe_path"
		success "Termix has been uninstalled from $(tildify "$exe_path")"
	else info "Termix not found. Nothing to do."; fi

	local profile_file
	profile_file=$(detect_profile)
	if [ -n "$profile_file" ] && grep -q "# Added by Termix installer" "$profile_file"; then
		echo ""
		read -p "Found a Termix entry in '$(tildify "$profile_file")'. May we remove it? (y/n) " -n 1 -r
		echo ""
		if [[ $REPLY =~ ^[Yy]$ ]]; then
			sed -i.bak -e '/# Added by Termix installer/,+1d' "$profile_file"
			success "Removed PATH entry. A backup was created at $(tildify "${profile_file}.bak")."
		fi
	fi
}

detect_profile() {
	local shell_name
	shell_name=$(basename "$SHELL")
	if [ "$shell_name" = "zsh" ]; then [ -f "$HOME/.zshrc" ] && echo "$HOME/.zshrc"; elif [ "$shell_name" = "bash" ]; then if [ -f "$HOME/.bashrc" ]; then echo "$HOME/.bashrc"; elif [ -f "$HOME/.bash_profile" ]; then echo "$HOME/.bash_profile"; fi; elif [ "$shell_name" = "fish" ]; then [ -f "$HOME/.config/fish/config.fish" ] && echo "$HOME/.config/fish/config.fish"; fi
}

# --- Main Logic ---
if [ "${1:-}" = "uninstall" ]; then uninstall_termix; else install_termix "${1:-}"; fi
