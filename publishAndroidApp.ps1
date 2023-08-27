param([String]$AndroidSigningPassword, [int]$ApplicationVersion )

$packageName = "com.codeuctivity.LockPDFy"

# https://learn.microsoft.com/en-us/dotnet/maui/android/deployment/publish-cli#create-a-keystore-file
# Signing using .net maui stack is a mess, finaly using https://stackoverflow.com/questions/50560045/sign-android-app-bundle-from-command-line it was accepted by google play store

# check if file exists
if (!(Test-Path "../GooglePlayStore.keystore")) {
	throw "../GooglePlayStore.keystore not found";
}

# keytool -export -rfc -keystore ../GooglePlayStore.keystore -alias LockPDFyMaui -file upload_certificate.pem
# keytool -list -keystore GooglePlayStore.keystore

if ([String]::IsNullOrEmpty($AndroidSigningPassword)) {
	$AndroidSigningPassword = Read-Host -Prompt "Enter Android Signing Password" -AsSecureString
}

if ([String]::IsNullOrEmpty($ApplicationVersion)) {
	$ApplicationVersion = Read-Host -Prompt "Enter Application Version" -AsSecureString
}

if (Test-Path -Path "./LockPDFyMaui/bin") {
	Write-Host "Removing existing directory"
	Remove-Item -Recurse -ErrorAction:Stop ./LockPDFyMaui/bin
}

dotnet clean
dotnet restore
dotnet build -c Release
dotnet test -c Release --no-build
dotnet publish LockPDFyMaui -f net7.0-android -c Release -p:ApplicationVersion=$ApplicationVersion -p:ApplicationDisplayVersion=$ApplicationVersion.0 -p:ApplicationId=$packageName
jarsigner -verbose -sigalg SHA256withRSA -digestalg SHA-256 -keystore ../GooglePlayStore.keystore .\LockPDFyMaui\bin\Release\net7.0-android\publish\$packageName.aab AndroidApps -storepass $AndroidSigningPassword
rm .\LockPDFyMaui\bin\Release\net7.0-android\publish\*signed.a*
explorer .\LockPDFyMaui\bin\Release\net7.0-android\publish\
