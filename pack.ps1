if ($env:APPVEYOR_REPO_TAG -eq "false") {
  dotnet pack $env:APPVEYOR_BUILD_FOLDER\src\MicroOrm.Dapper.Repositories -c Release  --no-build --version-suffix build$env:APPVEYOR_BUILD_NUMBER -o artifacts\myget
}

if ($env:APPVEYOR_REPO_TAG -eq "true") {
  dotnet pack $env:APPVEYOR_BUILD_FOLDER\src\MicroOrm.Dapper.Repositories -c Release --no-build -o artifacts\nuget
}
