$sourcePath = ".\src"
$testsPath = ".\tests"
$outputFile = ".\54_yazlab2_kaynakkod.txt"

# Folders to exclude
$excludedFolders = @("node_modules", "bin", "obj", "dist", ".angular", ".git")

if (Test-Path $outputFile) {
    Clear-Content $outputFile
}

function Append-Files($path, $extensions) {
    Get-ChildItem -Path $path -Recurse -File |
    Where-Object {
        $extensions -contains $_.Extension -and
        ($excludedFolders | Where-Object { $_ -in $_.FullName }) -eq $null
    } |
    ForEach-Object {
        Add-Content $outputFile "==============================="
        Add-Content $outputFile "FILE: $($_.FullName)"
        Add-Content $outputFile "==============================="
        Get-Content $_.FullName | Add-Content $outputFile
        Add-Content $outputFile "`n"
        Write-Host "✅ file $($_.FullName) added `n "
    }
}

# Backend
Append-Files $sourcePath @(".cs", ".py")
Write-Host "✅ source code under src generated successfully"
# Frontend
Append-Files $testsPath @(".cs", ".py")

Write-Host "✅ 54_yazlab2_kaynakkod.txt generated successfully"
