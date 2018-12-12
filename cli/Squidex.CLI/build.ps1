$runtimes = @("win-x64", "win-x86", "linux-x64", "ubuntu-x64", "osx-x64")

foreach ($runtime in $runtimes) {
	Write-Host "> Compiling for $runtime"
	
	dotnet publish -c Release -r $runtime

	xcopy "Squidex.CLI\bin\Release\netcoreapp2.2\$runtime\publish" "out\$runtime\" /S /Y /Q
	
	Compress-Archive -Path "out\$runtime\*" -Force -DestinationPath "out\$runtime.zip" -CompressionLevel Optimal
	
	Write-Host "> Compiling for $runtime completed"
	Write-Host ""
}
