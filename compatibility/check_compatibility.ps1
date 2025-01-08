$baselineDir = "./"
$packagesDir = "../packages/"
$exitCode = 0

# Process each baseline package
Get-ChildItem -Path $baselineDir -Filter *.nupkg -Recurse | ForEach-Object {
    $baselinePackage = $_.FullName
    $version = if ($_.BaseName -match '(\d+\.\d+\.\d+(?:\.\d+)?)') { $matches[0] } else { "" }
    $packageId = $_.BaseName -replace "\.$version.*", ""

    Write-Host "`e[36mCompatibility check for `e[35m$packageId v$version`e[33m`n"

    # Select suppression file
    $suppressionFile = if (Test-Path "$packageId.suppress.xml") { "$packageId.suppress.xml" } else { "default.suppress.xml" }

    # Find the latest matching package
    $package = Get-ChildItem -Path $packagesDir -Filter "$packageId*.nupkg" | Sort-Object Name -Descending | Select-Object -First 1

    if ($package) {
        # Run API compatibility check
        $output = apicompat package $package.FullName --baseline-package $baselinePackage --noWarn "CP0003" --suppression-file $suppressionFile

        if ($output) {
            Write-Host "`e[32m$output`e[32mCompatibility check successful for `e[35m$packageId"
        } else {
            Write-Host "`e[31m$output`e[31mCompatibility errors detected for `e[35m$packageId"
            $exitCode = 1
        }
    }
    Write-Host "`e[0m"
}

exit $exitCode