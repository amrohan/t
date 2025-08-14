# install.ps1
#
# Usage:
#   Install/Update: iex (iwr "https://raw.githubusercontent.com/amrohan/t/main/install.ps1")
#   Uninstall:      iex (iwr "https://raw.githubusercontent.com/amrohan/t/main/install.ps1") -Uninstall

# --- Parameters ---
param(
    [switch]$Uninstall
)

# --- Configuration ---
$RepoOwner = "amrohan"
$RepoName = "termix"
$ExeName = "termix.exe"
$InstallDir = "$env:LOCALAPPDATA\Programs\termix"
# ---------------------

function Install-Termix {
    Write-Host "Starting Termix installation..."
    
    # Get the latest release information
    $LatestReleaseUrl = "https://api.github.com/repos/$RepoOwner/$RepoName/releases/latest"
    try {
        $ReleaseInfo = Invoke-RestMethod -Uri $LatestReleaseUrl
    } catch {
        Write-Error "Failed to fetch latest release info. Please check your internet connection or the repository path."
        exit 1
    }

    # Detect Architecture and select asset
    $Arch = $env:PROCESSOR_ARCHITECTURE
    if ($Arch -eq "AMD64") { $AssetPattern = "win-x64.zip" }
    elseif ($Arch -eq "ARM64") { $AssetPattern = "win-arm64.zip" }
    else { Write-Error "Unsupported architecture: $Arch"; exit 1 }

    $Asset = $ReleaseInfo.assets | Where-Object { $_.name -like "*$AssetPattern" } | Select-Object -First 1
    if (-not $Asset) {
        Write-Error "Could not find a suitable release asset for your system ($AssetPattern)."
        exit 1
    }

    $DownloadUrl = $Asset.browser_download_url
    $DownloadPath = Join-Path $env:TEMP "termix.zip"

    Write-Host "Downloading from $DownloadUrl..."
    Invoke-WebRequest -Uri $DownloadUrl -OutFile $DownloadPath -UseBasicParsing

    # Create installation directory and extract
    if (-not (Test-Path $InstallDir)) {
        New-Item -ItemType Directory -Path $InstallDir | Out-Null
    }
    Write-Host "Installing to $InstallDir..."
    Expand-Archive -Path $DownloadPath -DestinationPath $InstallDir -Force

    # Add to user's PATH if not already present
    $UserPath = [System.Environment]::GetEnvironmentVariable("Path", "User")
    if ($UserPath -notlike "*$InstallDir*") {
        Write-Host "Adding installation directory to your PATH."
        $NewPath = "$UserPath;$InstallDir"
        [System.Environment]::SetEnvironmentVariable("Path", $NewPath, "User")
        $env:Path += ";$InstallDir" # Update for the current session
    }

    # Clean up
    Remove-Item $DownloadPath

    Write-Host ""
    Write-Host "✅ Termix was installed successfully!"
    Write-Host "Please restart your terminal or run 'refreshenv' for the 'termix' command to be available."
}

function Uninstall-Termix {
    Write-Host "Starting Termix uninstallation..."
    $ExePath = Join-Path $InstallDir $ExeName

    if (Test-Path $ExePath) {
        Remove-Item -Path $ExePath -Force
        Write-Host "✅ Termix executable removed from $ExePath"
    } else {
        Write-Host "Termix not found at $ExePath. Nothing to remove."
    }

    # Advise on PATH removal
    $UserPath = [System.Environment]::GetEnvironmentVariable("Path", "User")
    if ($UserPath -like "*$InstallDir*") {
        Write-Host "Note: '$InstallDir' is still in your PATH. You may want to remove it manually."
    }
}

# --- Main Logic ---
if ($Uninstall.IsPresent) {
    Uninstall-Termix
} else {
    Install-Termix
}
