param (
    [string]$date = $(Get-Date -Format "yyyy-MM-dd")
)

dotnet run -p .\src\Template\Template.csproj --ignore-failed-sources -- --date $date