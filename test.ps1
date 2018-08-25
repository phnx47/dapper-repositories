dotnet test -c Release test/* --no-build
if ($LastExitCode -ne 0) {
    $host.SetShouldExit($LastExitCode)
}