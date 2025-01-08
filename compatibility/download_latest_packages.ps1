$sourceDir = "../packages/"
$outputDir = "./"

Get-ChildItem $sourceDir -Filter *.nupkg | ForEach-Object {
    $packageId = $_.BaseName -replace '(\.\d+){3,4}([\-\+].+)?'
    nuget install $packageId -Source "https://api.nuget.org/v3/index.json" -OutputDirectory $outputDir -DependencyVersion Ignore
    if ($LASTEXITCODE -ne 0) { Copy-Item $_.FullName -Destination "./" }
}

exit 0