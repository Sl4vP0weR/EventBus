$baselineDir = "./"
$packagesDir = "../packages/"

$packageId = $args[0]

$suppressionFile = "$packageId.suppress.xml"

$package = Get-ChildItem -Path $packagesDir -Filter "$packageId*.nupkg" | Sort-Object Name -Descending | Select-Object -First 1
$baselinePackage = Get-ChildItem -Path $baselineDir -Filter "$packageId*.nupkg" | Sort-Object Name -Descending | Select-Object -First 1

if ($package) 
{
	apicompat package "$package" --baseline-package "$baselinePackage" --noWarn "CP0003" --generate-suppression-file --suppression-output-file "$suppressionFile"
}