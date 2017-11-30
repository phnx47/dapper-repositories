if($env:APPVEYOR_REPO_TAG -eq "false") {
    dotnet pack src\MicroOrm.Dapper.Repositories --include-symbols --no-build --version-suffix build-$env:APPVEYOR_BUILD_NUMBER -o artifacts\myget
}

if($env:APPVEYOR_REPO_TAG -eq "true") {
    dotnet pack src\MicroOrm.Dapper.Repositories --include-symbols --no-build -o artifacts\nuget
}