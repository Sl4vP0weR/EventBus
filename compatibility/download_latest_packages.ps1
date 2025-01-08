$sourceDir = "../packages/"
$outputDir = "./"

Get-ChildItem $sourceDir -Filter *.nupkg | ForEach-Object {
    $packageId = $_.BaseName -replace '\.\d+(\.\d+)*$'
    nuget install $packageId -Source "https://api.nuget.org/v3/index.json" -OutputDirectory $outputDir
    if ($LASTEXITCODE -ne 0) { Copy-Item $_.FullName -Destination "./" }
}

exit 0