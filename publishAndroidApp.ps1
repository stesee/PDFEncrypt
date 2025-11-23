param([String]$AndroidSigningPassword, [int]$Version )

# run as admin to prepare dependencies
# choco install androidstudio openjdk -y
# click through update in adnroid studio
# trigger updates in Android studio
# dotnet workload install android maui 

$packageName = "com.codeuctivity.LockPDFy"

if ([String]::IsNullOrEmpty($AndroidSigningPassword)) {
	$AndroidSigningPassword = Read-Host -Prompt "Enter Android Signing Password" -AsSecureString
}

if ([String]::IsNullOrEmpty($Version)) {
	$Version = Read-Host -Prompt "Enter version" -AsSecureString
}

if (Test-Path -Path "./LockPDFyMaui/bin") {
	Write-Host "Removing existing directory"
	Remove-Item -Recurse -ErrorAction:Stop ./LockPDFyMaui/bin
}

dotnet clean
dotnet restore
dotnet build -c Release
dotnet test -c Release --no-build
dotnet publish LockPDFyMaui -f net10.0-android -c Release -p:Version=$Version -p:ApplicationId=$packageName -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=$PWD/myapp.keystore -p:AndroidSigningKeyAlias=myapp -p:AndroidSigningKeyPass=$AndroidSigningPassword -p:AndroidSigningStorePass=$AndroidSigningPassword
explorer .\LockPDFyMaui\bin\Release\net10.0-android\publish\
