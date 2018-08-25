dotnet test -c Release test/MicroOrm.Dapper.Repositories.Tests --no-build
if ($LastExitCode -ne 0) {
    $host.SetShouldExit($LastExitCode)
}