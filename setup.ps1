$UNITY_VERSION = "6000.3.10f1"

function Find-UnityMergeTool {
    $candidate = "C:\Program Files\Unity\Hub\Editor\$UNITY_VERSION\Editor\Data\Tools\UnityYAMLMerge.exe"
    if (Test-Path $candidate) { return $candidate }

    $hubEditorPath = "C:\Program Files\Unity\Hub\Editor"
    if (Test-Path $hubEditorPath) {
        $found = Get-ChildItem $hubEditorPath -Directory |
            ForEach-Object { "$($_.FullName)\Editor\Data\Tools\UnityYAMLMerge.exe" } |
            Where-Object { Test-Path $_ } |
            Select-Object -First 1
        if ($found) { return $found }
    }

    return $null
}

$mergeTool = Find-UnityMergeTool

if (-not $mergeTool) {
    Write-Host "WARNING: UnityYAMLMerge not found."
    Write-Host "Unity $UNITY_VERSION is not installed, or it is in a non-standard location."
    Write-Host "Configure manually after installing Unity:"
    Write-Host ""
    Write-Host "  git config --local merge.unityyamlmerge.name `"Unity SmartMerge`""
    Write-Host "  git config --local merge.unityyamlmerge.driver `"'<path to UnityYAMLMerge>' merge -p %O %B %A %P`""
    Write-Host "  git config --local merge.unityyamlmerge.recursive binary"
} else {
    git config --local merge.unityyamlmerge.name "Unity SmartMerge"
    git config --local merge.unityyamlmerge.driver "'$mergeTool' merge -p %O %B %A %P"
    git config --local merge.unityyamlmerge.recursive binary
    Write-Host "SmartMerge configured: $mergeTool"
}
